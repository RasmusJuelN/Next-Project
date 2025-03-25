import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { Result } from '../models/result.model';

@Injectable({
  providedIn: 'root'
})
export class ResultService {
  private apiUrl = `${environment.apiUrl}/active-questionnaire`;
  private apiService = inject(ApiService);

  
  getResultById(id: string): Observable<Result> {
    return this.apiService.get<Result>(`${this.apiUrl}/${id}/getresponse`);
  }
}
