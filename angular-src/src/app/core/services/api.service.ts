import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ErrorHandlingService } from './error-handling.service';

/**
 * Wrapper around Angular's `HttpClient` that:
 * - Provides shorthand methods for common HTTP verbs.
 * - Applies centralized error handling through `ErrorHandlingService`.
 *
 * Usage:
 * ```ts
 * this.apiService.get<User[]>('/api/users').subscribe(...);
 * this.apiService.post('/api/items', { name: 'Example' }).subscribe(...);
 * ```
 */
@Injectable({
  providedIn: 'root',
})
export class ApiService {
  constructor(private http: HttpClient, private errorHandler: ErrorHandlingService) {}

  /**
   * Sends an HTTP GET request.
   * @param url - Endpoint URL
   * @param params - Optional query parameters
   * @param headers - Optional HTTP headers
   */
  get<T>(url: string, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .get<T>(url, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'GET Request Failed')));
  }

  /**
   * Sends an HTTP POST request.
   * @param url - Endpoint URL
   * @param body - Request payload
   * @param params - Optional query parameters
   * @param headers - Optional HTTP headers
   */
  post<T>(url: string, body: any, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .post<T>(url, body, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'POST Request Failed')));
  }

  /**
   * Sends an HTTP PUT request.
   * @param url - Endpoint URL
   * @param body - Request payload
   * @param params - Optional query parameters
   * @param headers - Optional HTTP headers
   */
  put<T>(url: string, body: any, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .put<T>(url, body, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'PUT Request Failed')));
  }

  /**
   * Sends an HTTP PATCH request.
   * @param url - Endpoint URL
   * @param body - Request payload
   * @param params - Optional query parameters
   * @param headers - Optional HTTP headers
   */
  patch<T>(url: string, body: any, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .patch<T>(url, body, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'PATCH Request Failed')));
  }

  /**
   * Sends an HTTP DELETE request.
   * @param url - Endpoint URL
   * @param params - Optional query parameters
   * @param headers - Optional HTTP headers
   */
  delete<T>(url: string, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .delete<T>(url, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'DELETE Request Failed')));
  }
  /**
   * Sends an HTTP HEAD request.
   * @param url - Endpoint URL
   * @param params - Optional query parameters
   * @param headers - Optional HTTP headers
   */
  head<T>(url: string, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .head<T>(url, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'HEAD Request Failed')));
  }
}
