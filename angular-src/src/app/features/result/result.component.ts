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
      // Tjek om resultatet kan tilgås inden det hentes
      this.resultService.canGetResult(id).subscribe({
        next: (canGet: boolean) => {
          if (canGet) {
            this.fetchResult(id);
          } else {
            this.errorMessage = 'Resultatet er ikke tilgængeligt eller ikke fuldført.';
            this.isLoading = false;
          }
        },
        error: (err) => {
          console.error(err);
          this.errorMessage = 'Fejl ved kontrol af resultatadgang.';
          this.isLoading = false;
        }
      });
    } else {
      this.errorMessage = 'Ugyldigt resultat-ID.';
      this.isLoading = false;
    }
  }

  fetchResult(id: string): void {
    this.resultService.getResultById(id).subscribe({
      next: (data: Result) => {
        if (data) {
          this.result = data;
        } else {
          this.errorMessage = 'Resultat ikke fundet.';
        }
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.errorMessage = 'Resultat ikke fundet.';
        this.isLoading = false;
      },
    });
  }
  printPage(): void {
    window.print();
  }
  
}
