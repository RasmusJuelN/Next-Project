import { Injectable } from '@angular/core';
import { AppDataService } from './data/app-data.service';
import { catchError, Observable, of } from 'rxjs';
import { ActiveQuestionnaire, Question, userAnswer } from '../models/questionare';
import { MockAuthService } from './auth/mock-auth.service';
import { ErrorHandlingService } from './error-handling.service';

@Injectable({
  providedIn: 'root'
})
export class QuestionareService {
  constructor(
    private appDataService: AppDataService,
    private authService: MockAuthService,
    private errorHandlingService: ErrorHandlingService
  ) {}

  /**
   * Fetches the active questionnaire for the user.
   * @param questionnaireId The ID of the questionnaire.
   * @returns An observable of the active questionnaire.
   */
  getActiveQuestionnaire(questionnaireId: string): Observable<ActiveQuestionnaire | null> {
    return this.appDataService.getActiveQuestionnaireById(questionnaireId).pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to get active questionnaire'))
    );
  }
  /**
   * Fetches the questions for the user.
   * @returns An observable of the questions.
   */
  getQuestionsForUser(): Observable<Question[]> {
    return this.appDataService.getQuestionsForUser().pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to get questions for user'))
    );
  }

  /**
   * Submits the user's answers.
   * @param answers The answers to submit.
   * @returns An observable for the operation.
   */
  submitAnswers(answers: Question[], questionnaireId: string | null): Observable<void> {
    const userId = this.authService.getUserId();
    const role = this.authService.getRole();
    if (role){
      return this.appDataService.submitUserAnswers(userId, role, answers, questionnaireId).pipe(
        catchError(error => this.errorHandlingService.handleError(error, 'Failed to submit answers'))
      );
    } else {
      return this.errorHandlingService.handleError(new Error('Invalid role'), 'Submit Answers');
    }
  }

  /**
   * Validates if the user has access to the questionnaire.
   * @param questionnaireId The ID of the questionnaire.
   * @returns True if the user has access, false otherwise.
   */
  validateUserAccess(questionnaireId: string): Observable<boolean> {
    const userId = this.authService.getUserId();
    const role = this.authService.getRole();
    if (role){
      return this.appDataService.validateUserAccess(userId, role, questionnaireId).pipe(
        catchError(error => this.errorHandlingService.handleError(error, 'Failed to validate user access'))
      );
    }
    return of(false);
  }
}