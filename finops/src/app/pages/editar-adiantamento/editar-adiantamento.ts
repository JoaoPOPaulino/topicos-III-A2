import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { Location } from '@angular/common';

@Component({
  selector: 'app-editar-adiantamento',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './editar-adiantamento.html',
  styleUrl: './editar-adiantamento.css',
})
export class EditarAdiantamento {

  constructor(private location: Location) {}

  private fb = inject(FormBuilder);

  sidebarOpen = false;
  profileMenuOpen = false;

  adiantamentoForm = this.fb.group({
    solicitante: ['', Validators.required],
    descricao: ['', Validators.required],
    valor: ['', Validators.required],
    moeda: ['BRL', Validators.required],
    data: ['', Validators.required],
    categoria: ['transporte', Validators.required],
    status: ['Pendente', Validators.required],
    anexo: [null]
  });

  // ===== SIDEBAR MOBILE =====
  toggleMenu() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  // ===== DROPDOWN PERFIL =====
  toggleProfileMenu(event: MouseEvent) {
    event.stopPropagation();
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  closeProfileMenu() {
    this.profileMenuOpen = false;
  }

  // ===== MÁSCARA DE VALOR =====
  onValorInput(event: Event) {
    const input = event.target as HTMLInputElement;
    let v = input.value.replace(/\D/g, '');
    v = (Number(v) / 100).toFixed(2);
    v = v.replace('.', ',');
    input.value = 'R$ ' + v;
    this.adiantamentoForm.patchValue({ valor: input.value });
  }

  // ===== SUBMIT =====
  submit() {
    if (this.adiantamentoForm.invalid) {
      this.adiantamentoForm.markAllAsTouched();
      return;
    }

    console.log('ADIANTAMENTO ATUALIZADO:', this.adiantamentoForm.value);
    alert('Adiantamento salvo com sucesso!');
  }

  voltar(): void {
    this.location.back();
  }
}
