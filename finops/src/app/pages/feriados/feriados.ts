import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

interface Holiday {
  date: string;
  name: string;
  type: string;
}

@Component({
  selector: 'app-feriados',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './feriados.html',
  styleUrl: './feriados.css',
})
export class Feriados {
    ano: number = new Date().getFullYear();
  holidays: Holiday[] = [];
  loading = false;
  errorMessage = '';

  async ngOnInit() {
    this.buscarFeriados();
  }

  async buscarFeriados() {
    this.errorMessage = '';
    this.holidays = [];
    this.loading = true;

    try {
      const url = `https://brasilapi.com.br/api/feriados/v1/${this.ano}`;
      const response = await fetch(url);

      if (!response.ok) {
        throw new Error('Erro ao consultar a API.');
      }

      const data: Holiday[] = await response.json();

      // filtrando apenas feriados nacionais
      this.holidays = data.filter(h => h.type === 'national');

    } catch (err) {
      this.errorMessage = 'Não foi possível carregar os feriados. Tente outro ano.';
    }

    this.loading = false;
  }
}
