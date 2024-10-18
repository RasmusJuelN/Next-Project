import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { DataService } from '../../services/data/data.service';
import { catchError } from 'rxjs/operators';
import { Observable, of } from 'rxjs';
import { CommonModule } from '@angular/common';
import { AnswerSession } from '../../models/questionare';

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
  private router = inject(Router);
  answerSession: AnswerSession | null = null; // Holds the fetched answer session data
  errorMessage: string | null = null;

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
    ).subscribe((data: AnswerSession | null) => {
      if (data) {
        this.answerSession = data; // Store the answer session data
      } else {
        this.errorMessage = 'Results not found for the specified questionnaire.';
      }
    });
  }
}
