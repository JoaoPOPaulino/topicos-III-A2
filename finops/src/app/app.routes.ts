import { Routes } from '@angular/router';
import { AuthLayout } from './layouts/auth-layout/auth-layout';
import { MainLayout } from './layouts/main-layout/main-layout';
import { Adiantamentos } from './pages/adiantamentos/adiantamentos';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },

  // Rotas de Autenticação
  {
    path: '',
    component: AuthLayout,
    children: [
      {
        path: 'login',
        loadComponent: () => import('./pages/login/login').then((m) => m.Login),
      },
    ],
  },

  // Rotas Principais
  {
    path: '',
    component: MainLayout,
    children: [
      {
        path: 'dashboard',
        loadComponent: () => import('./pages/dashboard/dashboard').then((m) => m.Dashboard),
      },
      {
        path: 'adiantamentos',
        component: Adiantamentos, // SEM LAZY LOADING
      },
      {
        path: 'novo-adiantamento',
        loadComponent: () =>
          import('./pages/novo-adiantamento/novo-adiantamento').then((m) => m.NovoAdiantamento),
      },
      {
        path: 'ver-adiantamento',
        loadComponent: () =>
          import('./pages/ver-adiantamento/ver-adiantamento').then((m) => m.VerAdiantamento),
      },
      {
        path: 'editar-adiantamento',
        loadComponent: () =>
          import('./pages/editar-adiantamento/editar-adiantamento').then(
            (m) => m.EditarAdiantamento
          ),
      },
      {
        path: 'perfil',
        loadComponent: () => import('./pages/perfil/perfil').then((m) => m.Perfil),
      },
      {
        path: 'aprovacoes',
        loadComponent: () => import('./pages/aprovacoes/aprovacoes').then((m) => m.Aprovacoes),
      },
      {
        path: 'conversor',
        loadComponent: () => import('./pages/conversor/conversor').then((m) => m.Conversor),
      },
      {
        path: 'feriados',
        loadComponent: () => import('./pages/feriados/feriados').then((m) => m.Feriados),
      },
      {
        path: 'configuracoes',
        loadComponent: () =>
          import('./pages/configuracoes/configuracoes').then((m) => m.Configuracoes),
      },
    ],
  },

  { path: '**', redirectTo: 'login' },
];
