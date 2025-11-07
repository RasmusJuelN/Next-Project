import { inject, Injectable } from '@angular/core';
import { ActiveQuestionnaireResponse, QuestionnaireGroupResponse } from '../models/dashboard.model';
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
   * Retrieves active questionnaires for the teacher.
   *
   * @param searchTerm Text to search (student name or questionnaire id).
   * @param searchType Field to search: 'name' | 'id'.
   * @param queryCursor Cursor for the next page (null for first page).
   * @param pageSize Number of items per page.
   * @param filterStudentCompleted If true, only include those where student is done.
   * @param filterTeacherCompleted If true, only include those where teacher is done.
   * @returns Observable emitting the paginated response.
   */
  getQuestionnaires(
    searchTerm: string,
    searchType: 'name' | 'id',
    queryCursor: string | null,
    pageSize: number,
    filterStudentCompleted: boolean,
    filterTeacherCompleted: boolean
  ): Observable<ActiveQuestionnaireResponse> {
    let params = new HttpParams()
      .set('pageSize', pageSize.toString())
      .set('filterStudentCompleted', filterStudentCompleted.toString())
      .set('filterTeacherCompleted', filterTeacherCompleted.toString());
  
    if (queryCursor) {
      params = params.set('queryCursor', queryCursor);
    }
  
    // Use appropriate query parameter based on search type.
    if (searchType === 'id') {
      params = params.set('activeQuestionnaireId', searchTerm);
    } else {
      params = params.set('student', searchTerm);
    }
  
    return this.apiService.get<ActiveQuestionnaireResponse>(
      `${this.apiUrl}/activequestionnaires`,
      params
    );
  }

  getQuestionnaireGroups(
  pageSize: number,
  queryCursor: string | null,
  searchTitle: string,
  filterPendingStudent: boolean,
  filterPendingTeacher: boolean
): Observable<QuestionnaireGroupResponse> {
  let params = new HttpParams().set('pageSize', pageSize.toString());
  if (queryCursor) params = params.set('queryCursor', queryCursor);
  if (searchTitle) params = params.set('title', searchTitle);
  if (filterPendingStudent) params = params.set('pendingStudent', 'true');
  if (filterPendingTeacher) params = params.set('pendingTeacher', 'true');

  return this.apiService.get<QuestionnaireGroupResponse>(
    `${this.apiUrl}/activequestionnaires/grouped`,
    params
  );
}


}
