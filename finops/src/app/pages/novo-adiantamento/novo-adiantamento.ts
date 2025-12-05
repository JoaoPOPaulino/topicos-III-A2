import { Component, inject, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { Location } from '@angular/common';
import { DataService } from '../../service/data';
import { AdvanceRequestService } from '../../service/advence-request';

interface DateValidationResponse {
  originalDate: string;
  isHoliday: boolean;
  isWeekend: boolean;
  adjustedDate: string;
  wasAdjusted: boolean;
  message: string;
}

@Component({
  selector: 'app-novo-adiantamento',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule],
  templateUrl: './novo-adiantamento.html',
  styleUrl: './novo-adiantamento.css',
})
export class NovoAdiantamento implements OnInit {
  private router = inject(Router);
  private location = inject(Location);
  private advanceService = inject(AdvanceRequestService);
  private dataService = inject(DataService);
  private cdr = inject(ChangeDetectorRef);

  colaboradores: any[] = [];
  moedas: any[] = [];
  departamentos: any[] = [];

  sidebarOpen = false;
  profileMenuOpen = false;
  showSuccessAlert = false;
  isLoading = false;

  // ✨ Validação de Feriados
  dateWarningMessage = '';
  isValidatingDate = false;
  adjustedDate: string | null = null;
  isHolidayWarning = false; // Para estilização diferente

  showNewColaboradorInput = false;
  showNewDepartamentoInput = false;
  newColaboradorNome = '';
  newDepartamentoNome = '';

  form = {
    colaboradorId: 0,
    departamentoId: 0,
    moedaId: 1,
    justificativa: '',
    valor: 0.0,
    valorMascarado: '',
    dataPagamentoRequerida: '',
    observacoes: '',
  };

  formErrors = {
    colaboradorId: false,
    departamentoId: false,
    justificativa: false,
    valorMascarado: false,
    dataPagamentoRequerida: false,
  };

  ngOnInit(): void {
    this.loadLookupData();
  }

  loadLookupData() {
    this.dataService.getUsers().subscribe((data) => {
      this.colaboradores = data;
      this.cdr.detectChanges();
    });
    this.dataService.getCurrencies().subscribe((data) => {
      this.moedas = data;
      this.cdr.detectChanges();
    });
    this.dataService.getDepartments().subscribe((data) => {
      this.departamentos = data;
      this.cdr.detectChanges();
    });
  }

  onColaboradorChange(event: any) {
    const value = event.target.value;
    console.log('Colaborador selecionado:', value);

    if (value === 'novo') {
      this.showNewColaboradorInput = true;
      this.form.colaboradorId = 0;
      this.newColaboradorNome = '';
    } else {
      this.showNewColaboradorInput = false;
      this.form.colaboradorId = parseInt(value);
    }

    this.cdr.detectChanges();
  }

  adicionarColaborador() {
    if (!this.newColaboradorNome.trim()) {
      alert('Digite o nome do colaborador');
      return;
    }

    this.isLoading = true;
    this.dataService.createUser({ nomeCompleto: this.newColaboradorNome.trim() }).subscribe({
      next: (novoColaborador) => {
        console.log('Colaborador criado:', novoColaborador);

        this.colaboradores.push(novoColaborador);
        this.form.colaboradorId = novoColaborador.id;

        this.showNewColaboradorInput = false;
        this.newColaboradorNome = '';
        this.isLoading = false;

        this.cdr.detectChanges();

        alert('Colaborador adicionado com sucesso!');
      },
      error: (err) => {
        console.error('Erro ao adicionar colaborador:', err);
        this.isLoading = false;
        alert('Erro ao adicionar colaborador. Tente novamente.');
      },
    });
  }

  cancelarNovoColaborador() {
    this.showNewColaboradorInput = false;
    this.newColaboradorNome = '';
    this.cdr.detectChanges();
  }

  onDepartamentoChange(event: any) {
    const value = event.target.value;
    console.log('Departamento selecionado:', value);

    if (value === 'novo') {
      this.showNewDepartamentoInput = true;
      this.form.departamentoId = 0;
      this.newDepartamentoNome = '';
    } else {
      this.showNewDepartamentoInput = false;
      this.form.departamentoId = parseInt(value);
    }

    this.cdr.detectChanges();
  }

  adicionarDepartamento() {
    if (!this.newDepartamentoNome.trim()) {
      alert('Digite o nome do departamento');
      return;
    }

    this.isLoading = true;
    this.dataService.createDepartment({ nome: this.newDepartamentoNome.trim() }).subscribe({
      next: (novoDepartamento) => {
        console.log('Departamento criado:', novoDepartamento);

        this.departamentos.push(novoDepartamento);
        this.form.departamentoId = novoDepartamento.id;

        this.showNewDepartamentoInput = false;
        this.newDepartamentoNome = '';
        this.isLoading = false;

        this.cdr.detectChanges();

        alert('Departamento adicionado com sucesso!');
      },
      error: (err) => {
        console.error('Erro ao adicionar departamento:', err);
        this.isLoading = false;
        alert('Erro ao adicionar departamento. Tente novamente.');
      },
    });
  }

  cancelarNovoDepartamento() {
    this.showNewDepartamentoInput = false;
    this.newDepartamentoNome = '';
    this.cdr.detectChanges();
  }

  normalizeValor(mascarado: string): number {
    return parseFloat(mascarado.replace(/[^\d,]/g, '').replace(',', '.'));
  }

