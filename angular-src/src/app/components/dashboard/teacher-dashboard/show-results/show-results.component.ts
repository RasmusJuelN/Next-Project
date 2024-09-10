import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DataService } from '../../../../services/data/data.service';
import { catchError, map } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Answer, AnswerSession } from '../../../../models/questionare';

@Component({
  selector: 'app-show-results',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './show-results.component.html',
  styleUrls: ['./show-results.component.css']
})
export class ShowResultsComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private dataService = inject(DataService);

  answerSession: AnswerSession| null = null; // Holds the fetched answer session data
  errorMessage: string | null = null;
  questions = [
    { id: 1, title: 'Indlæringsevne' },
    { id: 2, title: 'Kreativitet og selvstændighed' },
    { id: 3, title: 'Arbejdsindsats' }
  ]; // Mock questions

  ngOnInit(): void {
    const questionnaireId = this.route.snapshot.paramMap.get('id');
    if (questionnaireId) {
      this.loadResults(questionnaireId);
    } else {
      this.errorMessage = 'Questionnaire ID not found in route';
    }
  }

  // Load the results based on the questionnaireId
  loadResults(questionnaireId: string) {
    this.dataService.getResults(questionnaireId).pipe(
      catchError((error) => {
        this.errorMessage = error.message;
        return of(null); // Return null in case of error
      })
    ).subscribe((data) => {
      if (data) {
        this.answerSession = data; // Store the answer session data
      } else {
        this.errorMessage = 'Results not found for the specified questionnaire.';
      }
    });
  }

  // Helper function to get the question title by its ID
  getQuestionTitle(questionId: number): string {
    const question = this.questions.find(q => q.id === questionId);
    return question ? question.title : 'Unknown question';
  }

  displayAnswer(
    answer: Answer, 
  ): Observable<string> {
    
    // Return the custom answer if it exists
    if (answer.customAnswer) {
      return of(answer.customAnswer);
    }
    // Return the label of the selected option if the selectedOptionId is present
    if (answer.selectedOptionId !== undefined && answer.selectedOptionId !== null && this.answerSession?.questionnaireId) {
      return this.dataService.getOptionLabel(this.answerSession.questionnaireId, answer.questionId, answer.selectedOptionId).pipe(
        map((label: string | null) => label ?? 'No Label Found'),
        catchError(error => of('Error: ' + error.message)) // Handle errors by returning a fallback message
      );
    }
  
    // Return a "No Answer" message when no answer is provided
    return of('No Answer');
  }
  
  
  

  // Helper function to compare answers between student and teacher
  showComparison(studentAnswer: any, teacherAnswer: any): string {
    if (studentAnswer.selectedOptionId !== teacherAnswer.selectedOptionId) {
      return 'Both rated differently.';
    }
    if (studentAnswer.customAnswer && teacherAnswer.customAnswer && studentAnswer.customAnswer !== teacherAnswer.customAnswer) {
      return 'Both provided different feedback.';
    }
    return 'Both answered the same.';
  }
}
