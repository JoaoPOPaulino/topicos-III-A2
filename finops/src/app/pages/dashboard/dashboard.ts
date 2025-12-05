// src/app/pages/dashboard/dashboard.ts
import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import {
  DashboardService,
  DashboardStats,
  DashboardActivity,
  DashboardCharts,
} from '../../service/dashboard';

interface StatCard {
  title: string;
  value: string;
  icon: string;
  trend: string;
  trendUp: boolean;
  color: string;
  route: string;
  filter: string | null;
}

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard implements OnInit {
  private dashboardService = inject(DashboardService);
  private router = inject(Router);

  loading = false;
  errorMessage = '';

  stats: StatCard[] = [];
  recentActivities: DashboardActivity[] = [];
  chartData: DashboardCharts | null = null;

  ngOnInit() {
    this.loadDashboardData();
  }

  loadDashboardData() {
    this.loading = true;
    this.errorMessage = '';

    this.dashboardService.getStats().subscribe({
      next: (data: DashboardStats) => {
        console.log('📊 Estatísticas carregadas:', data);
        this.buildStatCards(data);
        this.loading = false;
      },
      error: (err) => {
        console.error('❌ Erro ao carregar estatísticas:', err);
        this.errorMessage = 'Erro ao carregar dados do dashboard';
        this.loading = false;
        this.loadFallbackData();
      },
    });

    this.dashboardService.getRecentActivities().subscribe({
      next: (activities) => {
        console.log('📋 Atividades carregadas:', activities);
        this.recentActivities = activities;
      },
      error: (err) => {
        console.error('❌ Erro ao carregar atividades:', err);
      },
    });

    this.dashboardService.getChartData().subscribe({
      next: (charts) => {
        console.log('📈 Gráficos carregados:', charts);
        this.chartData = charts;
      },
      error: (err) => {
        console.error('❌ Erro ao carregar gráficos:', err);
      },
    });
  }

  private buildStatCards(data: DashboardStats) {
    this.stats = [
      {
        title: 'Total de Adiantamentos Pendentes',
        value: data.totalAdiantamentosPendentesFormatado,
        icon: 'clock',
        trend: `${data.trendAdiantamentos > 0 ? '+' : ''}${data.trendAdiantamentos}%`,
        trendUp: data.trendAdiantamentos > 0,
        color: '#F2C94C',
        route: '/adiantamentos',
        filter: null,
      },
      {
        title: 'Despesas em Revisão',
        value: data.despesasEmRevisaoFormatado,
        icon: 'search',
        trend: `${data.trendDespesas > 0 ? '+' : ''}${data.trendDespesas}%`,
        trendUp: data.trendDespesas < 0,
        color: '#004AAD',
        route: '/aprovacoes',
        filter: 'Revisão',
      },
      {
        title: 'Pagamentos Atrasados',
        value: data.pagamentosAtrasados.toString(),
        icon: 'alert',
        trend: `${data.trendPagamentos > 0 ? '+' : ''}${data.trendPagamentos}`,
        trendUp: data.trendPagamentos < 0,
        color: '#E63946',
        route: '/aprovacoes',
        filter: 'Atrasado',
      },
      {
        title: 'Economia Mensal (PTAX)',
        value: data.economiaMensalFormatado,
        icon: 'trending-up',
        trend: `${data.trendEconomia > 0 ? '+' : ''}${data.trendEconomia}%`,
        trendUp: data.trendEconomia > 0,
        color: '#00B37E',
        route: '/conversor',
        filter: null,
      },
    ];
  }

  private loadFallbackData() {
    this.stats = [
      {
        title: 'Total de Adiantamentos Pendentes',
        value: 'R$ 4.520,00',
        icon: 'clock',
        trend: '+12%',
        trendUp: true,
        color: '#F2C94C',
        route: '/adiantamentos',
        filter: null,
      },
      {
        title: 'Despesas em Revisão',
        value: 'R$ 2.310,00',
        icon: 'search',
        trend: '-5%',
        trendUp: true,
        color: '#004AAD',
        route: '/aprovacoes',
        filter: 'Revisão',
      },
      {
        title: 'Pagamentos Atrasados',
        value: '3',
        icon: 'alert',
        trend: '-2',
        trendUp: true,
        color: '#E63946',
        route: '/aprovacoes',
        filter: 'Atrasado',
      },
      {
        title: 'Economia Mensal (PTAX)',
        value: 'R$ 1.280,00',
        icon: 'trending-up',
        trend: '+8%',
        trendUp: true,
        color: '#00B37E',
        route: '/conversor',
        filter: null,
      },
    ];

    this.recentActivities = [
      {
        id: 1,
        type: 'approval',
        userName: 'Lucas Henderson',
        action: 'aprovou um adiantamento',
        createdAt: new Date().toISOString(),
        timeAgo: '5 min atrás',
      },
      {
        id: 2,
        type: 'payment',
        userName: 'Ana Costa',
        action: 'solicitou pagamento',
        createdAt: new Date().toISOString(),
        timeAgo: '12 min atrás',
      },
    ];
  }

  // ✅ NAVEGAÇÃO SIMPLIFICADA (sem event)
  navigateToStat(stat: StatCard) {
    console.log('🔗 Navegando para:', stat.route, 'Filtro:', stat.filter);

    if (stat.filter) {
      this.router.navigate([stat.route], {
        queryParams: { status: stat.filter },
      });
    } else {
      this.router.navigate([stat.route]);
    }
  }

  navigateToActivity(activity: DashboardActivity) {
    if (activity.relatedEntityId) {
      this.router.navigate(['/ver-adiantamento'], {
        queryParams: { id: activity.relatedEntityId },
      });
    }
  }

  refresh() {
    this.loadDashboardData();
  }
}
