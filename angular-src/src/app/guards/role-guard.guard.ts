import { CanActivateFn } from '@angular/router';
import { inject } from '@angular/core';
import { AppAuthService } from '../services/auth/app-auth.service';
import { Router } from '@angular/router';

export const roleGuard: CanActivateFn = (route, state) => {
  const authService = inject(AppAuthService);
  const router = inject(Router);

  // Get the required role from route data
  const requiredRole = route.data['role'];

  if (authService.hasRole(requiredRole)) {
    return true;
  } else {
    // Redirect to login or a not-authorized page if the role check fails
    router.navigate(['/']);
    return false;
  }
};