import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ResultService } from './services/result.service';
import { Result } from './models/result.model';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-result',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './result.component.html',
  styleUrls: ['./result.component.css']
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
      next: (data: Result) => {
        if (data) {
          this.result = data;
        } else {
          this.errorMessage = 'Result not found.';
        }
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Result not found.';
        console.error(err);
        this.isLoading = false;
      },
    });
  }
}
