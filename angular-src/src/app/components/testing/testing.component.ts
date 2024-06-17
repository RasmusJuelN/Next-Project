import { Component, inject } from '@angular/core';
import { Question } from '../../models/questionare';
import { DataService } from '../../services/data.service';
import { CommonModule } from '@angular/common';
import { Chart } from 'chart.js';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-testing',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './testing.component.html',
  styleUrl: './testing.component.css'
})
export class TestingComponent {
  question?: Question;
  chart: Chart | null = null;
  route = inject(ActivatedRoute);
  private router = inject(Router);
  
  constructor(private dataService: DataService) {}
  
  ngOnInit(): void {
    const userId = Number(this.route.snapshot.paramMap.get('questionareId'));
    console.log(userId);
    if (isNaN(userId)) {
      console.error('Invalid user ID');
      this.router.navigate(['/']);
      return;
    }

    this.getQuestion(userId); // Fetch the question with ID 1 for testing
  }

  
  getQuestion(id: number): void {
    this.dataService.getQuestionFromId(id).subscribe((question) => {
      this.question = question;
    });
  }
}
