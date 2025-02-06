// role.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs';

type UserRole = 'teacher' | 'admin' | 'student';

export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Retrieve the array of allowed roles from route data
  const allowedRoles: UserRole[] = route.data?.['roles'] || [];

  // Subscribe to the current userRole via userRole$
  return authService.userRole$.pipe(
    map((userRole) => {
      if (!userRole) {
        router.navigate(['/']);
        return false;
      }

      // Check if the userRole is one of the allowed roles
      const hasAccess = allowedRoles.includes(userRole as UserRole);
      if (!hasAccess) {
        // User doesn't have a matching role: redirect or show an error page
        router.navigate(['/']);
        return false;
      }

      // Role is allowed
      return true;
    })
  );
};
