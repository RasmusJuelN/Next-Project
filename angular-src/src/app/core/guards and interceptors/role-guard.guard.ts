// role.guard.ts
import { inject } from '@angular/core';
import { CanActivateFn, ActivatedRouteSnapshot, RouterStateSnapshot, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { map } from 'rxjs';
import { Role } from '../../shared/models/user.model';


export const roleGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state: RouterStateSnapshot) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Retrieve the array of allowed roles from route data
  const allowedRoles: Role[] = route.data?.['roles'] || [];

  // Subscribe to the current userRole via userRole$
  return authService.userRole$.pipe(
    map((userRole) => {
      if (!userRole) {
        router.navigate(['/']);
        return false;
      }

      // Explicitly cast the userRole (which is a string) to Role
      const role = userRole as Role;

      // Check if the role is one of the allowed roles
      const hasAccess = allowedRoles.includes(role);
      if (!hasAccess) {
        router.navigate(['/']);
        return false;
      }

      return true;
    })
  );
};
