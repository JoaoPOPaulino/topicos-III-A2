import { Component, inject, OnInit } from '@angular/core';
import { CommonModule, Location } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { AdvanceRequestService } from '../../service/advence-request';
import { DataService } from '../../service/data';

interface AdvanceRequestCreateDto {
  colaboradorId: number;
  departamentoId: number;
  moedaId: number;
  valor: number; 
  justificativa: string;
  dataPagamentoRequerida: string;
  observacoes: string | null;
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
  
  statusOptions = [
    { value: 'Pendente', label: 'Pendente' },
    { value: 'Revisão', label: 'Revisão' },
    { value: 'Aprovado', label: 'Aprovado' },
    { value: 'Atrasado', label: 'Atrasado' },
    { value: 'Rejeitado', label: 'Rejeitado' },
    { value: 'Pago', label: 'Pago' },
    { value: 'PrestacaoEnviada', label: 'Prestação Enviada' }, 
    { value: 'Finalizado', label: 'Finalizado' },
  ];

  adiantamentoId: number | null = null;

  adiantamentoForm = this.fb.group({
    colaboradorId: [null as number | null, Validators.required],
    departamentoId: [null as number | null, Validators.required],
    justificativa: ['', Validators.required],
    valor: ['', Validators.required],
    moedaId: [null as number | null, Validators.required], 
    dataPagamentoRequerida: ['', Validators.required],
    observacoes: [''],
    categoria: ['transporte', Validators.required], 
    status: ['Pendente'],
  });
  
  ngOnInit(): void {
    console.log('🚀 EditarAdiantamento - ngOnInit chamado');
    
    this.route.queryParams.subscribe(params => {
      const id = params['id'];
      console.log('📋 Query Params:', params);
      console.log('🆔 ID recebido:', id);
      
      if (id) {
        this.adiantamentoId = Number(id);
        console.log('🔢 Adiantamento ID convertido:', this.adiantamentoId);
        
        // Carrega lookups PRIMEIRO, depois carrega os detalhes
        this.loadLookupDataThenDetails();
      } else {
        alert('ID do adiantamento não fornecido.');
        this.voltar();
      }
    });
  }

  loadLookupDataThenDetails(): void {
    console.log('📦 Iniciando carregamento de lookups...');
    
    let lookupsLoaded = 0;
    const totalLookups = 3;

    const checkAllLoaded = () => {
      lookupsLoaded++;
      console.log(`✅ Lookup carregado (${lookupsLoaded}/${totalLookups})`);
      
      if (lookupsLoaded === totalLookups) {
        console.log('🎉 Todos os lookups carregados! Agora carregando detalhes...');
        this.loadAdiantamentoDetails(this.adiantamentoId!);
      }
    };

    // Carrega moedas
    this.dataService.getCurrencies().subscribe({
      next: (data) => {
        this.moedas = data;
        console.log('💰 Moedas carregadas:', this.moedas);
        checkAllLoaded();
      },
      error: (err) => {
        console.error('❌ Erro ao carregar moedas:', err);
        checkAllLoaded(); // Continua mesmo com erro
      }
    });
    
    // Carrega colaboradores
    this.dataService.getUsers().subscribe({
      next: (data) => {
        this.colaboradores = data;
        console.log('👥 Colaboradores carregados:', this.colaboradores);
        checkAllLoaded();
      },
      error: (err) => {
        console.error('❌ Erro ao carregar colaboradores:', err);
        checkAllLoaded();
      }
    });
    
    // Carrega departamentos
    this.dataService.getDepartments().subscribe({
      next: (data) => {
        this.departamentos = data;
        console.log('🏢 Departamentos carregados:', this.departamentos);
        checkAllLoaded();
      },
      error: (err) => {
        console.error('❌ Erro ao carregar departamentos:', err);
        checkAllLoaded();
      }
    });
  }

