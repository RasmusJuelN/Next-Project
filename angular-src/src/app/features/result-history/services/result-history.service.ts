import { Injectable, inject } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Result } from '../../result/models/result.model';
import { TemplateBase, TemplateStatus } from '../../../shared/models/template.model';
import { Role, User } from '../../../shared/models/user.model';
import { HttpParams } from '@angular/common/http';
import { TemplateBaseResponse, UserPaginationResult } from '../models/result-history.model';

export interface StudentResultHistory {
  results: Result[];
  student: User;
  template: TemplateBase;
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

    const templateTitle = `Mock Template ${templateId.slice(0, 4)}`;
    const mockResults = [
      this.createMockResult('2024-02-01', `${templateTitle} • Initial Assessment`),
      this.createMockResult('2024-04-15', `${templateTitle} • Mid-term Assessment`),
      this.createMockResult('2024-07-20', `${templateTitle} • Final Assessment`)
    ];

    const student: User = {
      id: studentId,
      userName: 'mock.student',
      fullName: 'Mock Student',
      role: Role.Student
    };

    const template: TemplateBase = {
      id: templateId,
      title: templateTitle,
      createdAt: new Date().toISOString(),
      lastUpdated: new Date().toISOString(),
      isLocked: false,
      templateStatus: TemplateStatus.Finalized
    };

    return of({ results: mockResults, student, template });
  }

  /**
   * Get all results for a specific student across all templates
   */
  getAllStudentResults(studentId: string): Observable<Result[]> {
    // TODO: Replace with API call when backend endpoint is ready
    return of([]);
  }

  /**
   * Get result by ID
   */
  getResultById(resultId: string): Observable<Result> {
    return this.apiService.get<Result>(`${this.apiUrl}/${resultId}/getresponse`);
  }

  // -------------------
  // SEARCH HELPERS
  // -------------------

  searchTemplates(term: string, queryCursor?: string): Observable<TemplateBaseResponse> {
    let params = new HttpParams()
      .set('title', term)
      .set('pageSize', 5)
      .set('templateStatus', 'Finalized');

    if (queryCursor) params = params.set('queryCursor', queryCursor);

    return this.apiService.get<TemplateBaseResponse>(
      `${environment.apiUrl}/questionnaire-template/`,
      params
    );
  }

  searchUsers(
    term: string,
    role: 'student' | 'teacher',
    pageSize: number,
    sessionId?: string
  ): Observable<UserPaginationResult> {
    const formattedRole = role.charAt(0).toUpperCase() + role.slice(1);

    let params = new HttpParams()
      .set('User', term)
      .set('Role', formattedRole)
      .set('PageSize', pageSize.toString());

    if (sessionId) params = params.set('SessionId', sessionId);

    return this.apiService.get<UserPaginationResult>(`${environment.apiUrl}/User`, params);
  }

  // -------------------
  // MOCK DATA BUILDER
  // -------------------
  private createMockResult(date: string, title: string): Result {
    const baseDate = new Date(date);
    const stage = baseDate < new Date('2024-03-01') ? 'early' : baseDate < new Date('2024-06-01') ? 'mid' : 'late';
    const pick = (early: any, mid: any, late: any) =>
      stage === 'early' ? early : stage === 'mid' ? mid : late;

    const student = {
      user: {
        id: 'student-1',
        fullName: 'John Doe',
        userName: 'john.doe',
        role: Role.Student
      },
      completedAt: baseDate
    };

    const teacher = {
      user: {
        id: 'teacher-1',
        fullName: 'Jane Smith',
        userName: 'jane.smith',
        role: Role.Teacher
      },
      completedAt: baseDate
    };

    return {
      id: `mock-${date}`,
      title: `${title} (${date})`,
      description: `Assessment completed on ${date}`,
      student,
      teacher,
      answers: [
        {
          question: 'How well does the student understand basic arithmetic?',
          studentResponse: pick('Fair', 'Good', 'Excellent'),
          isStudentResponseCustom: false,
          teacherResponse: pick('Good', 'Good', 'Excellent'),
          isTeacherResponseCustom: false,
          options: ['Poor', 'Fair', 'Good', 'Excellent', 'Outstanding'].map((opt, i) => ({
            displayText: opt,
            optionValue: (i + 1).toString(),
            isSelectedByStudent: opt === pick('Fair', 'Good', 'Excellent'),
            isSelectedByTeacher: opt === pick('Good', 'Good', 'Excellent')
          }))
        },
        {
          question: "Student's problem-solving skills in mathematics",
          studentResponse: pick('Poor', 'Fair', 'Good'),
          isStudentResponseCustom: false,
          teacherResponse: pick('Fair', 'Good', 'Excellent'),
          isTeacherResponseCustom: false,
          options: ['Poor', 'Fair', 'Good', 'Excellent', 'Outstanding'].map((opt, i) => ({
            displayText: opt,
            optionValue: (i + 1).toString(),
            isSelectedByStudent: opt === pick('Poor', 'Fair', 'Good'),
            isSelectedByTeacher: opt === pick('Fair', 'Good', 'Excellent')
          }))
        },
        {
          question: 'What motivates the student most in learning?',
          studentResponse: pick(
            "I find math challenging but I'm trying my best.",
            "I'm getting better at understanding math concepts.",
            'Math has become much more enjoyable!'
          ),
          isStudentResponseCustom: true,
          teacherResponse: pick(
            'Needs support with problem-solving strategies.',
            'Shows improvement with practical examples.',
            'Excellent progress with analytical skills.'
          ),
          isTeacherResponseCustom: true,
          options: ['Grades and recognition', 'Understanding concepts', 'Practical applications', 'Peer interaction', 'Other (please specify)'].map(
            (opt, i) => ({
              displayText: opt,
              optionValue: (i + 1).toString(),
              isSelectedByStudent: false,
              isSelectedByTeacher: false
            })
          )
        },
        {
          question: 'How does the student respond to collaborative learning activities?',
          studentResponse: pick('Peer interaction', 'Practical applications', 'Understanding concepts'),
          isStudentResponseCustom: false,
          teacherResponse: pick(
            'Initially struggles with group work and needs encouragement.',
            'Shows growing comfort and better communication in groups.',
            'Excellent collaborator who helps peers and leads group discussions.'
          ),
          isTeacherResponseCustom: true,
          options: ['Prefers individual work', 'Peer interaction', 'Practical applications', 'Understanding concepts', 'Mixed results'].map(
            (opt, i) => ({
              displayText: opt,
              optionValue: (i + 1).toString(),
              isSelectedByStudent: opt === pick('Peer interaction', 'Practical applications', 'Understanding concepts'),
              isSelectedByTeacher: false
            })
          )
        }
      ]
    };
  }
}
