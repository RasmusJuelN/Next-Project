import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment.development';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class HomeService {
  private apiUrl = environment.apiUrl;
  private apiService = inject(ApiService);

  // Method to check for active questionnaires
  checkForExistingActiveQuestionnaires(): Observable<{ exists: boolean; id: string | null }> {
    // Updated URL without a userId parameter, as the backend extracts the user from the token
    const url = `${this.apiUrl}/active-questionnaire/check`;

    return this.apiService.get<string | null>(url).pipe(
      map((questionnaireId) => ({
        exists: !!questionnaireId,
        id: questionnaireId
      }))
    );
  }
}
