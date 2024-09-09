import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Observable } from 'rxjs';
import { map, take } from 'rxjs/operators';
import { AuthService } from '../services/auth/auth.service';

export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService); // Get instance of AuthService
  const router = inject(Router);              // Get instance of Router

  // Return an observable that checks the authentication state
  return authService.isAuthenticated$.pipe(
    take(1), // Take the latest authentication state and complete
    map(isAuthenticated => {
      if (isAuthenticated) {
        return true; // If authenticated, allow access
      } else {
        router.navigate(['/'], { queryParams: { returnUrl: state.url } });
        return false;
      }
    })
  );
};