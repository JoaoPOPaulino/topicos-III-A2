import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-ver-adiantamento',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './ver-adiantamento.html',
  styleUrl: './ver-adiantamento.css',
})
export class VerAdiantamento implements OnInit {
  sidebarOpen = false;
  profileMenuOpen = false;

  dados = {
    id: 1001,
    solicitante: 'Lucas Henderson',
    email: 'lucas@empresa.com',
    departamento: 'TI',
    descricao: 'Viagem para reunião estratégica em Brasília com clientes e parceiros',
    data: '2025-01-30',
    categoria: 'Transporte',
    valor: 'R$ 850,00',
    moeda: 'BRL',
    ptax: '5.12',
    status: 'Pendente',
    justificativa: 'Reunião presencial necessária para fechamento de contrato importante',
    anexos: [
      { nome: 'Recibo_Passagem.pdf', tamanho: '245 KB', tipo: 'pdf' },
      { nome: 'Nota_Hotel.png', tamanho: '1.2 MB', tipo: 'image' },
    ],
    historico: [
      {
        data: '31/01/2025 14:30',
        acao: 'Adiantamento criado',
        usuario: 'Lucas Henderson',
        tipo: 'create',
      },
      {
        data: '01/02/2025 09:15',
        acao: 'Enviado para revisão',
        usuario: 'Lucas Henderson',
        tipo: 'review',
      },
      {
        data: '02/02/2025 16:45',
        acao: 'Aprovado pelo gestor',
        usuario: 'Ana Costa',
        tipo: 'approved',
      },
      {
        data: '05/02/2025 10:20',
        acao: 'Pagamento realizado',
        usuario: 'Financeiro',
        tipo: 'paid',
      },
    ],
  };

  constructor(private router: Router, private location: Location) {}

  ngOnInit(): void {}

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  toggleProfileMenu(event: MouseEvent) {
    event.stopPropagation();
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  @HostListener('document:click')
  closeProfileMenu() {
    this.profileMenuOpen = false;
  }

  editar() {
    this.router.navigate(['/editar-adiantamento'], {
      queryParams: { id: this.dados.id },
    });
  }

  voltar(): void {
    this.location.back();
  }

  downloadAnexo(anexo: any) {
    console.log('Download:', anexo.nome);
  }

  getStatusClass(): string {
    return this.dados.status.replace(/\s+/g, '');
  }

  getHistoricoIcon(tipo: string): string {
    return tipo;
  }
}
