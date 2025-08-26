import { inject, Injectable } from '@angular/core';
import { environment } from '../../../../environments/environment';
import { ApiService } from '../../../core/services/api.service';
import { ActiveQuestionnaire, ResponseActiveQuestionnaireBase, TemplateBaseResponse, UserPaginationResult } from '../models/active.models';
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
      params = params.set('Student', studentSearch);
    }
  
    if (teacherSearch.trim() !== '') {
      params = params.set('Teacher', teacherSearch);
    }
  
    return this.apiService.get<ResponseActiveQuestionnaireBase>(this.apiUrl, params);
  }
  


  createActiveQuestionnaire(aq: { studentIds: string[]; teacherIds: string[]; templateId: string }): Observable<ActiveQuestionnaire[]> {
  const body = {
    StudentIds: aq.studentIds,
    TeacherIds: aq.teacherIds,
    TemplateId: aq.templateId
  };
  return this.apiService.post<ActiveQuestionnaire[]>(`${this.apiUrl}/activate`, body);
}

  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire> {
    return this.apiService.get<ActiveQuestionnaire>(`${this.apiUrl}/${id}`);
  }

  searchUsers(
    term: string,
    role: 'student' | 'teacher',
    pageSize: number,
    sessionId?: string
  ): Observable<UserPaginationResult> {
    // Convert the role string to match the C# enum (e.g., 'student' -> 'Student')
    const formattedRole = role.charAt(0).toUpperCase() + role.slice(1);
  
    let params = new HttpParams()
      .set('User', term)       // Matches the C# record property name
      .set('Role', formattedRole)
      .set('PageSize', pageSize.toString());
  
    if (sessionId) {
      params = params.set('SessionId', sessionId);
    }
  
    // Updated endpoint matching the API controller route
    return this.apiService.get<UserPaginationResult>(
      `${environment.apiUrl}/User`,
      params
    );
  }

searchTemplates(term: string, queryCursor?: string): Observable<TemplateBaseResponse> {
  let params = new HttpParams()
    .set('title', term)
    .set('pageSize', 5)
    .set('templateStatus', 'Finalized');

  if (queryCursor) {
    params = params.set('queryCursor', queryCursor);
  }

    return this.apiService.get<TemplateBaseResponse>(`${environment.apiUrl}/questionnaire-template/`, params);
  }

  createActiveQuestionnaireGroup(aq: { name: string; templateId: string; studentIds: string[]; teacherIds: string[] }) {
  const body = {
    Name: aq.name,
    TemplateId: aq.templateId,
    StudentIds: aq.studentIds,
    TeacherIds: aq.teacherIds
  };
  return this.apiService.post<any>(`${this.apiUrl}/creategroup`, body);
}

getQuestionnaireGroup(groupId: string) {
  return this.apiService.get<any>(`${this.apiUrl}/${groupId}/getGroup`);
}

getQuestionnaireGroups() {
  return this.apiService.get<any[]>(`${this.apiUrl}/groups`);
}

createAnonymousQuestionnaireGroup(payload: { participantIds: string[], templateId: string }) {
  return this.apiService.post<any>(`${this.apiUrl}/anonymous-group`, payload);
}
}