  mascaraValor(event: any) {
    let v = event.target.value.replace(/\D/g, '');
    if (v === '') {
      event.target.value = '';
      this.form.valorMascarado = '';
      return;
    }
    v = (Number(v) / 100).toFixed(2);
    v = v.replace('.', ',');
    v = v.replace(/\B(?=(\d{3})+(?!\d))/g, '.');
    this.form.valorMascarado = v;
    event.target.value = v;
  }

  // ✨ VALIDAÇÃO DE DATA COM FERIADOS
  onDateChange(event: Event): void {
    const input = event.target as HTMLInputElement;
    const selectedDate = input.value; // Formato: YYYY-MM-DD

    if (!selectedDate) {
      this.dateWarningMessage = '';
      this.adjustedDate = null;
      this.isHolidayWarning = false;
      return;
    }

    // Valida se a data é no passado
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const selected = new Date(selectedDate + 'T00:00:00');

    if (selected < today) {
      this.dateWarningMessage = '⚠️ A data não pode ser no passado.';
      this.adjustedDate = null;
      this.isHolidayWarning = false;
      this.formErrors.dataPagamentoRequerida = true;
      return;
    }

    // Limpa erro de validação local
    this.formErrors.dataPagamentoRequerida = false;

    // Chama a API para validar feriados
    this.validateDateWithApi(selectedDate);
  }

  private validateDateWithApi(date: string): void {
    this.isValidatingDate = true;
    this.dateWarningMessage = '';
    this.isHolidayWarning = false;

    console.log('📅 Validando data:', date);

    this.advanceService.validatePaymentDate(date).subscribe({
      next: (response: DateValidationResponse) => {
        console.log('📅 Resposta da validação:', response);
        this.isValidatingDate = false;

        if (response.wasAdjusted) {
          // Data foi ajustada para o próximo dia útil
          this.adjustedDate = response.adjustedDate;

          const originalDate = new Date(response.originalDate + 'T00:00:00');
          const adjusted = new Date(response.adjustedDate + 'T00:00:00');

          const originalFormatted = originalDate.toLocaleDateString('pt-BR');
          const adjustedFormatted = adjusted.toLocaleDateString('pt-BR');

          if (response.isWeekend) {
            this.dateWarningMessage = `📅 A data ${originalFormatted} cai em um fim de semana. A data será ajustada para ${adjustedFormatted} (próximo dia útil).`;
            this.isHolidayWarning = false;
          } else if (response.isHoliday) {
            this.dateWarningMessage = `🎉 A data ${originalFormatted} é um feriado nacional. A data será ajustada para ${adjustedFormatted} (próximo dia útil).`;
            this.isHolidayWarning = true;
          }

          // Atualiza o campo do formulário com a data ajustada
          this.form.dataPagamentoRequerida = response.adjustedDate;

        } else {
          // Data é válida (dia útil)
          this.dateWarningMessage = '';
          this.adjustedDate = null;
          this.isHolidayWarning = false;
        }

        this.cdr.detectChanges();
      },
      error: (err) => {
        console.error('❌ Erro ao validar data:', err);
        this.isValidatingDate = false;
        this.dateWarningMessage = '⚠️ Não foi possível validar a data. Tente novamente.';
        this.isHolidayWarning = false;
        this.cdr.detectChanges();
      },
    });
  }

  validateForm(): boolean {
    this.formErrors.colaboradorId = !this.form.colaboradorId || this.form.colaboradorId === 0;
    this.formErrors.departamentoId = !this.form.departamentoId || this.form.departamentoId === 0;
    this.formErrors.justificativa = !this.form.justificativa.trim();
    this.formErrors.valorMascarado =
      !this.form.valorMascarado || this.normalizeValor(this.form.valorMascarado) <= 0;
    this.formErrors.dataPagamentoRequerida = !this.form.dataPagamentoRequerida;

    return !Object.values(this.formErrors).some((error) => error);
  }

  salvar() {
    if (!this.validateForm()) {
      alert('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    this.isLoading = true;
    const valorLimpo = this.normalizeValor(this.form.valorMascarado);

    // ✅ Usa a data ajustada se existir, senão usa a data do form
    const finalDate = this.adjustedDate || this.form.dataPagamentoRequerida;

    const dto = {
      colaboradorId: this.form.colaboradorId,
      departamentoId: this.form.departamentoId,
      moedaId: this.form.moedaId,
      valor: valorLimpo,
      justificativa: this.form.justificativa,
      dataPagamentoRequerida: finalDate, // ✅ Data validada
      observacoes: this.form.observacoes || null,
    };

    console.log('📤 Enviando adiantamento:', dto);

    this.advanceService.createAdvanceRequest(dto as any).subscribe({
      next: (response) => {
        console.log('✅ Adiantamento criado:', response);
        this.isLoading = false;
        this.showSuccessAlert = true;

        setTimeout(() => {
          this.showSuccessAlert = false;
          this.router.navigate(['/adiantamentos']);
        }, 2000);
      },
      error: (err) => {
        this.isLoading = false;
        console.error('❌ Erro ao salvar:', err);
        const msg = err.error?.message || 'Erro ao comunicar com o servidor.';
        alert(`Falha ao criar adiantamento: ${msg}`);
      },
    });
  }

  voltar(): void {
    this.location.back();
  }
}