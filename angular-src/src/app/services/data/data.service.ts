import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of, throwError } from 'rxjs';
import { catchError, map, tap } from 'rxjs/operators';
import { ActiveQuestionnaire, Answer, AnswerSession, Question, AnswerDetails, QuestionTemplate, User } from '../../models/questionare';
import { environment } from '../../../environments/environment';
import { LogEntry, LogFileType } from '../../models/log-models';

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

  getLogFileTypes(): Observable<LogFileType> {
    return this.http
      .get<LogFileType>(`${this.apiUrl}/logs/get-available`)
      .pipe(catchError(this.handleError<LogFileType>('getLogFileTypes', {})));
  }

  getLogs(
    logSeverity: string,
    logFileType: string,
    lineCount: number | null,
    reverse: boolean
  ): Observable<LogEntry[]> {
    let params = new HttpParams()
      .set('logName', logFileType)
      .set('order', reverse ? 'desc' : 'asc')
      .set('severity', logSeverity);

    if (lineCount !== null && lineCount > 0) {
      params = params.set('amount', lineCount.toString());
    }
    
    return this.http
      .get<LogEntry[]>(`${this.apiUrl}/logs/get`, { params })
      .pipe(catchError(this.handleError<LogEntry[]>('getLogs', [])));
  }
  
  
  
  
  // Fetch the settings data from the backend
  getSettings(): Observable<any> {
    return this.http.get<any>(`${this.apiUrl}/settings/get`);
  }

  // Update the settings data by sending it to the backend
  updateSettings(updatedSettings: any): Observable<any> {
    return this.http.put(`${this.apiUrl}/settings/update`, updatedSettings);
  }



  getResults(activeQuestionnaireId: string): Observable<AnswerSession> {
    const url = `${this.apiUrl}/questionnaire/results/${activeQuestionnaireId}`;
    return this.http.get<AnswerSession>(url)
      .pipe(
        catchError(this.handleError<AnswerSession>('getResults'))
      );
  }
  

  // Active Questionnaire Methods
  createActiveQuestionnaire(student: User, teacher: User, id: string): Observable<any> {
    const url = `${this.apiUrl}/questionnaire/create`;
    const body = { student, teacher, id };
    return this.http.post(url, body)
      .pipe(
        catchError(this.handleError('createActiveQuestionnaire'))
      );
  }

  // Search and Pagination for Users
  getUsersFromSearch(role: string, searchQuery: string, page: number = 1, limit: number = 10): Observable<{ users: User[] }> {
    const params = new HttpParams()
      .set('role', role)
      .set('searchQuery', searchQuery)
      .set('page', page.toString())
      .set('limit', limit.toString());
  
    return this.http.get<{ users: User[] }>(`${this.apiUrl}/users/search`, { params })
      .pipe(
        catchError(this.handleError<{ users: User[] }>('getUsersFromSearch', { users: [] }))
      );
  }
  


  getTemplates(page: number = 1, limit: number = 10, titleString?: string): Observable<QuestionTemplate[]> {
    let params = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());
  
    if (titleString) {
      params = params.set('title', titleString);
    }
  
    return this.http.get<QuestionTemplate[]>(`${this.apiUrl}/templates/query`, { params })
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
    const url = `${this.apiUrl}/templates/update/${template.id}`;
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
    let params = new HttpParams()
      .set('page', page.toString())
      .set('limit', limit.toString());
  
    // Dynamically add filter properties to query parameters
    Object.keys(filter).forEach(key => {
      if (filter[key] !== null && filter[key] !== undefined) {
        params = params.set(key, filter[key].toString());
      }
    });
  
    const url = `${this.apiUrl}/questionnaire/query`;
    return this.http.get<ActiveQuestionnaire[]>(url, { params })
      .pipe(
        catchError(this.handleError<ActiveQuestionnaire[]>('getActiveQuestionnairePage', []))
      );
  }

  // Updated method to check for any active questionnaires for a user
  checkForActiveQuestionnaires(userId: string): Observable<string | null> {
    const url = `${this.apiUrl}/questionnaire/active/check/${userId}`;
    return this.http.get<string|null>(url)
    .pipe(
      catchError(this.handleError<string | null>('checkForActiveQuestionnaires'))
    );
  }


  // Method to retrieve an active questionnaire by its unique ID
  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire> {
    const url = `${this.apiUrl}/questionnaire/active/${id}`;
    return this.http.get<ActiveQuestionnaire>(url).pipe(
      catchError(this.handleError<ActiveQuestionnaire>('getActiveQuestionnaireById'))
    );
  }


  // Get questions for a user based on the template
  getQuestionsForUser(templateId: string): Observable<Question[]> {
    const url = `${this.apiUrl}/templates/get/${templateId}`;
    return this.http.get<Question[]>(url).pipe(
      map((response: any) => {
        // Ensure that the response contains a valid array of questions
        if (response && Array.isArray(response.questions)) {
          return response.questions as Question[];
        } else {
          console.warn(`Unexpected response format:`, response);
          return []; // Return an empty array if the response is not in the expected format
        }
      }),
      catchError(error => {
        console.error(`Error fetching questions for template ${templateId}:`, error);
        return of([]); // Return an empty array on error
      })
    );
  }

  // Submit user answers
  submitUserAnswers(userId: string, answers: Answer[], questionnaireId: string): Observable<void> {
    const url = `${this.apiUrl}/questionnaire/answers/submit`;
    const body = { userId, questionnaireId, answers };
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
