import { Component, OnInit, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router } from '@angular/router';

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
    solicitante: "Lucas Henderson",
    descricao: "Viagem Brasília",
    data: "2025-01-30",
    categoria: "Transporte",
    valor: "R$ 850,00",
    moeda: "BRL",
    ptax: "5.12",
    status: "pendente",
    anexos: ["Recibo1.pdf", "NotaHotel.png"]
  };

  constructor(private router: Router) {}

  ngOnInit(): void {}

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  toggleProfileMenu(event: MouseEvent) {
    event.stopPropagation();
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  @HostListener("document:click")
  closeProfileMenu() {
    this.profileMenuOpen = false;
  }

  editar() {
    this.router.navigate(['/editar-adiantamento'], {
      queryParams: { id: this.dados.id }
    });
  }

  voltar() {
    this.router.navigate(['/adiantamentos']);
  }
}
