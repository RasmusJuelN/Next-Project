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
  handleError(error: any, specificMessage?: string): Observable<never> {
    console.error('Error occurred:', specificMessage, error);
    return throwError(() => new Error(specificMessage ? `${specificMessage}: ${error.message}` : error.message));
  }
}
