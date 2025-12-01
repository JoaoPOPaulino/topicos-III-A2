// src/app/services/advance-request.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http'; // Adicionado HttpParams
import { Observable } from 'rxjs';

// DTOs (Mantidos)
interface AdvanceRequestCreateDto {
  colaboradorId: number;
  departamentoId: number;
  moedaId: number;
  valor: number;
  justificativa: string;
  dataPagamentoRequerida: string;
  observacoes: string | null;
}

interface AdvanceRequestDetailDto {
  id: number;
  solicitanteNome: string;
  descricao: string;
  valor: number;
  moedaCodigo: string;
  valorFormatado: string;
  dataCriacao: string;
  status: number;
  statusDescricao: string;
}

@Injectable({ providedIn: 'root' })
export class AdvanceRequestService {
  private http = inject(HttpClient);
  // Usamos HTTPS na porta 7244, pois o Login confirmou ser o protocolo correto
  private baseUrl = 'https://localhost:7244/api/AdvanceRequests'; 

  // POST: Criar Novo Adiantamento
  createAdvanceRequest(dto: AdvanceRequestCreateDto): Observable<any> {
    return this.http.post(this.baseUrl, dto);
  }

  // GET: Obter Detalhes por ID
  getAdvanceRequestById(id: number): Observable<AdvanceRequestDetailDto> {
    return this.http.get<AdvanceRequestDetailDto>(`${this.baseUrl}/${id}`);
  }
  
  // GET: Obter Listagem (Corrigido para Limpeza de Parâmetros)
  getAdvanceRequests(params?: any): Observable<AdvanceRequestDetailDto[]> {
    let httpParams = new HttpParams();

    // Itera sobre os filtros e adiciona apenas valores válidos (não nulos/vazios)
    if (params) {
      for (const key in params) {
        const value = params[key];
        // Exclui 'undefined', null, ou string vazia
        if (value !== null && value !== undefined && value !== '') {
          httpParams = httpParams.set(key, value.toString());
        }
      }
    }

    return this.http.get<AdvanceRequestDetailDto[]>(this.baseUrl, { params: httpParams });
  }
}