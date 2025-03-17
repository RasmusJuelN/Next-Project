import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ApiService } from '../../../core/services/api.service';
import { ActiveQuestionnaire, ResponseActiveQuestionnaireBase, Template } from '../models/active.models';
import { PaginationResponse } from '../../../shared/models/Pagination.model';
import { Observable } from 'rxjs';
import { HttpParams } from '@angular/common/http';
import { User } from '../../../shared/models/user.model';

@Injectable({
  providedIn: 'root',
})
export class ActiveService {
  private apiUrl = `${environment.apiUrl}/active-questionnaire`;
  private apiService = inject(ApiService);

  constructor() {}

  testgetActiveQuestionnaires(
    page: number,
    pageSize: number,
    searchStudent: string = '',
    searchStudentType: 'fullName' | 'userName' | 'both' = 'both',
    searchTeacher: string = '',
    searchTeacherType: 'fullName' | 'userName' | 'both' = 'both'
  ): Observable<PaginationResponse<ActiveQuestionnaire>> {
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
  
    return this.apiService.get<PaginationResponse<ActiveQuestionnaire>>(this.apiUrl, params);
  }

  getActiveQuestionnaires(
    pageSize: number,
    queryCursor: string = '',
    studentSearch: string = '',
    studentSearchType: 'fullName' | 'userName' | 'both' = 'both',
    teacherSearch: string = '',
    teacherSearchType: 'fullName' | 'userName' | 'both' = 'both'
  ): Observable<ResponseActiveQuestionnaireBase> {
    let params = new HttpParams().set('PageSize', pageSize.toString());
  
    if (queryCursor.trim() !== '') {
      params = params.set('QueryCursor', queryCursor);
    }
  
    if (studentSearch.trim() !== '') {
      params = params.set('StudentSearch', studentSearch);
      params = params.set('StudentSearchType', studentSearchType);
    }
  
    if (teacherSearch.trim() !== '') {
      params = params.set('TeacherSearch', teacherSearch);
      params = params.set('TeacherSearchType', teacherSearchType);
    }
  
    return this.apiService.get<ResponseActiveQuestionnaireBase>(this.apiUrl, params);
  }
  
  


  createActiveQuestionnaire(aq: { studentId: string; teacherId: string; templateId: string }): Observable<ActiveQuestionnaire> {
    const formData = new FormData();
    formData.append('StudentId', aq.studentId);
    formData.append('TeacherId', aq.teacherId);
    formData.append('TemplateId', aq.templateId);
    return this.apiService.post<ActiveQuestionnaire>(`${this.apiUrl}/activate`, formData);
  }

  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire> {
    return this.apiService.get<ActiveQuestionnaire>(`${this.apiUrl}/${id}`);
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
