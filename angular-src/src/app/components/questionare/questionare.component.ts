import { Component, inject, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { BehaviorSubject, Observable, of } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { QuestionnaireMetadata, Question, ActiveQuestionnaire, Answer } from '../../models/questionare';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { LoadingComponent } from '../loading/loading.component';
import { DataService } from '../../services/data/data.service';
import { ErrorHandlingService } from '../../services/error-handling.service';
import { AuthService } from '../../services/auth/auth.service';
import { QuestionComponent } from './question/question.component';


// REMEMBER TO FIX ISSUE WITH ALWAYS HAVING ACCESS
@Component({
  selector: 'app-questionare',
  standalone: true,
  imports: [FormsModule, CommonModule, MatIconModule, LoadingComponent, QuestionComponent],
  templateUrl: './questionare.component.html',
  styleUrls: ['./questionare.component.css']
})
export class QuestionareComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private dataService = inject(DataService);
  private authService = inject(AuthService);
  private errorHandlingService = inject(ErrorHandlingService);

  private metadataSubject = new BehaviorSubject<QuestionnaireMetadata | null>(null);
  private questionsSubject = new BehaviorSubject<Question[]>([]);
  private activeQuestionnaire: ActiveQuestionnaire | null = null;

  metadata: QuestionnaireMetadata | null = null;
  questions: Question[] = [];
  errorMessage: string | null = null;
  isLoading: boolean = true;

  ngOnInit(): void {
    this.authService.isAuthenticated$.subscribe((isAuthenticated: boolean) => {
      if (!isAuthenticated) {
        this.router.navigate(['/']);
      } else {
        const questionnaireId = this.route.snapshot.paramMap.get('id');
        if (questionnaireId) {
          this.loadQuestionnaireData(questionnaireId);
        } else {
          this.errorMessage = 'Invalid questionnaire ID';
          this.isLoading = false;
        }
      }
    });
  }

  private loadQuestionnaireData(questionnaireId: string): void {
    this.dataService.getActiveQuestionnaireById(questionnaireId).pipe(
      catchError(error => {
        this.errorMessage = 'Failed to load questionnaire';
        this.isLoading = false;
        return of(null);
      })
    ).subscribe((questionnaire: ActiveQuestionnaire | null) => {
      if (questionnaire) {
        this.initializeQuestionsAndMetadata(questionnaire);
      } else {
        this.errorMessage = 'Questionnaire not found';
        this.isLoading = false;
      }
    });
  }
  private initializeQuestionsAndMetadata(questionnaire: ActiveQuestionnaire): void {
    this.activeQuestionnaire = questionnaire;
    this.dataService.getQuestionsForUser(questionnaire.template.id).pipe(
      catchError(error => {
        this.errorMessage = 'Failed to load questions';
        this.isLoading = false;
        return of([]);
      })
    ).subscribe((questions: Question[]) => {
      this.metadata = this.initializeMetadata(questions, questionnaire.id);
      this.metadataSubject.next(this.metadata);
      this.questionsSubject.next(questions);
      this.questions = questions;
      this.isLoading = false;
    });
  }

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

  nextQuestion(): void {
    if (this.metadata && this.metadata.currentIndex < this.metadata.totalQuestions - 1) {
      this.metadata.currentIndex++;
      this.metadata.progress = ((this.metadata.currentIndex + 1) / this.metadata.totalQuestions) * 100;
      this.metadataSubject.next(this.metadata);
    }
  }

  previousQuestion(): void {
    if (this.metadata && this.metadata.currentIndex > 0) {
      this.metadata.currentIndex--;
      this.metadata.progress = ((this.metadata.currentIndex + 1) / this.metadata.totalQuestions) * 100;
      this.metadataSubject.next(this.metadata);
    }
  }

  selectOption(optionData: { optionId: number | undefined; customAnswer?: string }): void {
    if (this.metadata) {
      const currentQuestion = this.questions[this.metadata.currentIndex];
      if (typeof optionData.optionId === 'number') {
        currentQuestion.selectedOption = optionData.optionId;
        currentQuestion.customAnswer = undefined;
      } else if (typeof optionData.customAnswer === 'string') {
        currentQuestion.selectedOption = undefined;
        currentQuestion.customAnswer = optionData.customAnswer;
      } else {
        currentQuestion.selectedOption = undefined;
        currentQuestion.customAnswer = undefined;
      }
      this.questionsSubject.next(this.questions);
    }
  }
  

  submit(): void {
    if (this.activeQuestionnaire) {
      const userId = this.authService.getUserId();
      if (userId) {
        const answers = this.createAnswers();
        console.log(userId,this.activeQuestionnaire.id, answers)
        this.dataService.submitUserAnswers(userId, answers, this.activeQuestionnaire.id).pipe(
          catchError(error => {
            this.errorMessage = 'Failed to submit answers';
            return of(null);
          })
        ).subscribe(() => {
          this.router.navigate(['/']);
        });
      } else {
        this.errorMessage = 'Error submitting answers: Invalid user ID';
      }
    } else {
      this.errorMessage = 'No active questionnaire found';
    }
  }
  private createAnswers(): Answer[] {
    return this.questions.map((question) => {
      if (question.selectedOption !== undefined) {
        return { questionId: question.id, selectedOptionId: question.selectedOption };
      } else if (question.customAnswer !== undefined) {
        return { questionId: question.id, customAnswer: question.customAnswer };
      }
      return null;
    }).filter(answer => answer !== null) as Answer[];
  }

  hasSelectedOption(): boolean {
    if (!this.metadata) {
      return false;
    }
    const currentQuestion = this.questions[this.metadata.currentIndex];
    if (currentQuestion) {
      if (currentQuestion.selectedOption !== undefined) {
        // A standard option is selected
        return true;
      } else if (
        currentQuestion.customAnswer !== undefined &&
        currentQuestion.customAnswer.trim().length > 0
      ) {
        // A custom answer is provided
        return true;
      }
    }
    return false;
  }
}
