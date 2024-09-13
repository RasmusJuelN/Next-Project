import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ActiveQuestionnaire, Answer, AnswerSession, Question, QuestionTemplate, User } from '../../models/questionare';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private apiUrl = environment.apiUrl; 
  private http = inject(HttpClient);

  // Error handling function
  private handleError<T>(operation = 'operation', result?: T) {
    return (error: any): Observable<T> => {
      console.error(`${operation} failed: ${error.message}`);
      return throwError(() => new Error(`${operation} failed: ${error.message}`));
    };
  }

  // Placeholder for a not implemented method
  getResults(activeQuestionnaireId: string): Observable<{ answerSession: AnswerSession, questionDetails: { questionId: string, title: string, studentOptionLabel: string, teacherOptionLabel: string }[] }> {
    console.error("WIP, NOT YET IMPLEMENTED");
    return throwError(() => new Error('WIP, NOT YET IMPLEMENTED'));
  }

  // Active Questionnaire Methods
  createActiveQuestionnaire(student: User, teacher: User, templateId: string): Observable<ActiveQuestionnaire> {
    const url = `${this.apiUrl}/questionnaire/create`;
    const body = { student, teacher, templateId };
    return this.http.post<ActiveQuestionnaire>(url, body)
      .pipe(
        catchError(this.handleError<ActiveQuestionnaire>('createActiveQuestionnaire'))
      );
  }

  // Search and Pagination for Users
  getUsersFromSearch(role: string, searchQuery: string, page: number = 1, limit: number = 10): Observable<User[]> {
    const params = new HttpParams()
      .set('role', role)
      .set('searchQuery', searchQuery)
      .set('page', page.toString())
      .set('limit', limit.toString());
  
    return this.http.get<User[]>(`${this.apiUrl}/users`, { params })
      .pipe(
        catchError(this.handleError<User[]>('getUsersFromSearch', []))
      );
  }

  // Search and Pagination for Templates
  getTemplatesFromSearch(titleString: string, page: number = 1, limit: number = 10): Observable<QuestionTemplate[]> {
    const params = new HttpParams()
      .set('title', titleString)
      .set('page', page.toString())
      .set('limit', limit.toString());
  
    return this.http.get<QuestionTemplate[]>(`${this.apiUrl}/templates/search`, { params })
      .pipe(
        catchError(this.handleError<QuestionTemplate[]>('getTemplatesFromSearch', []))
      );
  }

  // Template Management Methods
  getTemplates(): Observable<QuestionTemplate[]> {
    const url = `${this.apiUrl}/templates`;
    return this.http.get<QuestionTemplate[]>(url)
      .pipe(
        catchError(this.handleError<QuestionTemplate[]>('getTemplates', []))
      );
  }

  createTemplate(template: QuestionTemplate): Observable<void> {
    const url = `${this.apiUrl}/templates/create`;
    return this.http.post<void>(url, template)
      .pipe(
        catchError(this.handleError<void>('createTemplate'))
      );
  }

  updateTemplate(template: QuestionTemplate): Observable<void> {
    const url = `${this.apiUrl}/templates/update/${template.templateId}`;
    return this.http.put<void>(url, template)
      .pipe(
        catchError(this.handleError<void>('updateTemplate'))
      );
  }

  deleteTemplate(templateId: string): Observable<void> {
    const url = `${this.apiUrl}/templates/delete/${templateId}`;
    return this.http.delete<void>(url)
      .pipe(
        catchError(this.handleError<void>('deleteTemplate'))
      );
  }

  // Dashboard Methods
  getActiveQuestionnairePage(filter: any, page: number, limit: number): Observable<ActiveQuestionnaire[]> {
    const url = `${this.apiUrl}/questionnaire?page=${page}&limit=${limit}`;
    return this.http.post<ActiveQuestionnaire[]>(url, filter)
      .pipe(
        catchError(this.handleError<ActiveQuestionnaire[]>('getActiveQuestionnairePage', []))
      );
  }

  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire | null> {
    const url = `${this.apiUrl}/questionnaire/${id}`;
    return this.http.get<ActiveQuestionnaire | null>(url)
      .pipe(
        catchError(this.handleError<ActiveQuestionnaire | null>('getActiveQuestionnaireById', null))
      );
  }

  // Get questions for a user based on the template
  getQuestionsForUser(templateId: string): Observable<Question[]> {
    const url = `${this.apiUrl}/questions/${templateId}`;
    return this.http.get<Question[]>(url)
      .pipe(
        catchError(this.handleError<Question[]>('getQuestionsForUser', []))
      );
  }

  // Submit user answers
  submitUserAnswers(role: string, answers: Answer[], questionnaireId: string | null): Observable<void> {
    const url = `${this.apiUrl}/questionnaire/submit/${questionnaireId}`;
    const body = { role, answers };
    return this.http.post<void>(url, body)
      .pipe(
        catchError(this.handleError<void>('submitUserAnswers'))
      );
  }

  // Validate user access to a questionnaire
  validateUserAccess(userId: any, role: string, questionnaireId: string): Observable<boolean> {
    const url = `${this.apiUrl}/questionnaire/validate`;
    const body = { userId, role, questionnaireId };
    return this.http.post<boolean>(url, body)
      .pipe(
        catchError(this.handleError<boolean>('validateUserAccess'))
      );
  }
}
