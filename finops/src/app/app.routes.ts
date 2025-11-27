import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Adiantamentos } from './pages/adiantamentos/adiantamentos';
import { NovoAdiantamento } from './pages/novo-adiantamento/novo-adiantamento';
import { VerAdiantamento } from './pages/ver-adiantamento/ver-adiantamento';
import { Perfil } from './pages/perfil/perfil';
import { Login } from './pages/login/login';
import { EditarAdiantamento } from './pages/editar-adiantamento/editar-adiantamento';
import { Aprovacoes } from './pages/aprovacoes/aprovacoes';
import { Conversor } from './pages/conversor/conversor';
import { Feriados } from './pages/feriados/feriados';
import { Configuracoes } from './pages/configuracoes/configuracoes';
import { AuthLayout } from './layouts/auth-layout/auth-layout';
import { MainLayout } from './layouts/main-layout/main-layout';

export const routes: Routes = [
      {
    path: '',
    component: AuthLayout,
    children: [
      {
        path: 'login',
        loadComponent: () =>
          import('./pages/login/login').then(m => m.Login)
      }
    ]
  },

    {
    path: '',
    component: MainLayout,
    children: [
      {
        path: 'dashboard',
        loadComponent: () =>
          import('./pages/dashboard/dashboard').then(m => m.Dashboard)
      },
      {
        path: 'adiantamentos',
        loadComponent: () =>
          import('./pages/adiantamentos/adiantamentos').then(m => m.Adiantamentos)
      },
      {
        path: 'novo-adiantamento',
        loadComponent: () =>
          import('./pages/novo-adiantamento/novo-adiantamento').then(m => m.NovoAdiantamento)
      },
       {
        path: 'ver-adiantamento',
        loadComponent: () =>
          import('./pages/ver-adiantamento/ver-adiantamento').then(m => m.VerAdiantamento)
      },
      {
        path: 'perfil',
        loadComponent: () =>
          import('./pages/perfil/perfil').then(m => m.Perfil)
      },
       {
        path: 'editar-adiantamento',
        loadComponent: () =>
          import('./pages/editar-adiantamento/editar-adiantamento').then(m => m.EditarAdiantamento)
      },
      {
        path: 'aprovacoes',
        loadComponent: () =>
          import('./pages/aprovacoes/aprovacoes').then(m => m.Aprovacoes)
      },
      {
        path: 'conversor',
        loadComponent: () =>
          import('./pages/conversor/conversor').then(m => m.Conversor)
      },
      {
        path: 'feriados',
        loadComponent: () =>
          import('./pages/feriados/feriados').then(m => m.Feriados)
      },
      {
        path: 'configuracoes',
        loadComponent: () =>
          import('./pages/configuracoes/configuracoes').then(m => m.Configuracoes)
      },
    ]
  },

  // Redireciona para login ao abrir
  { path: '**', redirectTo: 'login' },
];