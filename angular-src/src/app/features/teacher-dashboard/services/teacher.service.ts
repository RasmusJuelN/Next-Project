import { inject, Injectable } from '@angular/core';
import { QuestionnaireGroupResponse } from '../models/dashboard.model';
import { Observable, of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiService } from '../../../core/services/api.service';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { HttpParams } from '@angular/common/http';

/**
 * Teacher service.
 *
 * Provides teacher-specific operations for active questionnaires.
 *
 * Handles:
 * - Fetching active questionnaires with cursor-based pagination.
 * - Searching by student name or questionnaire id.
 * - Filtering by student/teacher completion status.
 */
@Injectable({
  providedIn: 'root'
})
export class TeacherService {
  private apiUrl = `${environment.apiUrl}/user/teacher`;
  private apiService = inject(ApiService);

 /**
   * Retrieves paginated questionnaire groups for the authenticated teacher.
   * 
   * @param pageNumber Current page (1-based)
   * @param pageSize Items per page
   * @param searchTitle Optional search filter for group names
   * @param filterPendingStudent Show only groups with incomplete student submissions
   * @param filterPendingTeacher Show only groups with incomplete teacher reviews
   */
  getQuestionnaireGroups(
  pageNumber: number,
  pageSize: number,
  searchTitle: string,
  filterPendingStudent: boolean,
  filterPendingTeacher: boolean
): Observable<QuestionnaireGroupResponse> {
  let params = new HttpParams()
      .set('pageNumber', pageNumber.toString())
      .set('pageSize', pageSize.toString());
  if (searchTitle) params = params.set('title', searchTitle);
  if (filterPendingStudent) params = params.set('pendingStudent', 'true');
  if (filterPendingTeacher) params = params.set('pendingTeacher', 'true');

  return this.apiService.get<QuestionnaireGroupResponse>(
    `${this.apiUrl}/activequestionnaires/GroupedAndOffsetPaginated`,
    params
  );
}


}
