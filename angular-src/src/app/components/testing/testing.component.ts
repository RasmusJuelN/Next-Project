import { Component } from '@angular/core';
import { Question } from '../../../models/questionare';
import { DataService } from '../../services/data.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-testing',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './testing.component.html',
  styleUrl: './testing.component.css'
})
export class TestingComponent {
  question?: Question;
  constructor(private dataService: DataService) {}
  ngOnInit(): void {
    this.getQuestion(1); // Fetch the question with ID 1 for testing
  }
  getQuestion(id: number): void {
    this.dataService.getQuestionFromId(id).subscribe((question) => {
      this.question = question;
    });
  }
}
