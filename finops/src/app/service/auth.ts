// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

interface LoginCredentials {
  email: string;
  senha: string;
}

interface UserData {
  id: number;
  nomeCompleto: string;
  email: string;
  perfil: string;
  departamento: string;
}

@Injectable({
  providedIn: 'root',
})
export class AuthService {
  private apiUrl = 'https://localhost:7244/api/Auth';

  constructor(private http: HttpClient) {}

  login(credentials: LoginCredentials): Observable<UserData> {
    const url = `${this.apiUrl}/login`;

    return this.http.post<UserData>(url, credentials);
  }
}
