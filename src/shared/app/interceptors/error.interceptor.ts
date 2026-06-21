import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { ToastService } from '../../ui/toast/toast.service';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const toastService = inject(ToastService);

  return next(req).pipe(
    catchError((error) => {
      if (error.status === 0) {
        toastService.showError('Unable to connect to the server. Please check your internet connection.');
      }
      else if (error.status >= 500) {
        toastService.showError('The server encountered an unexpected error. Please try again later.');
      }
      else if (error.status === 401 || error.status === 403) {
        toastService.showError('Your session has expired or you lack permission. Please log in again.');
      }

      return throwError(() => error);
    })
  );
};
