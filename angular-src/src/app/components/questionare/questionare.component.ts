import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActiveQuestionnaire, Question } from '../../models/questionare';
import { ActivatedRoute, Router } from '@angular/router';
import { MatIconRegistry, MatIconModule } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { MatTooltipModule } from '@angular/material/tooltip';
import { jwtDecode } from 'jwt-decode';
import { MockAuthService } from '../../services/auth/mock-auth.service';
import { LoadingComponent } from '../loading/loading.component';
import { LocalStorageService } from '../../services/misc/local-storage.service';
import { QuestionareService } from '../../services/questionare.service';

@Component({
  selector: 'app-questionare',
  standalone: true,
  imports: [FormsModule, CommonModule, MatIconModule, MatTooltipModule,LoadingComponent],
  templateUrl: './questionare.component.html',
  styleUrls: ['./questionare.component.css']
})
/**
 * Represents a component for displaying and interacting with a questionnaire.
 */
export class QuestionareComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private questionareService = inject(QuestionareService);

  currentQuestionIndex: number = 0;
  activeQuestionnaireId: string | null = null;
  questions: Question[] = [];
  activeQuestionnaire: ActiveQuestionnaire | null = null;
  errorMessage: string | null = null;
  isLoading: boolean = true;

  ngOnInit(): void {
    const questionnaireId = this.route.snapshot.paramMap.get('id');

    if (questionnaireId) {
      this.loadQuestionnaireData(questionnaireId);
    } else {
      this.errorMessage = 'Invalid questionnaire ID';
      this.isLoading = false;
    }
  }

  /**
   * Loads the questionnaire data.
   * @param questionnaireId The ID of the questionnaire.
   */
  private loadQuestionnaireData(questionnaireId: string): void {
    this.questionareService.validateUserAccess(questionnaireId).subscribe({
      next: (hasAccess) => {
        if (hasAccess) {
          this.questionareService.getActiveQuestionnaire(questionnaireId).subscribe({
            next: (questionnaire) => {
              this.activeQuestionnaire = questionnaire;
              if (this.activeQuestionnaire) {
                this.loadQuestions();
              } else {
                this.errorMessage = 'Questionnaire not found';
                this.isLoading = false;
              }
            },
            error: () => {
              this.errorMessage = 'Error loading questionnaire';
              this.isLoading = false;
            }
          });
        } else {
          this.errorMessage = 'Access denied or questionnaire already finished';
          this.router.navigate(['/']);
        }
      },
      error: () => {
        this.errorMessage = 'Error validating access';
        this.isLoading = false;
      }
    });
  }

  /**
   * Loads the questions for the questionnaire.
   */
  private loadQuestions(): void {
    this.questionareService.getQuestionsForUser().subscribe({
      next: (questions) => {
        this.questions = questions;
        this.isLoading = false;
      },
      error: () => {
        this.errorMessage = 'Error loading questions';
        this.isLoading = false;
      }
    });
  }

  nextQuestion(): void {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.currentQuestionIndex++;
    }
  }

  previousQuestion(): void {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
    }
  }

  hasSelectedOption(): boolean {
    return this.questions[this.currentQuestionIndex].selectedOption !== undefined;
  }

  selectOption(value: any): void {
    this.questions[this.currentQuestionIndex].selectedOption = value;
  }

  /**
   * Handles form submission.
   * @param answers The answers to submit.
   */
  submit(): void {
    if (this.activeQuestionnaire) {
      if(confirm("Will you proceed?")){
        this.questionareService.submitAnswers(this.questions, this.activeQuestionnaire.id).subscribe({
          next: () => {
            this.router.navigate(['/']);
          },
          error: () => {
            this.errorMessage = 'Error submitting answers';
          }
        });
      }
    }
  }
}