  loadAdiantamentoDetails(id: number): void {
    console.log(`🔍 Iniciando carregamento dos detalhes do adiantamento ID ${id}...`);
    this.loading = true;
    
    this.advanceService.getAdvanceRequestById(id).subscribe({
      next: (data: any) => {
        console.log('📄 Dados COMPLETOS recebidos do backend:', JSON.stringify(data, null, 2));
        
        // ✅ VERIFICAÇÃO CRÍTICA - Imprime cada campo individualmente
        console.log('🔎 Verificação individual dos campos:');
        console.log('  - colaboradorId:', data.colaboradorId, '(tipo:', typeof data.colaboradorId, ')');
        console.log('  - departamentoId:', data.departamentoId, '(tipo:', typeof data.departamentoId, ')');
        console.log('  - moedaId:', data.moedaId, '(tipo:', typeof data.moedaId, ')');
        console.log('  - valorFormatado:', data.valorFormatado);
        console.log('  - dataPagamentoRequerida:', data.dataPagamentoRequerida);
        console.log('  - justificativaCompleta:', data.justificativaCompleta);
        console.log('  - observacoes:', data.observacoes);
        console.log('  - statusDescricao:', data.statusDescricao);
        
        // Limpa o valorFormatado
        let valorLimpo = '';
        if (data.valorFormatado) {
          valorLimpo = data.valorFormatado
            .replace(/[^\d,]/g, '')
            .replace(/\./g, '');
          console.log('💵 Valor formatado limpo:', valorLimpo);
        }
        
        // ✅ CONVERSÃO EXPLÍCITA para garantir que são números
        const colaboradorIdNum = data.colaboradorId ? Number(data.colaboradorId) : null;
        const departamentoIdNum = data.departamentoId ? Number(data.departamentoId) : null;
        const moedaIdNum = data.moedaId ? Number(data.moedaId) : null;
        
        console.log('🔢 IDs convertidos para número:');
        console.log('  - colaboradorId:', colaboradorIdNum);
        console.log('  - departamentoId:', departamentoIdNum);
        console.log('  - moedaId:', moedaIdNum);
        
        // Extrai apenas a data (YYYY-MM-DD)
        const dataPagamento = data.dataPagamentoRequerida 
          ? data.dataPagamentoRequerida.split('T')[0] 
          : '';
        console.log('📅 Data de pagamento extraída:', dataPagamento);
        
        // ✅ PREENCHE O FORMULÁRIO
        this.adiantamentoForm.patchValue({
          colaboradorId: colaboradorIdNum,
          departamentoId: departamentoIdNum,
          moedaId: moedaIdNum,
          valor: valorLimpo,
          dataPagamentoRequerida: dataPagamento,
          justificativa: data.justificativaCompleta || data.descricao || '',
          observacoes: data.observacoes || '',
          status: data.statusDescricao || 'Pendente',
          categoria: 'transporte',
        });

        console.log('✅ Formulário preenchido. Valores atuais:');
        console.log(JSON.stringify(this.adiantamentoForm.value, null, 2));
        
        console.log('🎯 Estado de validade dos campos:');
        console.log('  - colaboradorId válido?', !this.adiantamentoForm.get('colaboradorId')?.invalid);
        console.log('  - departamentoId válido?', !this.adiantamentoForm.get('departamentoId')?.invalid);
        console.log('  - moedaId válido?', !this.adiantamentoForm.get('moedaId')?.invalid);
        
        this.loading = false;
        console.log('✅ Loading finalizado com sucesso!');
      },
      error: (err) => {
        console.error('❌ ERRO ao carregar adiantamento:', err);
        console.error('📊 Status do erro:', err.status);
        console.error('📝 Mensagem do erro:', err.message);
        console.error('🔍 Detalhes completos:', JSON.stringify(err, null, 2));
        
        this.loading = false;
        alert(`Erro ao carregar adiantamento: ${err.status || 'Desconhecido'} - ${err.message || 'Sem mensagem'}`);
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
    console.log('📤 Submit chamado!');
    console.log('📋 Valores do formulário:', this.adiantamentoForm.value);
    console.log('✅ Formulário válido?', this.adiantamentoForm.valid);
    console.log('🆔 Adiantamento ID:', this.adiantamentoId);
    
    if (this.adiantamentoForm.invalid || this.adiantamentoId === null) {
      this.adiantamentoForm.markAllAsTouched();
      console.log('❌ Formulário inválido!');
      console.log('🔍 Erros por campo:');
      Object.keys(this.adiantamentoForm.controls).forEach(key => {
        const control = this.adiantamentoForm.get(key);
        if (control?.invalid) {
          console.log(`  - ${key}: INVÁLIDO`, control.errors);
        }
      });
      alert('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    const formValue = this.adiantamentoForm.value;
    const observacoesValor = formValue.observacoes === '' ? null : (formValue.observacoes || null);
    
    const dto: AdvanceRequestCreateDto = {
      colaboradorId: formValue.colaboradorId!,
      departamentoId: formValue.departamentoId!,
      moedaId: formValue.moedaId!,
      valor: this.normalizeValor(formValue.valor!),
      justificativa: formValue.justificativa!,
      dataPagamentoRequerida: formValue.dataPagamentoRequerida!,
      observacoes: observacoesValor,
    };
    
    console.log('📦 DTO montado para envio:', JSON.stringify(dto, null, 2));
    
    this.loading = true;
    this.advanceService.updateAdvanceRequest(this.adiantamentoId, dto).subscribe({
      next: () => {
        console.log(`✅ Adiantamento ID ${this.adiantamentoId} atualizado com sucesso!`);
        this.loading = false;
        this.showSuccessAlert = true;
        setTimeout(() => {
          this.showSuccessAlert = false;
          this.voltar();
        }, 2000);
      },
      error: (err) => {
        console.error('❌ Erro ao atualizar adiantamento:', err);
        this.loading = false;
        const msg = err.error?.message || err.message || 'Erro ao comunicar com o servidor.';
        alert(`Falha na atualização: ${msg}`);
      }
    });
  }

  voltar(): void {
    console.log('⬅️ Voltando...');
    this.location.back();
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.adiantamentoForm.get(fieldName);
    return !!(field && field.invalid && field.touched);
  }
}