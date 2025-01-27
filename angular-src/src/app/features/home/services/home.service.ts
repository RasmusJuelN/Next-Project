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
  checkForExistingActiveQuestionnaires(userId: string): Observable<{ exists: boolean; id: string | null }> {
    const url = `${this.apiUrl}/questionnaire/active/check/${userId}`;

    return this.apiService.get<{ active: boolean; questionnaireId: string | null }>(url).pipe(
      map((response) => ({
        exists: response.active,
        id: response.questionnaireId
      }))
    );
  }
}
