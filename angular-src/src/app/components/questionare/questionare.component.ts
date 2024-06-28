import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { ActiveQuestionnaire, Question } from '../../models/questionare';
import { MockDataService } from '../../services/mock-data.service';
import { ActivatedRoute, Router } from '@angular/router';
import { MatIconRegistry, MatIconModule } from '@angular/material/icon';
import { DomSanitizer } from '@angular/platform-browser';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AuthService } from '../../services/auth.service';
import { jwtDecode } from 'jwt-decode';
import { MockAuthService } from '../../services/mock-auth.service';

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
  authService = inject(MockAuthService);
  route = inject(ActivatedRoute);
  router = inject(Router);

  userId: number | null = null;
  role: string | null = null;
  questions: Question[] = [];
  currentQuestionIndex: number = 0;
  activeQuestionnaireId: string | null = null;

  /**
   * Redirects to [/] home route and logs an error message.
   * @param message - A string error message to display.
   */
  private redirectToHomeErr(message: string): void {
    alert(message); // Display an alert message, comment out if not testing
    this.router.navigate(['/']);
  }

  /**
   * Initializes the component and retrieves the questions for the specified user.
   */
  ngOnInit(): void {
    const token = localStorage.getItem('token');
    if (token) {
      this.initializeUserFromToken(token);
    } else {
      this.redirectToHomeErr('Access denied or questionnaire already finished');
    }
  }

  private initializeUserFromToken(token: string): void {
    const user = this.authService.getUserFromToken(token);
    if (user) {
      this.userId = user.userId;
      this.role = user.role;
      this.activeQuestionnaireId = this.route.snapshot.paramMap.get('id');
      if (this.activeQuestionnaireId) {
        this.validateAccess();
      } else {
        this.redirectToHomeErr('Missing active questionnaire ID');
      }
    } else {
      this.redirectToHomeErr('Invalid token data');
    }
  }

  /**
   * Checks if the user is authorized to access the questionnaire.
   * @param questionnaire - The active questionnaire.
   * @returns True if the user is authorized, false otherwise.
   */
  private isAuthorizedUser(questionnaire: ActiveQuestionnaire) {
    return (this.role === 'student' && questionnaire.studentId == this.userId && !questionnaire.isStudentFinished) ||
           (this.role === 'teacher' && questionnaire.teacherId == this.userId && !questionnaire.isTeacherFinished);
  }

  /**
   * Validates if the user has access to the questionnaire.
   */
  private validateAccess(): void {
    this.dataService.getActiveQuestionnaireById(this.activeQuestionnaireId!).subscribe({
      next: (questionnaire) => {
        if (questionnaire && this.isAuthorizedUser(questionnaire)) {
          this.loadQuestions();
        } else {
          this.redirectToHomeErr('Access denied or questionnaire already finished');
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
  private loadQuestions(): void {
    this.dataService.getQuestionsForUser().subscribe({
      next: questions => this.questions = questions,
      error: () => this.redirectToHomeErr('Error loading questions')
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
   * If there are no more questions, submits the answers or navigates to next question.
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
    if (confirm("Will you proceed?")) {
      if (this.userId && this.activeQuestionnaireId) {
        this.dataService.submitData(this.userId, this.role!, this.activeQuestionnaireId).subscribe({
          next: () => {
            console.log("Data submitted!");
            this.router.navigate(['/']);
          },
          error: (err) => {
            console.error('Error submitting data:', err);
          }
        });
      } else {
        console.error('Invalid user ID or questionnaire ID');
      }
    } else {
      alert("You did not submit data!");
    }
  }
}
