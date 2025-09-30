// Angular service for fetching anonymised questionnaire data
import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { Data } from '@angular/router';
import { DataCompare } from '../models/data-compare.model';

@Injectable({
  providedIn: 'root'
})
export class DataCompareService {
  /**
   * Base API URL for active questionnaire endpoints
   */
  public apiUrl = `${environment.apiUrl}/active-questionnaire`;

  /**
   * Injected API service for HTTP requests
   */
  private apiService = inject(ApiService);

  /**
   * Fetches anonymised responses for a questionnaire.
   * If studentId is provided, fetches for that user; otherwise, fetches for all users.
   * @param templateId Questionnaire/template GUID
   * @param studentId User GUID (optional)
   * @returns Observable with anonymised response data
   */
  getAnonymisedResponses(templateId: string, studentId?: string) {
    // Build API URL with query parameters
    let url = `${environment.apiUrl}/active-questionnaire/getanonymisedresponses?QuestionnaireId=${templateId}`;
    if (studentId) {
      url += `&UserId=${studentId}`;
    }
    // Make GET request to backend
    return this.apiService.get<any>(url);
  }
}

