import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, RouterModule } from '@angular/router';

@Component({
  selector: 'app-adiantamentos',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './adiantamentos.html',
  styleUrl: './adiantamentos.css',
})
export class Adiantamentos {
  constructor(private router: Router) {}

  sidebarOpen = false;
  profileMenuOpen = false;

  // Dados mock
  dados = [
    {
      id: 1001,
      nome: 'Lucas Henderson',
      desc: 'Viagem Brasília',
      valor: 'R$ 850,00',
      moeda: 'BRL',
      data: '2025-01-31',
      status: 'Pendente',
    },
    {
      id: 1002,
      nome: 'Ana Costa',
      desc: 'Compra materiais',
      valor: '$ 320.00',
      moeda: 'USD',
      data: '2025-02-02',
      status: 'Rejeitado',
    },
    {
      id: 1003,
      nome: 'Carlos Silva',
      desc: 'Alimentação',
      valor: 'R$ 150,00',
      moeda: 'BRL',
      data: '2025-02-02',
      status: 'Atrasado',
    },
    {
      id: 1004,
      nome: 'Mariana Torres',
      desc: 'Hotel',
      valor: '€ 210,00',
      moeda: 'EUR',
      data: '2025-01-28',
      status: 'Pago',
    },
    {
      id: 1005,
      nome: 'João Paulo',
      desc: 'Taxi',
      valor: 'R$ 45,00',
      moeda: 'BRL',
      data: '2025-02-04',
      status: 'Pendente',
    },

    {
      id: 1006,
      nome: 'Eduardo Melo',
      desc: 'Reunião SP',
      valor: '€ 120,00',
      moeda: 'EUR',
      data: '2025-02-01',
      status: 'Aprovado',
    },
    {
      id: 1007,
      nome: 'Bianca Souza',
      desc: 'Uber',
      valor: 'R$ 40,00',
      moeda: 'BRL',
      data: '2025-02-03',
      status: 'Revisão',
    },
    {
      id: 1008,
      nome: 'Ricardo Lima',
      desc: 'Hospedagem',
      valor: '$ 200.00',
      moeda: 'USD',
      data: '2025-01-24',
      status: 'Pago',
    },
    {
      id: 1009,
      nome: 'Juliana Prado',
      desc: 'Material Escritório',
      valor: 'R$ 95,00',
      moeda: 'BRL',
      data: '2025-02-05',
      status: 'Pendente',
    },
    {
      id: 1010,
      nome: 'Ana Souza',
      desc: 'Passagem Aérea',
      valor: '$ 540.00',
      moeda: 'USD',
      data: '2025-02-05',
      status: 'Aprovado',
    },

    {
      id: 1011,
      nome: 'Thiago Ramos',
      desc: 'Combustível',
      valor: 'R$ 200,00',
      moeda: 'BRL',
      data: '2025-02-02',
      status: 'Revisão',
    },
    {
      id: 1012,
      nome: 'Cristina Alves',
      desc: 'Hospedagem',
      valor: 'R$ 310,00',
      moeda: 'BRL',
      data: '2025-01-29',
      status: 'Pago',
    },
    {
      id: 1013,
      nome: 'Diego Santos',
      desc: 'Almoço equipe',
      valor: 'R$ 180,00',
      moeda: 'BRL',
      data: '2025-02-04',
      status: 'Pendente',
    },
    {
      id: 1014,
      nome: 'Larissa Fonseca',
      desc: 'Transporte',
      valor: '$ 64.00',
      moeda: 'USD',
      data: '2025-02-03',
      status: 'Aprovado',
    },
    {
      id: 1015,
      nome: 'Renato Silva',
      desc: 'Alimentação',
      valor: '€ 48,00',
      moeda: 'EUR',
      data: '2025-02-05',
      status: 'Revisão',
    },

    {
      id: 1016,
      nome: 'Marcos Tavares',
      desc: 'Uber',
      valor: 'R$ 32,00',
      moeda: 'BRL',
      data: '2025-02-05',
      status: 'Pendente',
    },
    {
      id: 1017,
      nome: 'Alice Martins',
      desc: 'Passagem',
      valor: '$ 233.00',
      moeda: 'USD',
      data: '2025-02-02',
      status: 'Pago',
    },
    {
      id: 1018,
      nome: 'Felipe Rocha',
      desc: 'Taxi',
      valor: 'R$ 22,00',
      moeda: 'BRL',
      data: '2025-02-04',
      status: 'Pendente',
    },
    {
      id: 1019,
      nome: 'Heloísa Cruz',
      desc: 'Hotel',
      valor: '€ 135,00',
      moeda: 'EUR',
      data: '2025-01-28',
      status: 'Aprovado',
    },
    {
      id: 1020,
      nome: 'Bruna Ferreira',
      desc: 'Material',
      valor: 'R$ 80,00',
      moeda: 'BRL',
      data: '2025-02-01',
      status: 'Revisão',
    },
  ];

  search = '';
  status = '';
  dataInicial = '';
  dataFinal = '';

  pagina = 1;
  itensPorPagina = 10;
  paginaAtualizada: any[] = [];
  totalPaginas = 1;

  ngOnInit() {
    this.filtrar();
  }

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  toggleProfileMenu() {
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  filtrar() {
    let filtrados = this.dados.filter((item) => {
      const texto = (
        item.nome +
        ' ' +
        item.desc +
        ' ' +
        item.valor +
        ' ' +
        item.status
      ).toLowerCase();

      if (!texto.includes(this.search.toLowerCase())) return false;
      if (this.status && item.status !== this.status) return false;

      const data = new Date(item.data);
      if (this.dataInicial && data < new Date(this.dataInicial)) return false;
      if (this.dataFinal && data > new Date(this.dataFinal)) return false;

      return true;
    });

    this.totalPaginas = Math.ceil(filtrados.length / this.itensPorPagina);
    this.pagina = 1;

    this.paginaAtualizada = filtrados.slice(0, this.itensPorPagina);
  }

  atualizarPaginacao() {
    const inicio = (this.pagina - 1) * this.itensPorPagina;
    const fim = inicio + this.itensPorPagina;

    let filtrados = this.dados.filter((item) => {
      const texto = (
        item.nome +
        ' ' +
        item.desc +
        ' ' +
        item.valor +
        ' ' +
        item.status
      ).toLowerCase();

      if (!texto.includes(this.search.toLowerCase())) return false;
      if (this.status && item.status !== this.status) return false;

      const data = new Date(item.data);
      if (this.dataInicial && data < new Date(this.dataInicial)) return false;
      if (this.dataFinal && data > new Date(this.dataFinal)) return false;

      return true;
    });

    this.paginaAtualizada = filtrados.slice(inicio, fim);
  }

  paginaAnterior() {
    this.pagina--;
    this.atualizarPaginacao();
  }

  paginaProxima() {
    this.pagina++;
    this.atualizarPaginacao();
  }

  limparFiltros() {
    this.search = '';
    this.status = '';
    this.dataInicial = '';
    this.dataFinal = '';
    this.filtrar();
  }

  novoAdiantamento() {
    this.router.navigate(['/novo-adiantamento']);
  }

  verAdiantamento() {
    this.router.navigate(['/ver-adiantamento']);
  }
}
