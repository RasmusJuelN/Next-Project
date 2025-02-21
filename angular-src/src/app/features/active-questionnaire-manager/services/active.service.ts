import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ApiService } from '../../../core/services/api.service';
import { QuestionnaireSession, Template } from '../models/active.models';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { User } from '../../../shared/models/user.model';

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
      .set('pageSize', pageSize.toString());
  
    if (searchStudent.trim() !== '') {
      if (searchStudentType === 'fullName') {
        params = params.set('SearchStudentFullName', searchStudent);
      } else if (searchStudentType === 'userName') {
        params = params.set('SearchStudentUserName', searchStudent);
      } else if (searchStudentType === 'both') {
        // Set both parameters when searching by both.
        params = params
          .set('SearchStudentFullName', searchStudent)
          .set('SearchStudentUserName', searchStudent);
      }
    }
  
    if (searchTeacher.trim() !== '') {
      if (searchTeacherType === 'fullName') {
        params = params.set('SearchTeacherFullName', searchTeacher);
      } else if (searchTeacherType === 'userName') {
        params = params.set('SearchTeacherUserName', searchTeacher);
      } else if (searchTeacherType === 'both') {
        // Set both parameters when searching by both.
        params = params
          .set('SearchTeacherFullName', searchTeacher)
          .set('SearchTeacherUserName', searchTeacher);
      }
    }
  
    return this.apiService.get<PaginationResponse<QuestionnaireSession>>(this.apiUrl, params);
  }

  createActiveQuestionnaire(data: { studentId: string; teacherId: string; templateId: string }): Observable<QuestionnaireSession> {
    return this.apiService.post<QuestionnaireSession>(this.apiUrl, data);
  }

  getActiveQuestionnaireById(id: string): Observable<QuestionnaireSession> {
    return this.apiService.get<QuestionnaireSession>(`${this.apiUrl}/${id}`);
  }

  updateActiveQuestionnaire(id: string, data: Partial<QuestionnaireSession>): Observable<QuestionnaireSession> {
    return this.apiService.put<QuestionnaireSession>(`${this.apiUrl}/${id}`, data);
  }

  deleteActiveQuestionnaire(id: string): Observable<void> {
    return this.apiService.delete<void>(`${this.apiUrl}/${id}`);
  }

  searchUsers(term: string, role: 'student' | 'teacher', page: number): Observable<PaginationResponse<User>> {
    let params = new HttpParams()
      .set('term', term)
      .set('role', role)
      .set('page', page.toString());
    return this.apiService.get<PaginationResponse<User>>(`${environment.apiUrl}/users/search`, params);
  }

  searchTemplates(term: string, page: number): Observable<PaginationResponse<Template>> {
    let params = new HttpParams()
      .set('term', term)
      .set('page', page.toString());
    return this.apiService.get<PaginationResponse<Template>>(`${environment.apiUrl}/templates/search`, params);
  }
}
