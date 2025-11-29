// src/app/pages/login/login.ts (Modificado)
import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { HttpClientModule } from '@angular/common/http';
import { AuthService } from '../../service/auth';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, HttpClientModule],
  templateUrl: './login.html',
  styleUrl: './login.css',
})
export class Login {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private router = inject(Router);

  showPassword = false;
  loginError = '';

  loginForm = this.fb.group({
    email: ['', [Validators.required, Validators.email]],
    password: ['', [Validators.required, Validators.minLength(4)]],
  });

  togglePassword() {
    this.showPassword = !this.showPassword;
  }

  submit() {
    this.loginError = '';
    if (this.loginForm.invalid) {
      this.loginForm.markAllAsTouched();
      return;
    }

    const credentials = {
      email: this.loginForm.value.email!,
      senha: this.loginForm.value.password!,
    };

    this.authService.login(credentials).subscribe({
      next: (userData) => {
        console.log('Login Sucedido:', userData);
        localStorage.setItem('user', JSON.stringify(userData));
        this.router.navigate(['/dashboard']);
      },
      error: (err) => {
        console.error('Erro de Login:', err);
        this.loginError = 'Email ou senha inválidos. Tente novamente.';
      },
    });
  }

  get email() {
    return this.loginForm.get('email');
  }

  get password() {
    return this.loginForm.get('password');
  }
}
