import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css',
})
export class Dashboard {
  stats = [
    {
      title: 'Total de Adiantamentos Pendentes',
      value: 'R$ 4.520,00',
      icon: 'clock',
      trend: '+12%',
      trendUp: false,
      color: '#F2C94C',
    },
    {
      title: 'Despesas em Revisão',
      value: 'R$ 2.310,00',
      icon: 'search',
      trend: '-5%',
      trendUp: true,
      color: '#004AAD',
    },
    {
      title: 'Pagamentos Atrasados',
      value: '3',
      icon: 'alert',
      trend: '-2',
      trendUp: true,
      color: '#E63946',
    },
    {
      title: 'Economia Mensal (PTAX)',
      value: 'R$ 1.280,00',
      icon: 'trending-up',
      trend: '+8%',
      trendUp: true,
      color: '#00B37E',
    },
  ];

  recentActivities = [
    {
      type: 'approval',
      user: 'Lucas Henderson',
      action: 'aprovou um adiantamento',
      time: '5 min atrás',
    },
    { type: 'payment', user: 'Ana Costa', action: 'solicitou pagamento', time: '12 min atrás' },
    { type: 'review', user: 'Carlos Silva', action: 'enviou para revisão', time: '1 hora atrás' },
    { type: 'approval', user: 'Mariana Torres', action: 'aprovou despesa', time: '2 horas atrás' },
  ];
}
