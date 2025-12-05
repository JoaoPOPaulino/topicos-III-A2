import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule, ActivatedRoute } from '@angular/router';
import { AdvanceRequestService } from '../../service/advence-request';
import { HttpClientModule } from '@angular/common/http';

interface ApprovalItem {
  id: number;
  requester: string;
  description: string;
  amount: number;
  currency: string;
  date: string;
  status: string;
}

@Component({
  selector: 'app-aprovacoes',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, HttpClientModule],
  templateUrl: './aprovacoes.html',
  styleUrl: './aprovacoes.css',
})
export class Aprovacoes implements OnInit {
  private advanceRequest = inject(AdvanceRequestService);
  private router = inject(Router);
  private route = inject(ActivatedRoute); // ✅ NOVO
  private cdr = inject(ChangeDetectorRef);

  search = '';
  statusFilter = '';
  dateFrom = '';
  dateTo = '';
  loading = false;
  errorMessage = '';

  currentPage = 1;
  itemsPerPage = 10;

  allApprovals: ApprovalItem[] = [];
  filteredApprovals: ApprovalItem[] = [];

  ngOnInit(): void {
    console.log('Aprovacoes ngOnInit chamado');

    // ✅ Captura o filtro de status da query string
    this.route.queryParams.subscribe((params) => {
      if (params['status']) {
        console.log('📌 Filtro de status recebido:', params['status']);
        this.statusFilter = params['status'];
      }
      this.loadApprovals();
    });
  }

  loadApprovals(): void {
    if (this.loading) {
      console.log('Já existe uma requisição em andamento');
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    const params = {
      search: this.search,
      status: this.statusFilter,
      dataInicial: this.dateFrom,
      dataFinal: this.dateTo,
    };

    console.log('Carregando aprovações com params:', params);

    this.advanceRequest.getAdvanceRequests(params).subscribe({
      next: (data: any[]) => {
        console.log('Dados de aprovações recebidos:', data);

        this.allApprovals = data.map((item) => ({
          id: item.id,
          requester: item.solicitanteNome,
          description: item.descricao,
          amount: item.valor,
          currency: item.moedaCodigo,
          date: item.dataCriacao,
          status: item.statusDescricao,
        }));

        this.loading = false;
        this.applyFilters();

        console.log('Aprovações carregadas:', this.filteredApprovals.length);
        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('Erro ao carregar aprovações:', err);
        this.loading = false;

        if (err.status === 0) {
          this.errorMessage = 'Falha de conexão. Verifique se a API está rodando.';
        } else if (err.status === 400) {
          this.errorMessage = 'Erro de validação nos filtros.';
        } else {
          this.errorMessage = 'Falha ao carregar aprovações.';
        }

        this.cdr.detectChanges();
      },
    });
  }

  applyFilters(): void {
    const s = this.search.trim().toLowerCase();
    const sf = (this.statusFilter || '').trim();
    const from = this.dateFrom ? new Date(this.dateFrom) : null;
    const to = this.dateTo ? new Date(this.dateTo) : null;

    this.filteredApprovals = this.allApprovals.filter((a) => {
      const text = `${a.requester} ${a.description} ${a.amount} ${a.currency}`.toLowerCase();
      if (s && !text.includes(s)) return false;

      if (sf && a.status !== sf) return false;

      const ad = new Date(a.date);
      if (from && ad < from) return false;
      if (to) {
        const toEnd = new Date(to);
        toEnd.setHours(23, 59, 59, 999);
        if (ad > toEnd) return false;
      }
      return true;
    });

    this.currentPage = 1;
    this.cdr.detectChanges();
  }

  get totalPages(): number {
    return Math.max(1, Math.ceil(this.filteredApprovals.length / this.itemsPerPage));
  }

  get pageItems(): ApprovalItem[] {
    const start = (this.currentPage - 1) * this.itemsPerPage;
    return this.filteredApprovals.slice(start, start + this.itemsPerPage);
  }

  prevPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.cdr.detectChanges();
    }
  }

  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.cdr.detectChanges();
    }
  }

  limparFiltros(): void {
    this.search = '';
    this.statusFilter = '';
    this.dateFrom = '';
    this.dateTo = '';

    // ✅ Remove queryParams ao limpar filtros
    this.router.navigate([], {
      relativeTo: this.route,
      queryParams: {},
    });

    this.loadApprovals();
  }

  // ✅ NAVEGAÇÃO SEM CLIQUE DUPLO
  verAdiantamento(id: number, event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    this.router.navigate(['/ver-adiantamento'], {
      queryParams: { id: id },
    });
  }

  goToEdit(id: number, event: Event): void {
    event.preventDefault();
    event.stopPropagation();

    this.router.navigate(['/editar-adiantamento'], {
      queryParams: { id: id },
    });
  }

  formatMoney(v: number, currency = 'BRL'): string {
    const locale = 'pt-BR';
    const cur = currency || 'BRL';
    return new Intl.NumberFormat(locale, { style: 'currency', currency: cur }).format(v);
  }

  statusClass(status: string): string {
    return status ? status.replace(/\s+/g, '') : '';
  }
}
