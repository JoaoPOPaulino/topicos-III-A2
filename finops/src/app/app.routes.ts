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

export const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' }, // Redireciona raiz para dashboard
    { path: 'dashboard', component: Dashboard },
    { path: 'adiantamentos', component: Adiantamentos },
    { path: 'novo-adiantamento', component: NovoAdiantamento },
    { path: 'ver-adiantamento', component: VerAdiantamento },
    { path: 'perfil', component: Perfil },
    { path: 'login', component: Login },
    { path: 'editar-adiantamento', component: EditarAdiantamento },
    { path: 'aprovacoes', component: Aprovacoes },
    { path: 'conversor', component: Conversor },
    { path: 'feriados', component: Feriados },
];