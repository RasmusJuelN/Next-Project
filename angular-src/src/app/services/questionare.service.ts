import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { ActiveQuestionnaire, Answer, Question, QuestionnaireMetadata } from '../models/questionare';
import { MockAuthService } from './auth/mock-auth.service';
import { ErrorHandlingService } from './error-handling.service';
import { DataService } from './data/data.service';

@Injectable({
  providedIn: 'root',
})
export class QuestionnaireService {
  private metadataSubject = new BehaviorSubject<QuestionnaireMetadata | null>(null);
  private questionsSubject = new BehaviorSubject<Question[]>([]);
  private activeQuestionnaire: ActiveQuestionnaire | null = null;

  constructor(
    private dataService: DataService,
    private authService: MockAuthService,
    private errorHandlingService: ErrorHandlingService
  ) {}

  /**
   * Loads the active questionnaire for the user by ID.
   * @param questionnaireId The ID of the questionnaire.
   * @returns An observable of the active questionnaire.
   */
  loadQuestionnaireData(questionnaireId: string): Observable<ActiveQuestionnaire | null> {
    return this.getActiveQuestionnaire(questionnaireId);
  }

  /**
   * Fetches the active questionnaire for the user.
   * @param questionnaireId The ID of the questionnaire.
   * @returns An observable of the active questionnaire.
   */
  getActiveQuestionnaire(questionnaireId: string): Observable<ActiveQuestionnaire | null> {
    return this.dataService.getActiveQuestionnaireById(questionnaireId).pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to get active questionnaire'))
    );
  }

  /**
   * Initializes questions and metadata for the given questionnaire.
   * @param questionnaire The active questionnaire.
   */
  initializeQuestionsAndMetadata(questionnaire: ActiveQuestionnaire): void {
    this.activeQuestionnaire = questionnaire;
    this.getQuestionsForUser(questionnaire.questionnaireTemplate.templateId).subscribe((questions) => {
      const metadata = this.initializeMetadata(questions, questionnaire.id);
      this.metadataSubject.next(metadata);
      this.questionsSubject.next(questions);
    });
  }

  /**
   * Fetches the questions for the given template ID.
   * @param templateId The ID of the question template.
   * @returns An observable of the questions.
   */
  getQuestionsForUser(templateId: string): Observable<Question[]> {
    return this.dataService.getQuestionsForUser(templateId).pipe(
      catchError(error => this.errorHandlingService.handleError(error, 'Failed to get questions for user'))
    );
  }

  /**
   * Initializes the metadata for the questionnaire.
   * @param questions The list of questions.
   * @param questionnaireId The ID of the questionnaire.
   * @returns The initialized questionnaire metadata.
   */
  private initializeMetadata(questions: Question[], questionnaireId: string): QuestionnaireMetadata {
    const totalQuestions = questions.length;
    const currentIndex = 0;
    const progress = ((currentIndex + 1) / totalQuestions) * 100;

    return {
      questionnaireId,
      totalQuestions,
      questionIds: questions.map(q => q.id.toString()),
      currentIndex,
      progress,
    };
  }

  /**
   * Returns an observable for the questionnaire metadata.
   * @returns Observable of questionnaire metadata.
   */
  getMetadata(): Observable<QuestionnaireMetadata | null> {
    return this.metadataSubject.asObservable();
  }

  /**
   * Returns an observable for the list of questions.
   * @returns Observable of questions.
   */
  getQuestions(): Observable<Question[]> {
    return this.questionsSubject.asObservable();
  }

  /**
   * Moves to the next question in the questionnaire.
   */
  nextQuestion(): void {
    const metadata = this.metadataSubject.value;
    if (metadata && metadata.currentIndex < metadata.totalQuestions - 1) {
      metadata.currentIndex++;
      metadata.progress = ((metadata.currentIndex + 1) / metadata.totalQuestions) * 100;
      this.metadataSubject.next(metadata);
    }
  }

  /**
   * Moves to the previous question in the questionnaire.
   */
  previousQuestion(): void {
    const metadata = this.metadataSubject.value;
    if (metadata && metadata.currentIndex > 0) {
      metadata.currentIndex--;
      metadata.progress = ((metadata.currentIndex + 1) / metadata.totalQuestions) * 100;
      this.metadataSubject.next(metadata);
    }
  }

  /**
   * Selects an option for the current question.
   * @param value The value of the selected option.
   */
  selectOption(optionData:{ optionId: number | undefined, customAnswer?: string }): void {
    const metadata = this.metadataSubject.value;
    if (metadata) {
      const questions = this.questionsSubject.value;
      const currentQuestion = questions[metadata.currentIndex];
  
      if (typeof optionData.optionId === 'number') {
        // Handle predefined option selected (optionId)
        currentQuestion.selectedOption = optionData.optionId; // Update selectedOption ID
        currentQuestion.customAnswer = undefined; // Clear custom answer if option is selected
      } else if (typeof optionData.customAnswer === 'string') {
        // Handle custom answer input
        currentQuestion.customAnswer = optionData.customAnswer; // Update custom answer
        currentQuestion.selectedOption = undefined; // Clear selected option ID if custom answer is used
      }
  
      // Push the updated question list to the subject
      this.questionsSubject.next(questions);
    }
  }

  /**
   * Submits the user's answers.
   * @returns An observable for the operation.
   */
  submitAnswers(): Observable<void> {
    if (this.activeQuestionnaire) {
      const questionnaireId = this.activeQuestionnaire.id;
      const role = this.authService.getUserRole();

      if (role) {
        const answers = this.createAnswers(); // Generate the answers

        return this.dataService.submitUserAnswers(role, answers, questionnaireId).pipe(
          catchError(error => this.errorHandlingService.handleError(error, 'Failed to submit answers'))
        );
      } else {
        return this.errorHandlingService.handleError(new Error('Invalid role'), 'Submit Answers');
      }
    } else {
      return new Observable<void>((observer) => {
        observer.error('No active questionnaire found');
      });
    }
  }

  private createAnswers(): Answer[] {
    const questions = this.questionsSubject.value;
    const answers: Answer[] = [];

    // Iterate through each question and generate the Answer objects
    questions.forEach((question) => {
      if (question.selectedOption) {
        const answer: Answer = {
          questionId: question.id,
          selectedOptionId: question.selectedOption, // Assuming selectedOption is the ID of the selected option
        };
        answers.push(answer);
      } else if (question.customAnswer) {
        const answer: Answer = {
          questionId: question.id,
          customAnswer: question.customAnswer, // Handling custom answers
        };
        answers.push(answer);
      }
    });

    return answers;
  }
}
