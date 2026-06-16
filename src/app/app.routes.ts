import { Routes } from '@angular/router';
import { authGuard } from './providers/auth.provider';
import { AuthPageComponent } from '@pages/auth/auth-page.component';

export const routes: Routes = [
  {
    path: 'auth',
    component: AuthPageComponent,
    title: 'Authentication Page'
  },
  {
    path: 'tasks',
    title: 'Your tasks',
    loadComponent: () => import('@pages/tasks/tasks-page.component').then(c => c.TaskPageComponent),
    canActivate: [authGuard]
  },
  {
    path: '',
    pathMatch: 'full',
    redirectTo: 'tasks',
  },
  {
    path: '**',
    redirectTo: '',
  }
];
