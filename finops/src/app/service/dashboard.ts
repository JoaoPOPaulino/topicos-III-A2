// src/app/services/dashboard.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface DashboardStats {
  totalAdiantamentosPendentes: number;
  totalAdiantamentosPendentesFormatado: string;
  quantidadeAdiantamentosPendentes: number;

  despesasEmRevisao: number;
  despesasEmRevisaoFormatado: string;
  quantidadeDespesasEmRevisao: number;

  pagamentosAtrasados: number;

  economiaMensal: number;
  economiaMensalFormatado: string;

  trendAdiantamentos: number;
  trendDespesas: number;
  trendPagamentos: number;
  trendEconomia: number;
}

export interface DashboardActivity {
  id: number;
  type: 'approval' | 'payment' | 'review';
  userName: string;
  action: string;
  createdAt: string;
  timeAgo: string;
  relatedEntityId?: number;
}

export interface ChartDataPoint {
  label: string;
  count: number;
  value: number;
  color: string;
}

export interface DashboardCharts {
  adiantamentosPorStatus: ChartDataPoint[];
  despesasPorCategoria: ChartDataPoint[];
}

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7244/api/Dashboard';

  getStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.baseUrl}/stats`);
  }

  getRecentActivities(): Observable<DashboardActivity[]> {
    return this.http.get<DashboardActivity[]>(`${this.baseUrl}/activities`);
  }

  getChartData(): Observable<DashboardCharts> {
    return this.http.get<DashboardCharts>(`${this.baseUrl}/charts`);
  }
}
