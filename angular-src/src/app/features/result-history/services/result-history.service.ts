import { Injectable, inject } from '@angular/core';
import { Observable, of } from 'rxjs';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Result } from '../../../shared/models/result.model';
import { Template, TemplateBase, TemplateStatus } from '../../../shared/models/template.model';
import { Role, User } from '../../../shared/models/user.model';
import { HttpParams } from '@angular/common/http';
import { Attempt, AttemptAnswer, StudentResultHistory, TemplateBaseResponse, UserPaginationResult } from '../models/result-history.model';

// Mock template with stable IDs for questions/options
const mockTemplate: Template = {
  id: 'template-123',
  title: 'Math Skills Assessment',
  description: 'Evaluation of numeracy skills, problem solving, and classroom engagement.',
  templateStatus: TemplateStatus.Finalized,
  createdAt: '2025-01-10T09:00:00Z',
  lastUpdated: '2025-02-01T12:00:00Z',
  isLocked: true,
  questions: [
    {
      id: 1,
      prompt: 'How well does the student understand basic arithmetic?',
      allowCustom: false,
      options: [
        { id: 10, optionValue: 1, displayText: 'Poor' },
        { id: 11, optionValue: 2, displayText: 'Fair' },
        { id: 12, optionValue: 3, displayText: 'Good' },
        { id: 13, optionValue: 4, displayText: 'Excellent' },
        { id: 14, optionValue: 5, displayText: 'Outstanding' }
      ]
    },
    {
      id: 2,
      prompt: "Student's problem-solving skills in mathematics",
      allowCustom: false,
      options: [
        { id: 20, optionValue: 1, displayText: 'Poor' },
        { id: 21, optionValue: 2, displayText: 'Fair' },
        { id: 22, optionValue: 3, displayText: 'Good' },
        { id: 23, optionValue: 4, displayText: 'Excellent' },
        { id: 24, optionValue: 5, displayText: 'Outstanding' }
      ]
    },
    {
      id: 3,
      prompt: 'What motivates the student most in learning?',
      allowCustom: true,
      options: [
        { id: 30, optionValue: 1, displayText: 'Grades and recognition' },
        { id: 31, optionValue: 2, displayText: 'Understanding concepts' },
        { id: 32, optionValue: 3, displayText: 'Practical applications' },
        { id: 33, optionValue: 4, displayText: 'Peer interaction' },
        { id: 34, optionValue: 5, displayText: 'Other (please specify)' }
      ]
    },
    {
      id: 4,
      prompt: 'How does the student respond to collaborative learning activities?',
      allowCustom: true,
      options: [
        { id: 40, optionValue: 1, displayText: 'Prefers individual work' },
        { id: 41, optionValue: 2, displayText: 'Peer interaction' },
        { id: 42, optionValue: 3, displayText: 'Practical applications' },
        { id: 43, optionValue: 4, displayText: 'Understanding concepts' },
        { id: 44, optionValue: 5, displayText: 'Mixed results' }
      ]
    }
  ]
};

// Shared mock users
const mockStudent: User = {
  id: 'student-1',
  fullName: 'John Doe',
  userName: 'john.doe',
  role: Role.Student
};

const mockTeacher: User = {
  id: 'teacher-1',
  fullName: 'Jane Smith',
  userName: 'jane.smith',
  role: Role.Teacher
};

// Helper: build answers for a single attempt
function buildAttemptAnswers(
  variant: 'early' | 'late'
): AttemptAnswer[] {
  // variant lets us simulate improvement over time

  return [
    // Q1: basic arithmetic understanding
    {
      questionId: '1',
      studentResponse: variant === 'early' ? 'Fair' : 'Excellent',
      isStudentResponseCustom: false,
      selectedOptionIdsByStudent: [
        variant === 'early' ? 11 /* Fair */ : 13 /* Excellent */
      ],

      teacherResponse: variant === 'early' ? 'Good' : 'Excellent',
      isTeacherResponseCustom: false,
      selectedOptionIdsByTeacher: [
        variant === 'early' ? 12 /* Good */ : 13 /* Excellent */
      ]
    },

    // Q2: problem-solving skills
    {
      questionId: '2',
      studentResponse: variant === 'early' ? 'Fair' : 'Good',
      isStudentResponseCustom: false,
      selectedOptionIdsByStudent: [
        variant === 'early' ? 21 /* Fair */ : 22 /* Good */
      ],

      teacherResponse: variant === 'early' ? 'Good' : 'Excellent',
      isTeacherResponseCustom: false,
      selectedOptionIdsByTeacher: [
        variant === 'early' ? 22 /* Good */ : 23 /* Excellent */
      ]
    },

    // Q3: motivation (custom allowed)
    {
      questionId: '3',
      studentResponse:
        variant === 'early'
          ? "I find math challenging but I'm trying my best."
          : 'Math has become much more enjoyable!',
      isStudentResponseCustom: true,
      selectedOptionIdsByStudent: [
        // student didn't explicitly pick from predefined in early,
        // but in late, let's say they clicked "Understanding concepts"
        ...(variant === 'late' ? [31] : [])
      ],

      teacherResponse:
        variant === 'early'
          ? 'Needs support with problem-solving strategies.'
          : 'Excellent progress with analytical skills.',
      isTeacherResponseCustom: true,
      selectedOptionIdsByTeacher: undefined // teacher feedback is narrative here
    },

    // Q4: collaboration
    {
      questionId: '4',
      studentResponse:
        variant === 'early'
          ? 'Peer interaction'
          : 'Understanding concepts',
      isStudentResponseCustom: false,
      selectedOptionIdsByStudent: [
        variant === 'early' ? 41 /* Peer interaction */ : 43 /* Understanding concepts */
      ],

      teacherResponse:
        variant === 'early'
          ? 'Initially struggles with group work and needs encouragement.'
          : 'Excellent collaborator who helps peers and leads group discussions.',
      isTeacherResponseCustom: true,
      selectedOptionIdsByTeacher: undefined
    }
  ];
}

// Build two attempts: one older and one newer
const mockAttempts: Attempt[] = [
  {
    studentCompletedAt: new Date('2025-02-12T10:15:00Z'),
    teacherCompletedAt: new Date('2025-02-12T11:00:00Z'),
    answers: buildAttemptAnswers('early')
  },
  {
    studentCompletedAt: new Date('2025-05-28T09:40:00Z'),
    teacherCompletedAt: new Date('2025-05-28T10:10:00Z'),
    answers: buildAttemptAnswers('late')
  }
];

const mockStudentResultHistory: StudentResultHistory = {
  student: mockStudent,
  teacher: mockTeacher,
  template: mockTemplate,
  attempts: mockAttempts
};

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
  getStudentResultHistory(
    studentId: string,
    templateId: string
  ): Observable<StudentResultHistory> {
    // In a real impl you'd fetch results[] + template and transform,
    // but for now we just return the mock.

    return of(mockStudentResultHistory);
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
}
