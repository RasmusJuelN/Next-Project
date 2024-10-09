import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Observable, of } from 'rxjs';
import { map, switchMap, take } from 'rxjs/operators';
import { AuthService } from '../services/auth/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService); 
  const router = inject(Router); 

  return authService.isAuthenticated$.pipe(
    take(1), 
    switchMap(isAuthenticated => {
      if (!isAuthenticated) {
        router.navigate(['/'], { queryParams: { returnUrl: state.url } });
        return of(false);
      } else {
        return authService.checkServerConnection().pipe(
          map(isServerOnline => {
            if (isServerOnline) {
              return true; 
            } else {
              router.navigate(['/error']);
              return false;
            }
          })
        );
      }
    })
  );
};
