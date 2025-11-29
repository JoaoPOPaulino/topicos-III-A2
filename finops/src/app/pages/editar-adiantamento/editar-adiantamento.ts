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
  showSuccessAlert = false;
  selectedFileName = '';

  adiantamentoForm = this.fb.group({
    solicitante: ['Lucas Henderson', Validators.required],
    descricao: ['Viagem para reunião em Brasília', Validators.required],
    valor: ['850,00', Validators.required],
    moeda: ['BRL', Validators.required],
    data: ['2025-01-31', Validators.required],
    categoria: ['transporte', Validators.required],
    status: ['Pendente', Validators.required],
    justificativa: [''],
    anexo: [null],
  });

  categorias = [
    { value: 'transporte', label: 'Transporte', icon: 'car' },
    { value: 'alimentacao', label: 'Alimentação', icon: 'utensils' },
    { value: 'hospedagem', label: 'Hospedagem', icon: 'home' },
    { value: 'material', label: 'Material de Escritório', icon: 'package' },
    { value: 'outros', label: 'Outros', icon: 'more' },
  ];

  statusOptions = [
    { value: 'Pendente', label: 'Pendente' },
    { value: 'Revisão', label: 'Revisão' },
    { value: 'Aprovado', label: 'Aprovado' },
    { value: 'Atrasado', label: 'Atrasado' },
    { value: 'Rejeitado', label: 'Rejeitado' },
    { value: 'Pago', label: 'Pago' },
  ];

  moedas = [
    { value: 'BRL', label: 'Real Brasileiro (BRL)' },
    { value: 'USD', label: 'Dólar Americano (USD)' },
    { value: 'EUR', label: 'Euro (EUR)' },
    { value: 'GBP', label: 'Libra Esterlina (GBP)' },
  ];

  toggleMenu() {
    this.sidebarOpen = !this.sidebarOpen;
  }

  toggleProfileMenu(event: MouseEvent) {
    event.stopPropagation();
    this.profileMenuOpen = !this.profileMenuOpen;
  }

  closeProfileMenu() {
    this.profileMenuOpen = false;
  }

  onValorInput(event: Event) {
    const input = event.target as HTMLInputElement;
    let v = input.value.replace(/\D/g, '');
    if (v === '') {
      input.value = '';
      return;
    }
    v = (Number(v) / 100).toFixed(2);
    v = v.replace('.', ',');
    v = v.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
    input.value = v;
    this.adiantamentoForm.patchValue({ valor: input.value });
  }

  onFileChange(event: any) {
    const files = event.target.files;
    if (files && files.length > 0) {
      const fileNames = Array.from(files)
        .map((f: any) => f.name)
        .join(', ');
      this.selectedFileName = fileNames;
    } else {
      this.selectedFileName = '';
    }
  }

  submit() {
    if (this.adiantamentoForm.invalid) {
      this.adiantamentoForm.markAllAsTouched();
      return;
    }

    console.log('ADIANTAMENTO ATUALIZADO:', this.adiantamentoForm.value);

    this.showSuccessAlert = true;
    setTimeout(() => {
      this.showSuccessAlert = false;
      this.voltar();
    }, 2000);
  }

  voltar(): void {
    this.location.back();
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.adiantamentoForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }
}
