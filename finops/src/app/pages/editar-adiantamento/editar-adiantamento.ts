import { Component, inject, OnInit } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { AdvanceRequestService } from '../../service/advence-request';
import { DataService } from '../../service/data';

interface AdvanceRequestUpdateDto {
  colaboradorId: number;
  departamentoId: number;
  moedaId: number;
  valor: number; 
  justificativa: string;
  dataPagamentoRequerida: string;
  observacoes: string | null;
}

interface StatusOption {
  value: number;
  label: string;
}

interface AdvanceRequestDetailsDto {
  colaboradorId: number;
  departamentoId: number;
  moedaId: number;
  valorFormatado: string; 
  justificativaCompleta: string;
  descricao: string;
  dataPagamentoRequerida: string;
  observacoes: string | null;
  statusDescricao: string;
  status: number;
}

@Component({
  selector: 'app-editar-adiantamento',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule, HttpClientModule],
  templateUrl: './editar-adiantamento.html',
  styleUrls: ['./editar-adiantamento.css'],
})
export class EditarAdiantamento implements OnInit {
  private location = inject(Location);
  private fb = inject(FormBuilder);
  private route = inject(ActivatedRoute); 
  private router = inject(Router);
  private advanceService = inject(AdvanceRequestService);
  private dataService = inject(DataService);

  loading = false;
  showSuccessAlert = false;
  selectedFileName = '';
  
  moedas: any[] = []; 
  colaboradores: any[] = [];
  departamentos: any[] = [];
  
  categorias = [
    { value: 'transporte', label: 'Transporte' },
    { value: 'alimentacao', label: 'Alimentação' },
    { value: 'hospedagem', label: 'Hospedagem' },
    { value: 'material', label: 'Material de Escritório' },
    { value: 'outros', label: 'Outros' },
  ];
  
  // ✅ CORRETO: Sincronizado com o enum do C#
  statusOptions: StatusOption[] = [
    { value: 1, label: 'Pendente' },              // Pendente = 1
    { value: 2, label: 'Em Revisão' },            // Revisao = 2
    { value: 3, label: 'Aprovado' },              // Aprovado = 3
    { value: 4, label: 'Atrasado' },              // Atrasado = 4
    { value: 5, label: 'Rejeitado' },             // Rejeitado = 5
    { value: 6, label: 'Pago' },                  // Pago = 6
    { value: 7, label: 'Prestação Enviada' },     // PrestacaoEnviada = 7
    { value: 8, label: 'Finalizado' },            // Finalizado = 8
  ];

  adiantamentoId: number | null = null;
  originalStatus: number | null = null; 

  adiantamentoForm = this.fb.group({
    colaboradorId: [null as number | null, Validators.required],
    departamentoId: [null as number | null, Validators.required],
    justificativa: ['', Validators.required],
    valor: ['', Validators.required],
    moedaId: [null as number | null, Validators.required], 
    dataPagamentoRequerida: ['', Validators.required],
    observacoes: [''],
    categoria: ['transporte', Validators.required], 
    status: [1 as number, Validators.required], 
  });
  
  ngOnInit(): void {
    this.route.queryParams.subscribe(params => {
      const id = params['id'];
      if (id) {
        this.adiantamentoId = Number(id);
        this.loadLookupDataThenDetails();
      } else {
        alert('ID do adiantamento não fornecido.');
        this.voltar();
      }
    });
  }

  loadLookupDataThenDetails(): void {
    let lookupsLoaded = 0;
    const totalLookups = 3;

    const checkAllLoaded = () => {
      lookupsLoaded++;
      if (lookupsLoaded === totalLookups && this.adiantamentoId) {
        this.loadAdiantamentoDetails(this.adiantamentoId);
      }
    };

    this.dataService.getCurrencies().subscribe({
      next: (data) => { this.moedas = data; checkAllLoaded(); },
      error: (err) => { console.error('❌ Erro ao carregar moedas:', err); checkAllLoaded(); }
    });
    
    this.dataService.getUsers().subscribe({
      next: (data) => { this.colaboradores = data; checkAllLoaded(); },
      error: (err) => { console.error('❌ Erro ao carregar colaboradores:', err); checkAllLoaded(); }
    });
    
    this.dataService.getDepartments().subscribe({
      next: (data) => { this.departamentos = data; checkAllLoaded(); },
      error: (err) => { console.error('❌ Erro ao carregar departamentos:', err); checkAllLoaded(); }
    });
  }

