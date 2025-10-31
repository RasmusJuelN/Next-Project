// Angular service for fetching anonymised questionnaire data
import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Observable } from 'rxjs';
import { Data } from '@angular/router';
import { DataCompareOverTime } from '../models/data-compare-overtime.model';

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
  /**
   * Fetches anonymised responses for a questionnaire.
   * If userId is provided, fetches for that user; if groupId is provided, fetches for that group.
   * @param templateId Questionnaire/template GUID
   * @param userId User GUID (optional)
   * @param groupId Group GUID (optional)
   * @returns Observable with anonymised response data
   */
  getAnonymisedResponses(templateId: string, userId?: string, groupId?: string) {
    let url = `${environment.apiUrl}/active-questionnaire/getanonymisedresponses?QuestionnaireId=${templateId}`;
    if (userId) {
      url += `&Users=${userId}`;
    }
    if (groupId) {
      url += `&Groups=${groupId}`;
    }
    return this.apiService.get<any>(url);
  }

  getQuestionaireDataByID(templateId:string)
  {
    let url = `${environment.apiUrl}/active-questionnaire/${templateId}`;
    return this.apiService.get<any>(url);
  }
}

