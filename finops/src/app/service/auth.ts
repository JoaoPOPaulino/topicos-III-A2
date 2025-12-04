// src/app/services/auth.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';

export interface LoginCredentials {
  email: string;
  senha: string;
}

export interface UserData {
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
  private userKey = 'userData';

  constructor(private http: HttpClient) {}

  login(credentials: LoginCredentials): Observable<UserData> {
    const url = `${this.apiUrl}/login`;

    return this.http.post<UserData>(url, credentials).pipe(
      tap((user) => {
        // Armazena o usuário localmente
        localStorage.setItem(this.userKey, JSON.stringify(user));
      })
    );
  }

  getUser(): UserData | null {
    const data = localStorage.getItem(this.userKey);
    return data ? JSON.parse(data) : null;
  }

  logout() {
    localStorage.removeItem(this.userKey);
  }
}
