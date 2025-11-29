import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient, HttpClientModule } from '@angular/common/http';
import { ChangeDetectorRef } from '@angular/core';

interface Holiday {
  date: string;
  name: string;
  type: string;
}

@Component({
  selector: 'app-feriados',
  standalone: true,
  imports: [CommonModule, FormsModule, HttpClientModule],
  templateUrl: './feriados.html',
  styleUrls: ['./feriados.css'],
})
export class Feriados {
  ano: number = new Date().getFullYear();
  holidays: Holiday[] = [];
  loading = false;
  errorMessage = '';

  constructor(private http: HttpClient, private cdr: ChangeDetectorRef) {}

  ngOnInit() {
    this.buscarFeriados();
  }

  buscarFeriados() {
    this.errorMessage = '';
    this.holidays = [];
    this.loading = true;

    const url = `https://brasilapi.com.br/api/feriados/v1/${this.ano}`;

    this.http.get<Holiday[]>(url).subscribe({
      next: (data) => {
        this.holidays = data.filter((h) => h.type === 'national');
        this.loading = false;
        this.cdr.detectChanges();
      },
      error: () => {
        this.errorMessage = 'Não foi possível carregar os feriados. Tente outro ano.';
        this.loading = false;
      },
    });
  }

  getMonthName(dateString: string): string {
    const date = new Date(dateString + 'T12:00:00');
    return date.toLocaleDateString('pt-BR', { month: 'short' }).replace('.', '');
  }

  getDay(dateString: string): string {
    const date = new Date(dateString + 'T12:00:00');
    return date.getDate().toString().padStart(2, '0');
  }

  getDayOfWeek(dateString: string): string {
    const date = new Date(dateString + 'T12:00:00');
    return date.toLocaleDateString('pt-BR', { weekday: 'long' });
  }
}
