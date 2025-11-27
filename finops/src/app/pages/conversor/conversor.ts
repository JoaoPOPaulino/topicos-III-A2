import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

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
  imports: [CommonModule, FormsModule],
  templateUrl: './conversor.html',
  styleUrl: './conversor.css',
})
export class Conversor {
  
  currencies = ['BRL', 'USD', 'EUR', 'GBP', 'JPY', 'ARS', 'CAD'];

  from: string = 'BRL';
  to: string = 'USD';
  amount: number | null = null;

  result: ConversionResult | null = null;

  history: ConversionResult[] = [];

  loading = false;
  errorMessage = '';

  constructor() {}

  invertCurrencies() {
    const t = this.from;
    this.from = this.to;
    this.to = t;
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

    try {
      const pair = `${this.from}-${this.to}`;
      const url = `https://economia.awesomeapi.com.br/json/last/${pair}`;

      const response = await fetch(url);
      const data = await response.json();

      const key = pair.replace('-', '');
      const info = data[key];

      const rate = parseFloat(info.bid);
      const converted = this.amount * rate;

      this.result = {
        from: this.from,
        to: this.to,
        amount: this.amount,
        converted,
        rate,
        date: new Date().toLocaleString('pt-BR')
      };

      this.history.unshift(this.result);

    } catch (err) {
      this.errorMessage = 'Erro ao buscar taxa de câmbio.';
    }

    this.loading = false;
  }
}
