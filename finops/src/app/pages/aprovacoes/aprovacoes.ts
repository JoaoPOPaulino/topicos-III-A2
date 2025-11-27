import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

interface Approval {
  id: number;
  requester: string;
  description: string;
  amount: number;
  currency: string;
  date: string; // ISO yyyy-mm-dd
  status: 'Pendente' | 'Revisão' | 'Aprovado' | 'Rejeitado' | 'Pago' | 'Atrasado';
}

@Component({
  selector: 'app-aprovacoes',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './aprovacoes.html',
  styleUrl: './aprovacoes.css',
})
export class Aprovacoes implements OnInit {
  
  // filtros
  search = '';
  statusFilter = '';
  dateFrom = '';
  dateTo = '';

  // paginação
  currentPage = 1;
  itemsPerPage = 10;

  // dados mock (substituir por chamada HTTP ao backend)
  allApprovals: Approval[] = [
    { id: 2001, requester: 'Lucas Henderson', description: 'Reembolso viagem SP', amount: 220.00, currency: 'BRL', date: '2025-02-02', status: 'Pendente' },
    { id: 2002, requester: 'Ana Costa', description: 'Jantar reunião', amount: 80.00, currency: 'USD', date: '2025-01-31', status: 'Revisão' },
    { id: 2003, requester: 'Carlos Silva', description: 'Material escritório', amount: 130.00, currency: 'BRL', date: '2025-02-01', status: 'Aprovado' },
    { id: 2004, requester: 'Beatriz Lima', description: 'Translado aeroporto', amount: 40.00, currency: 'EUR', date: '2025-01-29', status: 'Rejeitado' },
    { id: 2005, requester: 'Mariana Torres', description: 'Hotel SP', amount: 210.00, currency: 'EUR', date: '2025-01-28', status: 'Pago' },
    { id: 2006, requester: 'João Paulo', description: 'Taxi', amount: 45.00, currency: 'BRL', date: '2025-02-04', status: 'Atrasado' },
    { id: 2007, requester: 'Eduardo Melo', description: 'Reunião SP', amount: 120.00, currency: 'EUR', date: '2025-02-01', status: 'Aprovado' },
    { id: 2008, requester: 'Bianca Souza', description: 'Uber', amount: 40.00, currency: 'BRL', date: '2025-02-03', status: 'Revisão' },
    { id: 2009, requester: 'Ricardo Lima', description: 'Hospedagem', amount: 200.00, currency: 'USD', date: '2025-01-24', status: 'Pago' },
    { id: 2010, requester: 'Juliana Prado', description: 'Material Escritório', amount: 95.00, currency: 'BRL', date: '2025-02-05', status: 'Pendente' },
    { id: 2011, requester: 'Ana Souza', description: 'Passagem Aérea', amount: 540.00, currency: 'USD', date: '2025-02-05', status: 'Aprovado' },
    { id: 2012, requester: 'Thiago Ramos', description: 'Combustível', amount: 200.00, currency: 'BRL', date: '2025-02-02', status: 'Revisão' },
    { id: 2013, requester: 'Cristina Alves', description: 'Hospedagem', amount: 310.00, currency: 'BRL', date: '2025-01-29', status: 'Pago' },
    { id: 2014, requester: 'Diego Santos', description: 'Almoço equipe', amount: 180.00, currency: 'BRL', date: '2025-02-04', status: 'Pendente' },
    { id: 2015, requester: 'Larissa Fonseca', description: 'Transporte', amount: 64.00, currency: 'USD', date: '2025-02-03', status: 'Aprovado' },
    { id: 2016, requester: 'Renato Silva', description: 'Alimentação', amount: 48.00, currency: 'EUR', date: '2025-02-05', status: 'Revisão' },
    { id: 2017, requester: 'Marcos Tavares', description: 'Uber', amount: 32.00, currency: 'BRL', date: '2025-02-05', status: 'Pendente' },
    { id: 2018, requester: 'Alice Martins', description: 'Passagem', amount: 233.00, currency: 'USD', date: '2025-02-02', status: 'Pago' },
    { id: 2019, requester: 'Felipe Rocha', description: 'Taxi', amount: 22.00, currency: 'BRL', date: '2025-02-04', status: 'Pendente' },
    { id: 2020, requester: 'Heloísa Cruz', description: 'Hotel', amount: 135.00, currency: 'EUR', date: '2025-01-28', status: 'Aprovado' },
  ];

  filteredApprovals: Approval[] = [];

  constructor(private router: Router) {}

  ngOnInit(): void {
    this.applyFilters();
  }

  applyFilters(): void {
    const s = this.search.trim().toLowerCase();
    const sf = (this.statusFilter || '').trim();
    const from = this.dateFrom ? new Date(this.dateFrom) : null;
    const to = this.dateTo ? new Date(this.dateTo) : null;

    this.filteredApprovals = this.allApprovals.filter(a => {
      // busca textual
      const text = `${a.requester} ${a.description} ${a.amount} ${a.currency}`.toLowerCase();
      if (s && !text.includes(s)) return false;

      // status
      if (sf && a.status !== sf) return false;

      // datas
      const ad = new Date(a.date);
      if (from && ad < from) return false;
      if (to) {
        // include same day
        const toEnd = new Date(to);
        toEnd.setHours(23,59,59,999);
        if (ad > toEnd) return false;
      }

      return true;
    });

    // reset page
    this.currentPage = 1;
  }

  // tabela paginada
  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredApprovals.length / this.itemsPerPage));
  }

  get pageItems(): Approval[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredApprovals.slice(start, start + this.itemsPerPage);
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
    }
  }

  verAdiantamento() {
    this.router.navigate(['/ver-adiantamento']);
  }

  goToEdit() {
    this.router.navigate(['/editar-adiantamento']);
  }

  // helper: formata valor
  formatMoney(v: number, currency = 'BRL'): string {
    const locale = 'pt-BR';
    const cur = currency || 'BRL';
    return new Intl.NumberFormat(locale, { style: 'currency', currency: cur }).format(v);
  }

  // helper to return CSS class from status
  statusClass(status: Approval['status']): string {
    return status ? status.replace(/\s+/g, '') : '';
  }
}
