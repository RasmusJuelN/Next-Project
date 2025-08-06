import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ResultService } from './services/result.service';
import { Result } from './models/result.model';
import { CommonModule } from '@angular/common';
import { AgCharts } from "ag-charts-angular";
import { AgBarSeriesOptions, AgChartOptions } from "ag-charts-community";
import { RouterModule } from '@angular/router';

@Component({
    selector: 'app-result',
    standalone: true,
    providers: [ResultService],
    imports: [CommonModule, AgCharts, RouterModule],
    templateUrl: './result.component.html',
    template: `<ag-charts [options]="chartOptions" ></ag-charts>`,
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
  const questionMap = new Map<string, Record<string, number>>();

  [data].forEach((result) => {
    result.answers.forEach((answer, index) => {
      const questionKey = `Q${index + 1}`;
      if (!questionMap.has(questionKey)) {
        questionMap.set(questionKey, {});
      }

      const questionData = questionMap.get(questionKey)!;

      // Collect both student and teacher responses
      const responses = [answer.studentResponse, answer.teacherResponse];

      responses.forEach(resp => {
        const trimmed = (resp || '').trim();
        if (!trimmed) return;
        questionData[trimmed] = (questionData[trimmed] || 0) + 1;
      });
    });
  });

  // Build chartData array
  const chartData: any[] = [];
  const uniqueAnswers = new Set<string>();

  questionMap.forEach((answerCounts, question) => {
    const entry: Record<string, any> = { question };
    Object.entries(answerCounts).forEach(([answer, count]) => {
      entry[answer] = count;
      uniqueAnswers.add(answer);
    });
    chartData.push(entry);
  });

  // Build series from unique answer keys
  const series: AgBarSeriesOptions[] = Array.from(uniqueAnswers).map(answer => ({
    type: 'bar',
    xKey: 'question',
    yKey: answer,
    yName: `Svar: ${answer}`,
    stacked: true
  }));

  this.chartOptions = {
    data: chartData,
    title: {
      text: 'Sammenligning af besvarelser',
      fontSize: 18
    },
    series,
    axes: [
      { type: 'category', position: 'bottom', title: { text: 'Spørgsmål' } },
      { type: 'number', position: 'left', title: { text: 'Valgt svar' } }
    ]
    };
  }

}