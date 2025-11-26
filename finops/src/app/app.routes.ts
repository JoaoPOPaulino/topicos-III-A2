import { Routes } from '@angular/router';
import { Dashboard } from './pages/dashboard/dashboard';
import { Adiantamentos } from './pages/adiantamentos/adiantamentos';

export const routes: Routes = [
    { path: '', redirectTo: 'dashboard', pathMatch: 'full' }, // Redireciona raiz para dashboard
    { path: 'dashboard', component: Dashboard },
    { path: 'adiantamentos', component: Adiantamentos },
];