import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { SessionService } from '@entities/session/model/session.service';

export const authGuard: CanActivateFn = () => {
  if (inject(SessionService).accessToken()) {
    return true;
  }

  return inject(Router).parseUrl('/auth');
};
