import { Component, inject, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { QuestionnaireService } from '../../services/questionare.service';
import { QuestionnaireMetadata, Question } from '../../models/questionare';

import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { MatIconModule } from '@angular/material/icon';
import { LoadingComponent } from '../loading/loading.component';

@Component({
  selector: 'app-questionare',
  standalone: true,
  imports: [FormsModule, CommonModule, MatIconModule, LoadingComponent],
  templateUrl: './questionare.component.html',
  styleUrls: ['./questionare.component.css']
})
export class QuestionareComponent implements OnInit {
  private router = inject(Router);
  private route = inject(ActivatedRoute);
  private questionnaireService = inject(QuestionnaireService);  // Updated service injection

  metadata: QuestionnaireMetadata | null = null;
  questions: Question[] = [];
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

  private loadQuestionnaireData(questionnaireId: string): void {
    this.questionnaireService.loadQuestionnaireData(questionnaireId).subscribe({
      next: (questionnaire) => {
        if (questionnaire) {
          this.questionnaireService.initializeQuestionsAndMetadata(questionnaire);
          this.subscribeToData();
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
  }

  private subscribeToData(): void {
    this.questionnaireService.getMetadata().subscribe(metadata => {
      this.metadata = metadata;
    });

    this.questionnaireService.getQuestions().subscribe(questions => {
      this.questions = questions;
      this.isLoading = false;
    });
  }

  nextQuestion(): void {
    this.questionnaireService.nextQuestion();
  }

  previousQuestion(): void {
    this.questionnaireService.previousQuestion();
  }

  selectOption(value: any): void {
    this.questionnaireService.selectOption(value);
  }

  hasSelectedOption(): boolean {
    if (!this.metadata) {
      return false;
    }
  
    const currentQuestion = this.questions[this.metadata.currentIndex];
    return currentQuestion && currentQuestion.selectedOption !== undefined;
  }

  submit(): void {
    this.questionnaireService.submitAnswers().subscribe({
      next: () => {
        this.router.navigate(['/']);
      },
      error: () => {
        this.errorMessage = 'Error submitting answers';
      }
    });
  }
}
