import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ActiveQuestionnaire, Question, QuestionTemplate, User } from '../../models/questionare';
import { environment } from '../../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class DataService {
  private apiUrl = environment.apiUrl; 
  private http = inject(HttpClient);

  // Error handling function
  private handleError(error: any): Observable<never> {
    console.error('An error occurred:', error); // Log error to the console (or send to a logging service)
    return throwError(() => new Error(error.message || 'Something went wrong, please try again later.'));
  }

  // Active Questionnaire Methods
  createActiveQuestionnaire(student: User, teacher: User, templateId: string): Observable<ActiveQuestionnaire> {
    const url = `${this.apiUrl}/questionnaire/create`;
    const body = { student, teacher, templateId };
    return this.http.post<ActiveQuestionnaire>(url, body)
      .pipe(
        catchError(this.handleError)
      );
  }

  getUsersFromSearch(role: string, nameString: string, page: number = 1, limit: number = 10): Observable<User[]> {
    const url = `${this.apiUrl}/users/search?role=${role}&name=${nameString}&page=${page}&limit=${limit}`;
    return this.http.get<User[]>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  getTemplatesFromSearch(titleString: string, page: number = 1, limit: number = 10): Observable<QuestionTemplate[]> {
    const url = `${this.apiUrl}/templates/search?title=${titleString}&page=${page}&limit=${limit}`;
    return this.http.get<QuestionTemplate[]>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  // Template Management Methods
  getTemplates(): Observable<QuestionTemplate[]> {
    const url = `${this.apiUrl}/templates`;
    return this.http.get<QuestionTemplate[]>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  createTemplate(template: QuestionTemplate): Observable<void> {
    const url = `${this.apiUrl}/templates/create`;
    return this.http.post<void>(url, template)
      .pipe(
        catchError(this.handleError)
      );
  }

  updateTemplate(template: QuestionTemplate): Observable<void> {
    const url = `${this.apiUrl}/templates/update/${template.templateId}`;
    return this.http.put<void>(url, template)
      .pipe(
        catchError(this.handleError)
      );
  }

  deleteTemplate(templateId: string): Observable<void> {
    const url = `${this.apiUrl}/templates/delete/${templateId}`;
    return this.http.delete<void>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  // Dashboard Methods
  getActiveQuestionnairePage(filter: any, page: number, limit: number): Observable<ActiveQuestionnaire[]> {
    const url = `${this.apiUrl}/questionnaire?page=${page}&limit=${limit}`;
    return this.http.post<ActiveQuestionnaire[]>(url, filter)
      .pipe(
        catchError(this.handleError)
      );
  }

  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire | null> {
    const url = `${this.apiUrl}/questionnaire/${id}`;
    return this.http.get<ActiveQuestionnaire | null>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  getQuestionsForUser(templateId: string): Observable<Question[]> {
    const url = `${this.apiUrl}/questions/${templateId}`;
    return this.http.get<Question[]>(url)
      .pipe(
        catchError(this.handleError)
      );
  }

  submitUserAnswers(userId: number | null, role: string, answers: Question[], questionnaireId: string | null): Observable<void> {
    const url = `${this.apiUrl}/questionnaire/submit/${questionnaireId}`;
    const body = { userId, role, answers };
    return this.http.post<void>(url, body)
      .pipe(
        catchError(this.handleError)
      );
  }

  validateUserAccess(userId: any, role: string, questionnaireId: string): Observable<boolean> {
    const url = `${this.apiUrl}/questionnaire/validate`;
    const body = { userId, role, questionnaireId };
    return this.http.post<boolean>(url, body)
      .pipe(
        catchError(this.handleError)
      );
  }
}
