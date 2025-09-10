import { inject } from '@angular/core';
import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs';
import { Role } from '../../shared/models/user.model';


/**
 * Role-based route guard.
 *
 * Ensures that only users with specific roles (provided via `data.roles`) can access a route.
 *
 * Behavior:
 * - If no role is set → redirects to `'/'` and denies access.
 * - If the role is not in the allowed list → redirects to `'/'` and denies access.
 * - If the role is valid and allowed → returns `true` and navigation continues.
 *
 * @param route - The activated route snapshot (used to read `data.roles`).
 * @param state - The current router state snapshot.
 * @returns An observable resolving to `true` (allow) or `false` (deny).
 *
 * @example
 * ```ts
 * export const routes: Routes = [
 *   {
 *     path: 'templates',
 *     component: TemplateManagerComponent,
 *     canActivate: [authGuard, roleGuard],
 *     data: { roles: [Role.Admin] } // only Admins allowed
 *   },
 *   {
 *     path: 'results/:id',
 *     component: ResultComponent,
 *     canActivate: [authGuard, roleGuard],
 *     data: { roles: [Role.Teacher, Role.Student] } // Teachers + Students allowed
 *   }
 * ];
 * ```
 */
export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  const allowedRoles: Role[] = route.data?.['roles'] || [];

  return authService.userRole$.pipe(
    map((userRole) => {
      if (!userRole) {
        router.navigate(['/']);
        return false;
      }

      const role = userRole as Role;
      const hasAccess = allowedRoles.includes(role);
      if (!hasAccess) {
        router.navigate(['/']);
        return false;
      }

      return true;
    })
  );
};