  loadAdiantamentoDetails(id: number): void {
    this.loading = true;
    
    this.advanceService.getAdvanceRequestById(id).subscribe({
      next: (data: AdvanceRequestDetailsDto) => {
        let valorLimpo = '';
        if (data.valorFormatado) {
          valorLimpo = data.valorFormatado
            .replace(/[^\d,]/g, '')
            .replace(/\./g, '');
        }
        
        const colaboradorIdNum = data.colaboradorId ? Number(data.colaboradorId) : null;
        const departamentoIdNum = data.departamentoId ? Number(data.departamentoId) : null;
        const moedaIdNum = data.moedaId ? Number(data.moedaId) : null;
        
        const dataPagamento = data.dataPagamentoRequerida 
          ? data.dataPagamentoRequerida.split('T')[0] 
          : '';

        let statusLoadedNumber = 1;
        
        if (data.status) {
          statusLoadedNumber = data.status;
        } else if (data.statusDescricao) {
          const statusMatch = this.statusOptions.find(opt => 
            opt.label.toLowerCase().replace(/\s/g, '') === data.statusDescricao.toLowerCase().replace(/\s/g, '')
          );
          statusLoadedNumber = statusMatch ? statusMatch.value : 1;
        }

        this.originalStatus = statusLoadedNumber;
        
        this.adiantamentoForm.patchValue({
          colaboradorId: colaboradorIdNum,
          departamentoId: departamentoIdNum,
          moedaId: moedaIdNum,
          valor: valorLimpo,
          dataPagamentoRequerida: dataPagamento,
          justificativa: data.justificativaCompleta || data.descricao || '',
          observacoes: data.observacoes || '',
          status: statusLoadedNumber,
          categoria: 'transporte',
        });

        this.loading = false;
      },
      error: (err) => {
        console.error('❌ ERRO ao carregar adiantamento:', err);
        this.loading = false;
        alert(`Erro ao carregar adiantamento: ${err.status || 'Desconhecido'}`);
        this.voltar();
      }
    });
  }

  normalizeValor(mascarado: string): number {
    const limpo = mascarado.replace(/\./g, '').replace(',', '.');
    return parseFloat(limpo) || 0;
  }

  onValorInput(event: Event): void {
    const input = event.target as HTMLInputElement;
    let v = input.value.replace(/\D/g, '');
    
    if (v === '') {
      input.value = '';
      this.adiantamentoForm.patchValue({ valor: '' });
      return;
    }
    
    const val = (Number(v) / 100).toFixed(2);
    const maskedVal = val.replace('.', ',').replace(/\B(?=(\d{3})+(?!\d))/g, '.');
    input.value = maskedVal;
    this.adiantamentoForm.patchValue({ valor: maskedVal });
  }

  onFileChange(event: any): void {
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

  submit(): void {
    if (this.adiantamentoForm.invalid || this.adiantamentoId === null) {
      this.adiantamentoForm.markAllAsTouched();
      alert('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    this.loading = true;
    const formValue = this.adiantamentoForm.value;
    const observacoesValor = formValue.observacoes === '' ? null : (formValue.observacoes || null);
    
    const dto: AdvanceRequestUpdateDto = {
      colaboradorId: formValue.colaboradorId!,
      departamentoId: formValue.departamentoId!,
      moedaId: formValue.moedaId!,
      valor: this.normalizeValor(formValue.valor!),
      justificativa: formValue.justificativa!,
      dataPagamentoRequerida: formValue.dataPagamentoRequerida!,
      observacoes: observacoesValor,
    };
    
    this.advanceService.updateAdvanceRequest(this.adiantamentoId, dto).subscribe({
      next: () => {
        const newStatusNumber = formValue.status!;

        if (newStatusNumber !== this.originalStatus) { 
          this.advanceService.changeStatus(this.adiantamentoId!, newStatusNumber).subscribe({
            next: () => this.finalizeSubmit(),
            error: (err) => this.handleError(err, 'Falha ao atualizar o status.')
          });
        } else {
          this.finalizeSubmit();
        }
      },
      error: (err) => this.handleError(err, 'Falha ao atualizar dados.')
    });
  }

  private finalizeSubmit(): void {
    this.loading = false;
    this.showSuccessAlert = true;
    setTimeout(() => {
      this.showSuccessAlert = false;
      this.voltar();
    }, 2000);
  }

  private handleError(err: any, defaultMsg: string): void {
    this.loading = false;
    
    let msg = defaultMsg;
    
    if (err.status === 400 && err.error && typeof err.error === 'object') {
      const modelStateErrors = Object.values(err.error)
        .flatMap((x: any) => x)
        .filter((item: any) => typeof item === 'string' && item.length > 0);
      
      if (modelStateErrors.length > 0) {
        msg = `Falha de Validação: ${modelStateErrors.join('; ')}`;
      } else {
        msg = err.error?.message || msg;
      }
    } else {
      msg = err.error?.message || err.message || msg;
    }
    
    alert(`${msg} (Status: ${err.status || 'Desconhecido'})`);
  }

  voltar(): void {
    this.location.back();
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.adiantamentoForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }
}