import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ChangeDetectorRef } from '@angular/core';
import { HttpClient, HttpParams, HttpClientModule } from '@angular/common/http';

interface ConversionResult {
  from: string;
  to: string;
  amount: number;
  converted: number;
  rate: number;
  date: string;
}

@Component({
  selector: 'app-conversor',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './conversor.html',
  styleUrls: ['./conversor.css'],
})
export class Conversor {
  // Moedas
  currencies = ['BRL', 'USD', 'EUR', 'GBP', 'JPY', 'ARS', 'CAD'];

  from: string = 'BRL';
  to: string = 'USD';
  amount: number | null = null;

  result: ConversionResult | null = null;
  history: ConversionResult[] = [];

  loading = false;
  errorMessage = '';

  // ajuste para sua URL do backend
  private apiBase = 'https://localhost:7244/api/conversor';

  constructor(private cdr: ChangeDetectorRef, private http: HttpClient) {
    this.loadHistory();
  }

  invertCurrencies() {
    const t = this.from;
    this.from = this.to;
    this.to = t;
  }

  async loadHistory() {
    try {
      const url = `${this.apiBase}/history`;
      const list = await this.http.get<ConversionResult[]>(url).toPromise();
      this.history = list || [];
      this.cdr.detectChanges();
    } catch (err) {
      // não interrompe o usuário se o histórico não puder ser carregado
      console.error('Erro ao carregar histórico', err);
    }
  }

  async convert() {
    this.errorMessage = '';
    this.result = null;

    if (!this.amount || this.amount <= 0) {
      this.errorMessage = 'Digite um valor válido para converter.';
      return;
    }

    if (this.from === this.to) {
      this.errorMessage = 'Selecione moedas diferentes.';
      return;
    }

    this.loading = true;
    this.cdr.detectChanges();

    try {
      const params = new HttpParams()
        .set('from', this.from)
        .set('to', this.to)
        .set('amount', String(this.amount));

      const url = `${this.apiBase}/convert`;

      const res = await this.http.get<ConversionResult>(url, { params }).toPromise();

      if (res) {
        this.result = res;

        // adiciona no histórico local para exibir imediatamente
        this.history.unshift(res);
      } else {
        this.errorMessage = 'Resposta inválida do servidor.';
      }
    } catch (err: any) {
      console.error(err);
      if (err.status === 502) {
        this.errorMessage = 'Erro ao consultar o serviço externo de câmbio.';
      } else {
        this.errorMessage = 'Erro ao buscar taxa de câmbio.';
      }
    }

    this.loading = false;
    this.cdr.detectChanges();
  }

  clearHistory() {
    // opcional: chama endpoint para limpar histórico no backend
    // se quiser habilitar, descomente abaixo
    /*
    this.http.delete(`${this.apiBase}/history/clear`).subscribe({
      next: () => {
        this.history = [];
      },
      error: (err) => console.error(err)
    });
    */
    this.history = [];
  }
}