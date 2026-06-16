import { Routes } from '@angular/router';
import { authGuard } from './providers/auth.provider';
import { AuthPage } from '@pages/auth/auth-page.component';

export const routes: Routes = [
  {
    path: 'auth',
    component: AuthPage,
    title: 'Authentication Page'
  },
  {  //todo
    path: 'tasks',
    // loadComponent: () => import('@pages/tasks-page').then(m => m.TasksPage),
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
