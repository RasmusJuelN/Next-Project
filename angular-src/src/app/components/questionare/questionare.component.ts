import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Question } from '../../models/questionare';
import { MockDataService } from '../../services/mock-data.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatIconRegistry, MatIconModule } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../services/auth.service';
import { jwtDecode } from 'jwt-decode';

@Component({
  selector: 'app-questionare',
  standalone: true,
  imports: [FormsModule, CommonModule, MatIconModule, MatTooltipModule],
  templateUrl: './questionare.component.html',
  styleUrls: ['./questionare.component.css']
})
/**
 * Represents a component for displaying and interacting with a questionnaire.
 */
export class QuestionareComponent implements OnInit {
  dataService = inject(MockDataService);
  authService = inject(AuthService);
  route = inject(ActivatedRoute);
  router = inject(Router);

  userId: number | null = null;
  role: string | null = null;
  questions: Question[] = [];
  currentQuestionIndex: number = 0;
  activeQuestionnaireId: string | null = null;

  /**
   * Initializes the component and retrieves the questions for the specified user.
   */
  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = jwtDecode(token);
        this.userId = decodedToken.sub;
        this.role = decodedToken.scope;
        this.activeQuestionnaireId = this.route.snapshot.paramMap.get('id');

        if (this.userId && this.role && this.activeQuestionnaireId) {
          this.validateAccess();
        } else {
          console.error('Invalid token data or missing active questionnaire ID');
          this.router.navigate(['/']);
        }
      } catch (error) {
        console.error('Invalid token', error);
        this.router.navigate(['/']);
      }
    } else {
      console.error('No token found');
      this.router.navigate(['/']);
    }
  }

  /**
   * Validates if the user has access to the questionnaire.
   */
  validateAccess(): void {
    this.dataService.getActiveQuestionnaireById(this.activeQuestionnaireId!).subscribe({
      next: (questionnaire) => {
        if (questionnaire) {
          if ((this.role === 'student' && questionnaire.studentId == this.userId && !questionnaire.isStudentFinished) ||
              (this.role === 'teacher' && questionnaire.teacherId == this.userId && !questionnaire.isTeacherFinished)) {
            this.loadQuestions();
          } else {
            console.error('Access denied or questionnaire already finished');
            this.router.navigate(['/']);
          }
        } else {
          console.error('Invalid questionnaire ID');
          this.router.navigate(['/']);
        }
      },
      error: (err) => {
        console.error(err);
        this.router.navigate(['/']);
      }
    });
  }

  /**
   * Loads the questions for the user.
   */
  loadQuestions(): void {
    this.dataService.getQuestionsForUser().subscribe({
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
      this.submit();
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
      if (this.userId && this.activeQuestionnaireId) {
        this.dataService.submitData(this.userId, this.role!, this.activeQuestionnaireId);
        console.log("Data submitted!");
        this.dataService.submitData(this.userId, this.role!, this.activeQuestionnaireId);
        this.router.navigate(['/']);
      } else {
        console.error('Invalid user ID or questionnaire ID');
      }
    } else {
      alert("You did not submit data!");
    }
  }
}
