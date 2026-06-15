import { Routes } from '@angular/router';
import { AuthPage } from '@pages/auth-page';
import { authGuard } from './providers/auth.interceptor';

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
