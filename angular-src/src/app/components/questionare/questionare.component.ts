import { Component, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Question } from '../../models/questionare';
import { MockDataService } from '../../services/mock-data.service';
import { ActivatedRoute, Router } from '@angular/router';
import {MatIconRegistry, MatIconModule} from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import {MatTooltipModule} from '@angular/material/tooltip';

@Component({
  selector: 'app-questionare',
  standalone: true,
  imports: [FormsModule, CommonModule, MatIconModule,MatTooltipModule],
  templateUrl: './questionare.component.html',
  styleUrl: './questionare.component.css'
})
/**
 * Represents a component for displaying and interacting with a questionnaire.
 */
export class QuestionareComponent {
  dataService = inject(MockDataService);
  route = inject(ActivatedRoute);
  private router = inject(Router);

  questions: Question[] = [];
  currentQuestionIndex: number = 0;

  /**
   * Initializes the component and retrieves the questions for the specified user.
   */
  ngOnInit(): void {
    const userId = Number(this.route.snapshot.paramMap.get('userId'));
    if (isNaN(userId)) {
      console.error('Invalid user ID');
      this.router.navigate(['/']);
      return;
    }

    this.dataService.getQuestionsForUser(userId).subscribe({
      next: questions => {
        this.questions = questions;
      },
      error: err => {
        console.error(err);
        this.router.navigate(['/']);
      }
    });
  }

  /**
   * Selects an option for the current question.
   * @param value - The value of the selected option.
   */
  selectOption(value: number): void {
    this.questions[this.currentQuestionIndex].selectedOption = value;
  }  

  /**
   * Moves to the next question.
   * If there are no more questions, submits the answers or navigates to another page.
   */
  nextQuestion(): void {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.currentQuestionIndex++;
    } else {
      // Submit the answers or navigate to another page
      console.log('Submit answers');
    }
  }

  /**
   * Moves to the previous question.
   */
  previousQuestion(): void {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
    }
  }

  /**
   * Submits the answers.
   * Displays a confirmation dialog and shows an alert based on the user's choice.
   */
  submit(): void {
    let result = confirm("Will you proceed?");
    if (result) {
      // User clicked 'Yes'
      alert("You submitted Data!");
    } else {
      // User clicked 'No'
      alert("You did not submit data!");
    }
  }
}