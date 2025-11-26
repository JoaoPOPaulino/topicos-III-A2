import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';

@Component({
  selector: 'app-novo-adiantamento',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './novo-adiantamento.html',
  styleUrl: './novo-adiantamento.css',
})
export class NovoAdiantamento {
  constructor(private router: Router) {}

  sidebarOpen = false;
  profileMenuOpen = false;

  form = {
    solicitante: '',
    descricao: '',
    valor: '',
    moeda: 'BRL',
    data: '',
    categoria: 'transporte',
    anexos: [] as File[]
  };

  toggleSidebar() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  toggleProfileMenu() {
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  selecionarArquivos(event: any) {
    this.form.anexos = Array.from(event.target.files);
  }

  mascaraValor(event: any) {
    let v = event.target.value.replace(/\D/g, '');
    v = (v / 100).toFixed(2) + '';
    v = v.replace('.', ',');
    event.target.value = 'R$ ' + v;
    this.form.valor = event.target.value;
  }

  salvar() {
    alert('Adiantamento salvo com sucesso!');
    this.router.navigate(['/adiantamentos']);
  }
}
