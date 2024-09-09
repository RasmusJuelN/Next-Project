import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { JWTTokenService } from '../services/auth/jwt-token.service';

export const jwtInterceptor: HttpInterceptorFn = (req, next) => {
  const jwtService = inject(JWTTokenService);
  const token = jwtService.getToken();  // Get the JWT token from the AuthService

  // Clone the request to add the Authorization header with the JWT token
  const authReq = token
    ? req.clone({
        setHeaders: { Authorization: `Bearer ${token}` },
      })
    : req;  // If no token, don't modify the request

  // Pass the modified request to the next handler
  return next(authReq);
};
