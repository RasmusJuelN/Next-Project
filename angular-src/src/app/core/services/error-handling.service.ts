import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

/**
 * Centralized error handling service.
 *
 * Responsibilities:
 * - Logs errors to the console (with optional context message).
 * - Wraps them into a new `Error` and rethrows via RxJS `throwError`.
 *
 * This service is typically used in `catchError` pipes in API calls.
 *
 * @example
 * ```ts
 * this.apiService.get<User[]>('/api/users')
 *   .pipe(
 *     catchError(err => this.errorHandler.handleError(err, 'Failed to load users'))
 *   )
 *   .subscribe(...);
 * ```
 */
@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {
  handleError(err: any, specificMessage?: string): Observable<never> {
    const error = new Error(specificMessage ? `${specificMessage}: ${err.message}` : err.message);
    console.error('Error occurred:', error);
    if (err instanceof HttpErrorResponse) {
      return throwError(() => err);  // keep the real HttpErrorResponse
    }
    return throwError(() => error);
  }
}
