import { Component, OnInit, inject, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { AdvanceRequestService } from '../../service/advence-request';

interface AdiantamentoItemDto {
  id: number;
  solicitanteNome: string;
  descricao: string;
  valor: number;
  moedaCodigo: string;
  valorFormatado: string;
  dataCriacao: string;
  statusDescricao: string;
}

interface AdiantamentoExibicao {
  id: number;
  nome: string;
  desc: string;
  valor: string;
  moeda: string;
  data: string;
  status: string;
}

@Component({
  selector: 'app-adiantamentos',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, HttpClientModule],
  templateUrl: './adiantamentos.html',
  styleUrl: './adiantamentos.css',
})
export class Adiantamentos implements OnInit {
  private router = inject(Router);
  private advanceService = inject(AdvanceRequestService);
  private cdr = inject(ChangeDetectorRef);

  sidebarOpen = false;
  profileMenuOpen = false;
  loading = false;
  errorMessage = '';

  allRequests: AdiantamentoExibicao[] = [];

  search = '';
  // ✅ PADRÃO: Mostrar apenas Pendente na aba Adiantamentos
  status = 'Pendente'; 
  dataInicial = '';
  dataFinal = '';

  pagina = 1;
  itensPorPagina = 10;
  paginaAtualizada: AdiantamentoExibicao[] = [];
  totalPaginas = 1;

  private filterTimeout: any;
  private isLoadingRequest = false;

  ngOnInit() {
    console.log('ngOnInit chamado. Status padrão: Pendente');
    this.loadRequests();
  }

  loadRequests() {
    if (this.isLoadingRequest) {
      console.log('Já existe uma requisição em andamento');
      return;
    }

    this.isLoadingRequest = true;
    this.loading = true;
    this.errorMessage = '';
    this.cdr.detectChanges();

    const params = {
      search: this.search,
      status: this.status, // ✅ Envia o status 'Pendente' (ou outro filtro)
      dataInicial: this.dataInicial,
      dataFinal: this.dataFinal,
    };

    console.log('Carregando adiantamentos com params:', params);

    this.advanceService.getAdvanceRequests(params).subscribe({
      next: (data: AdiantamentoItemDto[]) => {
        console.log('Dados recebidos:', data);

        this.allRequests = data.map((item) => ({
          id: item.id,
          nome: item.solicitanteNome,
          desc: item.descricao,
          valor: item.valorFormatado,
          moeda: item.moedaCodigo,
          data: item.dataCriacao,
          status: item.statusDescricao,
        }));

        this.loading = false;
        this.isLoadingRequest = false;
        this.totalPaginas = Math.ceil(this.allRequests.length / this.itensPorPagina);
        this.filtrarPaginacao(this.allRequests);

        console.log('Página atualizada:', this.paginaAtualizada);

        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('❌ Erro ao carregar adiantamentos:', err);
        this.loading = false;
        this.isLoadingRequest = false;

        if (err.status === 0) {
          this.errorMessage = 'Falha de conexão. Verifique se a API está rodando em HTTPS:7244.';
        } else if (err.status === 400) {
          this.errorMessage = 'Erro de validação nos filtros (400 Bad Request).';
        } else {
          this.errorMessage = 'Não foi possível carregar a lista. Erro interno.';
        }

        this.cdr.detectChanges();
      },
    });
  }

  filtrar() {
    clearTimeout(this.filterTimeout);
    this.filterTimeout = setTimeout(() => {
      this.pagina = 1;
      this.loadRequests();
    }, 300);
  }

  filtrarPaginacao(requests: AdiantamentoExibicao[]) {
    const inicio = (this.pagina - 1) * this.itensPorPagina;
    const fim = inicio + this.itensPorPagina;

    this.totalPaginas = Math.ceil(requests.length / this.itensPorPagina);
    this.paginaAtualizada = requests.slice(inicio, fim);

    console.log('filtrarPaginacao - itens na página:', this.paginaAtualizada.length);
  }

  atualizarPaginacao() {
    this.filtrarPaginacao(this.allRequests);
    this.cdr.detectChanges();
  }

  paginaAnterior() {
    if (this.pagina > 1) {
      this.pagina--;
      this.atualizarPaginacao();
    }
  }

  paginaProxima() {
    if (this.pagina < this.totalPaginas) {
      this.pagina++;
      this.atualizarPaginacao();
    }
  }

  limparFiltros() {
    this.search = '';
    // ✅ Garante que o status volte para 'Pendente' após limpar
    this.status = 'Pendente'; 
    this.dataInicial = '';
    this.dataFinal = '';
    clearTimeout(this.filterTimeout);
    this.pagina = 1;
    this.loadRequests();
  }

  novoAdiantamento() {
    this.router.navigate(['/novo-adiantamento']);
  }

  verAdiantamento(id: number) {
    this.router.navigate(['/ver-adiantamento'], { queryParams: { id: id } });
  }

  ngOnDestroy() {
    if (this.filterTimeout) {
      clearTimeout(this.filterTimeout);
    }
  }
}