import { Component, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';

type CamposSenha = {
  senhaAtual: boolean;
  novaSenha: boolean;
  confirmarSenha: boolean;
};

@Component({
  selector: 'app-perfil',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './perfil.html',
  styleUrl: './perfil.css',
})
export class Perfil {
 
  sidebarOpen = false;
  profileMenuOpen = false;

  user = {
    nome: 'Lucas Henderson',
    email: 'lucas@empresa.com',
    role: 'Analista de Sistemas',
    department: 'TI'
  };

  senhaAtual = '';
  novaSenha = '';
  confirmarSenha = '';

  // AGORA SEM TS4111
  camposVisiveis: CamposSenha = {
    senhaAtual: false,
    novaSenha: false,
    confirmarSenha: false
  };

  constructor(private router: Router) {}

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

  togglePassword(field: keyof CamposSenha) {
    this.camposVisiveis[field] = !this.camposVisiveis[field];
  }

  salvar() {
    if (this.senhaAtual || this.novaSenha || this.confirmarSenha) {
      if (!this.senhaAtual) {
        alert('Informe a senha atual.');
        return;
      }
      if (this.novaSenha.length < 4) {
        alert('A nova senha deve ter ao menos 4 caracteres.');
        return;
      }
      if (this.novaSenha !== this.confirmarSenha) {
        alert('A nova senha e a confirmação não coincidem.');
        return;
      }
      alert('Senha alterada com sucesso (simulado).');
    }

    alert('Dados do perfil atualizados (simulado).');
  }
}
