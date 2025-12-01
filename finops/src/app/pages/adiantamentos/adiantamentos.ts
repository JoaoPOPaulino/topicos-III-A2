import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { AdvanceRequestService } from '../../service/advence-request';

// -----------------------------------------------------------
// 1. Interfaces (Correspondem ao SolicitacaoAdiantamentoListDto do back-end)
// -----------------------------------------------------------

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

// Interface usada para exibição na tabela 
interface AdiantamentoExibicao {
  id: number;
  nome: string; // = solicitanteNome
  desc: string; // = descricao
  valor: string; // = valorFormatado
  moeda: string; // = moedaCodigo
  data: string;
  status: string; // = statusDescricao
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

  sidebarOpen = false;
  profileMenuOpen = false;
  loading = false;
  errorMessage = '';

  // Dados reais carregados da API
  allRequests: AdiantamentoExibicao[] = []; 

  search = '';
  status = '';
  dataInicial = '';
  dataFinal = '';

  pagina = 1;
  itensPorPagina = 10;
  paginaAtualizada: AdiantamentoExibicao[] = []; 
  totalPaginas = 1;

  ngOnInit() {
    this.loadRequests(); // Carrega os dados reais ao iniciar
  }

// -----------------------------------------------------------
// 2. FUNÇÃO DE CARREGAMENTO (CHAMA A API COM FILTROS)
// -----------------------------------------------------------

  loadRequests() {
    this.loading = true;
    this.errorMessage = '';
    
    // Mapear filtros Angular para parâmetros da API (serão limpos no serviço)
    const params = {
      search: this.search,
      status: this.status,
      dataInicial: this.dataInicial,
      dataFinal: this.dataFinal
    };

    this.advanceService.getAdvanceRequests(params).subscribe({
      next: (data: AdiantamentoItemDto[]) => {
        // Mapeia o DTO do back-end para o formato de exibição 
        this.allRequests = data.map(item => ({
            id: item.id,
            nome: item.solicitanteNome,
            desc: item.descricao,
            valor: item.valorFormatado, 
            moeda: item.moedaCodigo,
            data: item.dataCriacao,
            status: item.statusDescricao, 
        }));
        
        this.loading = false;
        this.totalPaginas = Math.ceil(this.allRequests.length / this.itensPorPagina);
        this.filtrarPaginacao(this.allRequests); // Aplica a paginação
      },
      error: (err) => {
        console.error('Erro ao carregar adiantamentos:', err);
        this.loading = false;
        if (err.status === 400) {
            this.errorMessage = 'Erro de validação nos filtros (400 Bad Request).';
        } else {
            this.errorMessage = 'Não foi possível carregar a lista. Verifique a API.';
        }
      }
    });
  }

// -----------------------------------------------------------
// 3. LÓGICA DE FILTRAGEM E PAGINAÇÃO
// -----------------------------------------------------------
  
  filtrar() {
    this.loadRequests(); // Recarrega a lista com os filtros atualizados
  }

  filtrarPaginacao(requests: AdiantamentoExibicao[]) {
    const inicio = (this.pagina - 1) * this.itensPorPagina;
    const fim = inicio + this.itensPorPagina;
    
    this.paginaAtualizada = requests.slice(inicio, fim);
  }

  atualizarPaginacao() {
    this.filtrarPaginacao(this.allRequests);
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
    this.status = '';
    this.dataInicial = '';
    this.dataFinal = '';
    this.filtrar(); // Chama loadRequests
  }

  novoAdiantamento() {
    this.router.navigate(['/novo-adiantamento']);
  }

  // Passar o ID (ainda não implementado no HTML, mas pronto aqui)
  verAdiantamento(id: number) {
    this.router.navigate(['/ver-adiantamento'], { queryParams: { id: id } }); 
  }
}