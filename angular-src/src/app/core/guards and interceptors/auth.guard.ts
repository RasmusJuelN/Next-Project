import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs';

/**
 * Authentication guard.
 *
 * Ensures that only authenticated users can access protected routes.
 * - If the user is authenticated → returns `true` (navigation allowed).
 * - If not authenticated → redirects to `'/'` (login page) and returns `false`.
 *
 * @returns An observable that resolves to `true` (allow) or `false` (deny).
 *
 * @example
 * ```ts
 * export const routes: Routes = [
 *   { path: '', component: HomeComponent },
 *   { path: 'hub', component: AccessHubComponent, canActivate: [authGuard] }
 * ];
 * ```
 */
export const authGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  return authService.isAuthenticated$.pipe(
    map((isAuthenticated) => {
      if (isAuthenticated) {
        return true;
      } else {
        router.navigate(['/']);
        return false;
      }
    })
  );
};
