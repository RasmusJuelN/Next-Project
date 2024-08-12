import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class ErrorHandlingService {

  constructor() { }

  handleError(error: any, specficMessage: string | void): void {
    console.error('Error occurred:', error);
  }
  
}
