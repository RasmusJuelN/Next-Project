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
export class QuestionareComponent {
  dataService = inject(MockDataService);
  route = inject(ActivatedRoute);
  private router = inject(Router);

  questions: Question[] = [];
  currentQuestionIndex: number = 0;

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

  selectOption(value: number): void {
    this.questions[this.currentQuestionIndex].selectedOption = value;
  }  

  nextQuestion(): void {
    if (this.currentQuestionIndex < this.questions.length - 1) {
      this.currentQuestionIndex++;
    } else {
      // Submit the answers or navigate to another page
      console.log('Submit answers');
    }
  }

  previousQuestion(): void {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
    }
  }
  submit(){
    let result = confirm("Will you proceed?");
    if (result) {
      // User clicked 'Yes'
      alert("You submited Data!");
    } else {
        // User clicked 'No'
        alert("You did not submit data!");
    }

  }
}