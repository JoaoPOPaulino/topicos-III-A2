// src/app/services/data.service.ts
import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface LookupItem {
  id: number;
  nomeCompleto?: string;
  nome?: string;
  codigo?: string;
  simbolo?: string;
}

@Injectable({ providedIn: 'root' })
export class DataService {
  private http = inject(HttpClient);
  private baseUrl = 'https://localhost:7244/api/Data';

  getUsers(): Observable<LookupItem[]> {
    return this.http.get<LookupItem[]>(`${this.baseUrl}/Users`);
  }

  getCurrencies(): Observable<LookupItem[]> {
    return this.http.get<LookupItem[]>(`${this.baseUrl}/Currencies`);
  }

  getDepartments(): Observable<LookupItem[]> {
    return this.http.get<LookupItem[]>(`${this.baseUrl}/Departments`);
  }

  createUser(data: { nomeCompleto: string }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Users`, data);
  }

  createDepartment(data: { nome: string }): Observable<any> {
    return this.http.post<any>(`${this.baseUrl}/Departments`, data);
  }
}
