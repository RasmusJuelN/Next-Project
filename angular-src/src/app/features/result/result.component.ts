import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ResultService } from './services/result.service';
import { Result } from './models/result.model';
import { CommonModule, DatePipe } from '@angular/common';

@Component({
  selector: 'app-result',
  standalone: true,
  imports: [CommonModule,DatePipe],
  templateUrl: './result.component.html',
  styleUrl: './result.component.css'
})
export class ResultComponent implements OnInit {
  result: Result | null = null;
  isLoading = true;
  errorMessage: string | null = null;

  constructor(private route: ActivatedRoute, private resultService: ResultService) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    if (id) {
      this.fetchResult(id);
    } else {
      this.errorMessage = 'Invalid result ID.';
      this.isLoading = false;
    }
  }

  fetchResult(id: string): void {
    this.resultService.getResultById(id).subscribe({
      next: (data) => {
        if (data) {
          this.result = data;
        } else {
          this.errorMessage = 'Result not found.';
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Failed to load result.';
        console.error(err);
        this.isLoading = false;
      },
    });
  }
}
