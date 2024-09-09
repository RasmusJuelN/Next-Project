import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { AuthService } from '../services/auth/auth.service';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  // Get the required roles from route data
  const requiredRoles: string[] = route.data['roles'] || [];

  // Check if the user has at least one of the required roles
  const hasRole = requiredRoles.some(role => authService.hasRole(role));

  if (hasRole) {
    return true; // User has one of the roles, allow access
  } else {
    // Redirect to login or a not-authorized page if the role check fails
    router.navigate(['/']);
    return false;
  }
};