import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ResultService } from './services/result.service';
import { Result } from './models/result.model';
import { CommonModule } from '@angular/common';
import { AgChartsModule } from 'ag-charts-angular';
import { RouterModule } from '@angular/router';
import { AgChartOptions } from 'ag-charts-community';

@Component({
    selector: 'app-result',
    standalone: true,
    providers: [ResultService],
    imports: [CommonModule, AgChartsModule, RouterModule],
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

 
  printPage(): void {
    window.print();
  }
  
    chartOptions: AgChartOptions | null = null;

  fetchResult(id: string): void {
    this.resultService.getResultById(id).subscribe({
      next: (data: Result) => {
        if (data) {
          this.result = data;
          this.generateChartOptions(data); // <- generate chart
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

  generateChartOptions(data: Result): void {
    const chartData = data.answers.map((answer, index) => ({
      question: `Q${index + 1}`,
      studentScore: answer.studentResponse ?? 0,
      teacherScore: answer.teacherResponse ?? 0,
    }));

    this.chartOptions = {
      data: chartData,
      title: { text: 'Sammenligning af besvarelser', fontSize: 18 },
      series: [
      {
        type: 'bar',
        xKey: 'question',
        yKey: 'studentScore',
        yName: 'Elev',
      },
      {
        type: 'bar',
        xKey: 'question',
        yKey: 'teacherScore',
        yName: 'Lærer',
      }
      ],
      axes: [
      {
        type: 'category',
        position: 'bottom',
        title: { text: 'Spørgsmål' }
      },
      {
        type: 'number',
        position: 'left',
        title: { text: 'Score' }
      }
      ]
    };
  }

}