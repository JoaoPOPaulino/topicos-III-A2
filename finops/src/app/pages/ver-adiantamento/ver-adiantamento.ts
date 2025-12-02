// src/app/pages/ver-adiantamento/ver-adiantamento.ts

import { Component, OnInit, HostListener, inject, ChangeDetectorRef } from '@angular/core'; // Adicionado ChangeDetectorRef
import { CommonModule, Location } from '@angular/common';
import { ActivatedRoute, Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { AdvanceRequestService } from '../../service/advence-request';

// Interface do DTO de Detalhe (Corresponde ao SolicitacaoAdiantamentoDetailDto do back-end)
interface AdiantamentoDetalhes {
  id: number;
  solicitanteNome: string;
  departamentoNome: string;
  justificativaCompleta: string;
  valorFormatado: string;
  moedaCodigo: string;
  dataPagamentoRequerida: string;
  dataPagamentoAjustada: string | null;
  observacoes: string | null;
  statusDescricao: string;
  criadoPorNome: string;
  dataCriacao: string;
  anexos: string[];

  historico: any[]; // --- CHAVES DE EXIBIÇÃO SIMPLIFICADAS PARA O TEMPLATE (RESOLVE ERROS NO HTML) ---
  status: string;
  valor: string;
  moeda: string;
  solicitante: string;
  departamento: string;
  descricao: string;
  email: string; // Mock
  categoria: string; // Mock
  ptax: string; // Mock
}

@Component({
  selector: 'app-ver-adiantamento',
  standalone: true,
  imports: [CommonModule, HttpClientModule],
  templateUrl: './ver-adiantamento.html',
  styleUrls: ['./ver-adiantamento.css'],
})
export class VerAdiantamento implements OnInit {
  private router = inject(Router);
  private location = inject(Location);
  private advanceService = inject(AdvanceRequestService);
  private route = inject(ActivatedRoute);
  private cdr = inject(ChangeDetectorRef); // INJETAR ChangeDetectorRef

  loading = true;
  errorMessage = ''; // Inicialização completa com valores de fallback para evitar erros de leitura no template

  dados: AdiantamentoDetalhes = {
    id: 0,
    solicitanteNome: '',
    departamentoNome: '',
    justificativaCompleta: '',
    valorFormatado: 'R$ 0,00',
    moedaCodigo: '',
    statusDescricao: 'Carregando...',
    dataCriacao: '',
    dataPagamentoRequerida: '',
    criadoPorNome: '',
    observacoes: null,
    dataPagamentoAjustada: null,
    anexos: [], // Chaves de exibição e mocks (fallback)

    solicitante: 'Carregando...',
    departamento: 'N/A',
    descricao: 'Carregando...',
    email: 'N/A',
    categoria: 'N/A',
    ptax: 'N/A',
    status: 'Carregando...',
    valor: 'R$ 0,00',
    moeda: 'BRL',

    historico: [
      { data: '01/01/2025', acao: 'Aguardando dados da API', usuario: 'Sistema', tipo: 'create' },
    ],
  };

  ngOnInit(): void {
    console.log('ngOnInit: Componente VerAdiantamento iniciado.');
    this.route.queryParams.subscribe((params) => {
      const id = params['id'];
      console.log(`ngOnInit: ID da URL recebido: ${id}`);
      if (id) {
        this.loadDetails(Number(id));
      } else {
        this.errorMessage = 'ID do adiantamento não encontrado na URL.';
        this.loading = false;
        this.location.back();
      }
    });
  }

  loadDetails(id: number) {
    this.loading = true;
    console.log(`LOAD: Chamando API para ID #${id}`);

    this.advanceService.getAdvanceRequestById(id).subscribe({
      next: (dataApi: any) => {
        console.log('API SUCCESS: Dados brutos recebidos:', dataApi); // Mapeamento Explícito
        this.dados = {
          ...dataApi, // 1. Mapeamento de chaves que o HTML exige:
          status: dataApi.statusDescricao,
          valor: dataApi.valorFormatado,
          moeda: dataApi.moedaCodigo,
          solicitante: dataApi.solicitanteNome,
          departamento: dataApi.departamentoNome,
          descricao: dataApi.justificativaCompleta, // 2. Mocks (Mantidos)
          email: 'solicitante@empresa.com',
          categoria: 'Viagem',
          ptax: 'N/A',
          historico: this.dados.historico, // Preserva o mock
        };

        console.log('MAPPING RESULT: Dados finais mapeados:', this.dados);

        this.loading = false;
        this.cdr.detectChanges(); // FORÇA A DETECÇÃO DE MUDANÇA
      },
      error: (err) => {
        console.error(`❌ Erro ${err.status} ao carregar detalhes:`, err);
        this.errorMessage = `Falha ao carregar o adiantamento #${id}. Código: ${err.status}`;
        this.loading = false;
      },
    });
  } // Métodos para navegação e lógica de status

  editar() {
    this.router.navigate(['/editar-adiantamento'], {
      queryParams: { id: this.dados.id },
    });
  }

  voltar(): void {
    this.location.back();
  } // Função para mapear o status para classes CSS
  getStatusClass(): string {
    const status = this.dados?.statusDescricao;
    return status ? status.replace(/\s+/g, '') : 'Pendente';
  } // Formata os anexos para que o HTML consiga ler o tipo e tamanho
  getAnexosFormatados() {
    if (!this.dados || !this.dados.anexos) return [];
    return this.dados.anexos.map((nome) => ({
      nome: nome,
      tamanho: '1.2 MB',
      tipo: nome.toLowerCase().endsWith('.pdf') ? 'pdf' : 'image',
    }));
  }

  downloadAnexo(anexo: any) {
    console.log('Download do anexo:', anexo.nome);
  }
  getHistoricoIcon(tipo: string): string {
    return tipo;
  }
}
