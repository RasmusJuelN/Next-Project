import { Injectable, inject } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Result } from '../../result/models/result.model';
import { TemplateBase, TemplateStatus } from '../../../shared/models/template.model';
import { Role, User } from '../../../shared/models/user.model';
import { HttpParams } from '@angular/common/http';
import { TemplateBaseResponse, UserPaginationResult } from '../models/result-history.model';

export interface StudentResultHistory{
  results: Result[];
  student: User
  template: TemplateBase
}

@Injectable({
  providedIn: 'root'
})
export class ResultHistoryService {
  private apiUrl = `${environment.apiUrl}/active-questionnaire`;
  private apiService = inject(ApiService);

  /**
   * Get result history for a specific student and template combination
   * @param studentId - The ID of the student
   * @param templateId - The ID of the questionnaire template
   * @returns Observable of StudentResultHistory
   */
  getStudentResultHistory(studentId: string, templateId: string): Observable<StudentResultHistory> {
    // TODO: Replace with actual API call when backend endpoint is ready
    // return this.apiService.get<StudentResultHistory>(`${this.apiUrl}/student-history/${studentId}/template/${templateId}`);
    
    // For now, return mock data
    return of({
      results: [],
      student: {
        id: studentId,
        userName: 'mock.student',
        fullName: 'Mock Student',
        role: Role.Student
      },
      template: {
        id: templateId,
        title: 'Mock Template',
        createdAt: new Date().toISOString(),
        lastUpdated: new Date().toISOString(),
        isLocked: false,
        templateStatus: TemplateStatus.Finalized
      }
    });
  }

  /**
   * Get all results for a specific student across all templates
   * @param studentId - The ID of the student
   * @returns Observable of Result array
   */
  getAllStudentResults(studentId: string): Observable<Result[]> {
    // TODO: Replace with actual API call when backend endpoint is ready
    // return this.apiService.get<Result[]>(`${this.apiUrl}/student/${studentId}/all-results`);
    
    // For now, return empty array
    return of([]);
  }

  /**
   * Get result by ID - delegates to the existing result service
   * @param resultId - The ID of the result
   * @returns Observable of Result
   */
  getResultById(resultId: string): Observable<Result> {
    return this.apiService.get<Result>(`${this.apiUrl}/${resultId}/getresponse`);
  }



  // REAL
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
}