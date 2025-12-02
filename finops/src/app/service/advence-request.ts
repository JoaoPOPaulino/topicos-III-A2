// src/app/services/advance-request.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';

interface AdvanceRequestCreateDto {
  colaboradorId: number;
  departamentoId: number;
  moedaId: number;
  valor: number;
  justificativa: string;
  dataPagamentoRequerida: string;
  observacoes: string | null;
}

interface AdvanceRequestListDto {
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

interface AdvanceRequestDetailDto extends AdvanceRequestListDto {
  // ✨ ADICIONADO: IDs para edição
  colaboradorId: number;
  departamentoId: number;
  moedaId: number;
  
  justificativaCompleta: string;
  departamentoNome: string;
  dataPagamentoRequerida: string;
  dataPagamentoAjustada: string | null;
  observacoes: string | null;
  criadoPorNome: string;
  anexos: string[];
}

@Injectable({ providedIn: 'root' })
export class AdvanceRequestService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7244/api/AdvanceRequests';

  createAdvanceRequest(dto: AdvanceRequestCreateDto): Observable<any> {
    return this.http.post(this.baseUrl, dto);
  }

  getAdvanceRequestById(id: number): Observable<AdvanceRequestDetailDto> {
    return this.http.get<AdvanceRequestDetailDto>(`${this.baseUrl}/${id}`);
  }

  getAdvanceRequests(params?: any): Observable<AdvanceRequestListDto[]> {
    let httpParams = new HttpParams();

    if (params) {
      for (const key in params) {
        const value = params[key];
        if (value !== null && value !== undefined && value !== '') {
          httpParams = httpParams.set(key, value.toString());
        }
      }
    }

    return this.http.get<AdvanceRequestListDto[]>(this.baseUrl, { params: httpParams });
  }

  updateAdvanceRequest(id: number, dto: AdvanceRequestCreateDto): Observable<void> {
    return this.http.put<void>(`${this.baseUrl}/${id}`, dto);
  }
}