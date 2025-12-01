import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, Router } from '@angular/router';
import { Location } from '@angular/common';
import { DataService } from '../../service/data';
import { AdvanceRequestService } from '../../service/advence-request';

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

  colaboradores: any[] = [];
  moedas: any[] = [];
  departamentos: any[] = [];

  sidebarOpen = false;
  profileMenuOpen = false;

 form = {
    colaboradorId: 0,
    departamentoId: 0,
    moedaId: 1,
    justificativa: '',
    valor: 0.00,
    valorMascarado: '',
    dataPagamentoRequerida: '',
    observacoes: '',
  };

  ngOnInit(): void {
      this.loadLookupData();
  }

 loadLookupData() {
    this.dataService.getUsers().subscribe(data => this.colaboradores = data);
    this.dataService.getCurrencies().subscribe(data => this.moedas = data);
    this.dataService.getDepartments().subscribe(data => this.departamentos = data);
  }

  normalizeValor(mascarado: string): number {
    // Remove "R$", "." e substitui "," por "."
    return parseFloat(mascarado.replace(/[^\d,]/g, '').replace(',', '.'));
  }

  mascaraValor(event: any) {
    let v = event.target.value.replace(/\D/g, '');
    v = (Number(v) / 100).toFixed(2) + '';
    v = v.replace('.', ',');
    this.form.valorMascarado = 'R$ ' + v;
    event.target.value = this.form.valorMascarado;
  }

  salvar() {
    const valorLimpo = this.normalizeValor(this.form.valorMascarado);

    const dto = {
        colaboradorId: this.form.colaboradorId,
        departamentoId: this.form.departamentoId,
        moedaId: this.form.moedaId,
        valor: valorLimpo,
        justificativa: this.form.justificativa,
        dataPagamentoRequerida: this.form.dataPagamentoRequerida,
        observacoes: this.form.observacoes,
    };

    // 2. Chama o serviço do back-end
    this.advanceService.createAdvanceRequest(dto as any).subscribe({
        next: (response) => {
            alert(`Adiantamento #${response.id} criado com sucesso!`);
            this.router.navigate(['/adiantamentos']);
        },
        error: (err) => {
            console.error('Erro ao salvar:', err);
            const msg = err.error?.message || 'Erro ao comunicar com o servidor.';
            alert(`Falha ao criar adiantamento: ${msg}`);
        }
    });
  }

  voltar(): void {
    this.location.back();
  }
}