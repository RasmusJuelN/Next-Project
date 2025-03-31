import { inject, Injectable } from '@angular/core';
import { ActiveQuestionnaireResponse } from '../models/dashboard.model';
import { Observable, of } from 'rxjs';
import { environment } from '../../../../environments/environment';
import { ApiService } from '../../../core/services/api.service';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class TeacherService {
  private apiUrl = `${environment.apiUrl}/user/teacher`;
  private apiService = inject(ApiService);

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
  
}
