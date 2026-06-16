import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { SessionService } from '@entities/session/model/session.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const sessionService = inject(SessionService);
  const router = inject(Router);

  const accessToken = sessionService.accessToken();

  let authReq = req;
  if (accessToken) {
    authReq = req.clone({
      setHeaders: { Authorization: `Bearer ${accessToken}` }
    });
  }

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      const isAuthRequest = req.url.includes('/auth/login')
                         || req.url.includes('/auth/register')
                         || req.url.includes('/auth/refresh');

      if (error.status === 401 && !isAuthRequest) {
        return sessionService.refresh().pipe(
          switchMap((newTokens) => {
            const retryReq = req.clone({
              setHeaders: { Authorization: `Bearer ${newTokens.accessToken}` }
            });
            return next(retryReq);
          }),
          catchError((refreshError) => {
            sessionService.logout();
            router.navigate(['/auth']);
            return throwError(() => refreshError);
          })
        );
      }

      return throwError(() => error);
    })
  );
};
