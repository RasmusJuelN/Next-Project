import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { catchError, switchMap, throwError } from 'rxjs';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const authService = inject(AuthService);
  const token = tokenService.getToken();

  const authReq = token
    ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } })
    : req;

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status === 401) {
        // Token might be expired; try refreshing token.
        return authService.refreshToken().pipe(
          switchMap((_response) => {
            // After refreshing, retry the original request with the new token.
            const newToken = tokenService.getToken();
            const newAuthReq = newToken
              ? req.clone({ setHeaders: { Authorization: `Bearer ${newToken}` } })
              : req;
            return next(newAuthReq);
          }),
          catchError((err) => {
            // If refresh fails, log out and propagate the error.
            authService.logout();
            return throwError(err);
          })
        );
      }
      return throwError(error);
    })
  );
};
