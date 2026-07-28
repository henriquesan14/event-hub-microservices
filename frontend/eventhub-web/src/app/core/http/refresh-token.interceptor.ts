import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { HttpClient } from '@angular/common/http';

export const refreshTokenInterceptor: HttpInterceptorFn = (request, next) => {
  const http = inject(HttpClient);
  const isAuthRequest = request.url.includes('/api/auth');

  return next(request).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401 || isAuthRequest) {
        return throwError(() => error);
      }

      return http.post<void>('/api/auth/refresh', {}, { withCredentials: true }).pipe(
        switchMap(() => next(request)),
        catchError(refreshError => throwError(() => refreshError)),
      );
    }),
  );
};
