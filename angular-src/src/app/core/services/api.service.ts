import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  constructor(private http: HttpClient, private errorHandler: ErrorHandlingService) {}

  get<T>(url: string, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .get<T>(url, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'GET Request Failed')));
  }

  post<T>(url: string, body: any, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .post<T>(url, body, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'POST Request Failed')));
  }

  put<T>(url: string, body: any, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .put<T>(url, body, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'PUT Request Failed')));
  }

  patch<T>(url: string, body: any, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .patch<T>(url, body, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'PATCH Request Failed')));
  }

  delete<T>(url: string, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .delete<T>(url, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'DELETE Request Failed')));
  }
  head<T>(url: string, params?: HttpParams, headers?: HttpHeaders): Observable<T> {
    return this.http
      .head<T>(url, { params, headers })
      .pipe(catchError((error) => this.errorHandler.handleError(error, 'HEAD Request Failed')));
  }
}
