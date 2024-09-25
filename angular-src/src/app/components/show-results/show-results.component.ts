import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DataService } from '../../services/data/data.service';
import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { Answer, AnswerSession, QuestionDetails } from '../../models/questionare';

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
  private router = inject(Router)
  answerSession: AnswerSession | null = null; // Holds the fetched answer session data
  errorMessage: string | null = null;
  questionDetails: QuestionDetails[] = []; // Combined question and option data for both student and teacher

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
        this.answerSession = data.answerSession; // Store the answer session data
        this.questionDetails = data.questionDetails; // Store the combined question and option data
      } else {
        this.errorMessage = 'Results not found for the specified questionnaire.';
      }
    });
  }

  displayAnswer(questionId: number, role: 'student' | 'teacher'): string {
    const questionDetail = this.questionDetails.find(q => q.questionId === questionId);
    if (!questionDetail) return 'No Answer';
  
    return role === 'student' ? questionDetail.studentAnswer : questionDetail.teacherAnswer;
  }


  // Helper function to get the question title by its ID
  getQuestionTitle(questionId: number): string {
    const questionDetail = this.questionDetails.find(q => q.questionId === questionId);
    return questionDetail ? questionDetail.questionTitle : 'Unknown question';
  }
}
