import { Injectable } from '@angular/core';
import { Observable, throwError } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {

  constructor() { }

  handleError(error: any, specificMessage?: string): Observable<never> {
    console.error('Error occurred:', specificMessage, error);
    return throwError(() => new Error(specificMessage ? `${specificMessage}: ${error.message}` : error.message));
  }
  
}
