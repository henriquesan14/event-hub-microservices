import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { filter, map, take } from 'rxjs';
import { toObservable } from '@angular/core/rxjs-interop';
import { AuthStore } from './auth.store';

export const authGuard: CanActivateFn = (_, state) => {
  const auth = inject(AuthStore);
  const router = inject(Router);

  return toObservable(auth.isReady).pipe(
    filter(Boolean),
    take(1),
    map(() => auth.isAuthenticated()
      ? true
      : router.createUrlTree(['/entrar'], { queryParams: { returnUrl: state.url } })),
  );
};
