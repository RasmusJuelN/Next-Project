import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ApiService } from '../../../core/services/api.service';
import { QuestionnaireSession } from '../models/active.models';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';

@Injectable({
  providedIn: 'root',
})
export class ActiveService {
  private apiUrl = `${environment.apiUrl}/active-questionnaires`;
  private apiService = inject(ApiService);

  constructor() {}

  getActiveQuestionnaires(
    page: number,
    pageSize: number,
    searchStudent: string = '',
    searchStudentType: 'fullName' | 'userName' | 'both' = 'both',
    searchTeacher: string = '',
    searchTeacherType: 'fullName' | 'userName' | 'both' = 'both'
  ): Observable<PaginationResponse<QuestionnaireSession>> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString())
      .set('searchStudent', searchStudent)
      .set('searchStudentType', searchStudentType)
      .set('searchTeacher', searchTeacher)
      .set('searchTeacherType', searchTeacherType);

    return this.apiService.get<PaginationResponse<QuestionnaireSession>>(this.apiUrl, params);
  }
}
