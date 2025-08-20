import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { TokenService } from '../services/token.service';
import { AuthService } from '../services/auth.service';
import { BehaviorSubject, catchError, filter, finalize, switchMap, take, throwError } from 'rxjs';

const isRefreshingFlag = { value: false }; // module-level state (persists across calls)
const refreshSubject = new BehaviorSubject<string | null>(null);

function addAuth(req: any, token: string | null) {
  return token ? req.clone({ setHeaders: { Authorization: `Bearer ${token}` } }) : req;
}

function isAuthOrRefreshUrl(url: string) {
  // Adjust to your exact endpoints if needed
  return url.endsWith('/auth') || url.endsWith('/auth/refresh');
}

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const tokenService = inject(TokenService);
  const authService = inject(AuthService);

  // 1) Never attach access token to auth/refresh calls and never try to refresh for them
  if (isAuthOrRefreshUrl(req.url)) {
    return next(req);
  }

  // Attach current access token once
  const authReq = addAuth(req, tokenService.getToken());

  return next(authReq).pipe(
    catchError((error: HttpErrorResponse) => {
      if (error.status !== 401) {
        return throwError(() => error);
      }

      // 2) Handle 401s: coordinate refresh across parallel requests
      if (!isRefreshingFlag.value) {
        isRefreshingFlag.value = true;
        refreshSubject.next(null); // reset gate

        return authService.refreshToken().pipe(
          switchMap(() => {
            // refreshToken() should update TokenService internally
            const newToken = tokenService.getToken();
            refreshSubject.next(newToken); // release queued requests
            // Retry original request once with the fresh token
            return next(addAuth(req, newToken));
          }),
          catchError((refreshErr) => {
            // 3) If refresh fails, logout and propagate the original 401
            authService.logout();
            return throwError(() => refreshErr);
          }),
          finalize(() => {
            isRefreshingFlag.value = false;
          })
        );
      }

      // 4) A refresh is already in progress: wait for it to finish, then retry once
      return refreshSubject.pipe(
        filter((t) => t !== null),
        take(1),
        switchMap((t) => next(addAuth(req, t)))
      );
    })
  );
};