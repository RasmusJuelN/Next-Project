import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { ResultService } from './services/result.service';
import { Result } from './models/result.model';
import { CommonModule } from '@angular/common';
import { AgCharts } from "ag-charts-angular";
import { AgBarSeriesOptions, AgChartOptions } from "ag-charts-community";
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';

@Component({
    selector: 'app-result',
    standalone: true,
    providers: [ResultService],
    imports: [CommonModule, AgCharts, RouterModule, FormsModule],
    templateUrl: './result.component.html',
    template: `
      <button (click)="updateChart('stacked')">Stacked</button>
      <button (click)="updateChart('donut')">Donut</button>
      <ag-charts-angular *ngIf="chartOptions" [options]="chartOptions"></ag-charts-angular>
    `,
    styleUrls: ['./result.component.css']
})
export class ResultComponent implements OnInit {
  result: Result | null = null;
  isLoading = true;
  errorMessage: string | null = null;

  chartType: 'stacked' | 'donut' = 'stacked';
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
          this.updateChart(this.chartType, data); 
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

  updateChart(type: 'stacked' | 'donut', dataOverride?: Result): void {
    this.chartType = type;
    const data = dataOverride || this.result;
    if (!data) return;
    if (type === 'stacked') {
      this.chartOptions = this.generateStackedBarOptions(data);
    } else if (type === 'donut') {
      this.chartOptions = this.generateDonutOptions(data);
    }
  }

generateStackedBarOptions(data: Result): AgChartOptions {
  const questionMap = new Map<string, Record<string, number>>();
  [data].forEach((result) => {
    result.answers.forEach((answer, index) => {
      const questionKey = `Q${index + 1}`;
      if (!questionMap.has(questionKey)) {
        questionMap.set(questionKey, {});
      }
      const questionData = questionMap.get(questionKey)!;

      // Brug "Brugerdefineret svar" hvis svaret er brugerdefineret
      const responses = [
        answer.isStudentResponseCustom ? "Brugerdefineret svar" : (answer.studentResponse || '').trim(),
        answer.isTeacherResponseCustom ? "Brugerdefineret svar" : (answer.teacherResponse || '').trim()
      ];
      responses.forEach(resp => {
        if (!resp) return;
        questionData[resp] = (questionData[resp] || 0) + 1;
      });
    });
  });
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
  const series: AgBarSeriesOptions[] = Array.from(uniqueAnswers).map(answer => ({
    type: 'bar',
    xKey: 'question',
    yKey: answer,
    yName: `Svar: ${answer}`,
    stacked: true
  }));
  return {
    data: chartData,
    theme: {
      baseTheme: "ag-polychroma",
      overrides: {
        bar: { series: { label: { enabled: true } } }
      }
    },
    title: { text: 'Sammenligning af besvarelser', fontSize: 18 },
    series,
    axes: [
      { type: 'category', position: 'bottom', title: { text: 'Spørgsmål' } },
      { type: 'number', position: 'left', title: { text: 'Valgt svar' } }
    ]
  };
}

generateDonutOptions(data: Result): AgChartOptions {
  const answerCounts: Record<string, number> = {};
  data.answers.forEach(answer => {
    // Brug "Brugerdefineret svar" hvis svaret er brugerdefineret
    const responses = [
      answer.isStudentResponseCustom ? "Brugerdefineret svar" : (answer.studentResponse || '').trim(),
      answer.isTeacherResponseCustom ? "Brugerdefineret svar" : (answer.teacherResponse || '').trim()
    ];
    responses.forEach(resp => {
      if (!resp) return;
      answerCounts[resp] = (answerCounts[resp] || 0) + 1;
    });
  });
  const chartData = Object.entries(answerCounts).map(([answer, count]) => ({
    answer, count
  }));
  const total = chartData.reduce((sum, d) => sum + d.count, 0);

  return {
    data: chartData,
    title: { text: 'Svarfordeling (Donut)', fontSize: 18 },
    footnote: { text: `Total: ${total}` },
    series: [{
      type: "donut", 
      angleKey: 'count',
      sectorLabelKey: 'count',
      calloutLabelKey: 'answer',
      innerRadiusRatio: 1, 
      sectorSpacing: 4,
      calloutLabel: { enabled: false },
      title: { text: "Antal" },
    }],
    legend: { position: 'right' }
  };
}

}