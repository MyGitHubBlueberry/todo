import { Routes } from '@angular/router';
import { AuthPage } from '@pages/auth-page';

export const routes: Routes = [
  {
    path: '',
    component: AuthPage,
    title: 'Authentication Page'
  }
];
