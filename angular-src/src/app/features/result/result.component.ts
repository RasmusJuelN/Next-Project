import { Component, OnInit } from "@angular/core";
import { ActivatedRoute } from "@angular/router";
import { ResultService } from "./services/result.service";
import { PdfGenerationService } from "./services/pdf-generation.service";
import { Result } from "./models/result.model";
import { CommonModule } from "@angular/common";
import { AgCharts } from "ag-charts-angular";
import { AgBarSeriesOptions, AgChartOptions } from "ag-charts-community";
import { RouterModule } from "@angular/router";
import { FormsModule } from "@angular/forms";
import { TranslateService, TranslateModule } from "@ngx-translate/core";

@Component({
  selector: "app-result",
  standalone: true,
  providers: [ResultService, PdfGenerationService],
  imports: [CommonModule, AgCharts, RouterModule, FormsModule, TranslateModule],
  templateUrl: "./result.component.html",
  template: `
    <button (click)="updateChart('stacked')">Stacked</button>
    <button (click)="updateChart('donut')">Donut</button>
    <ag-charts-angular
      *ngIf="chartOptions"
      [options]="chartOptions"
    ></ag-charts-angular>
  `,
  styleUrls: ["./result.component.css"],
})
export class ResultComponent implements OnInit {
  result: Result | null = null;
  isLoading = true;
  errorMessage: string | null = null;

  chartType: "stacked" | "donut" = "stacked";
  chartOptions: AgChartOptions | null = null;
  
  // Toggle between compressed and full view
  isFullView = false;

  constructor(
    private route: ActivatedRoute,
    private resultService: ResultService,
    private pdfGenerationService: PdfGenerationService,
    public translate: TranslateService
  ) {}

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get("id");
    if (id) {
      // Tjek om resultatet kan tilgås inden det hentes
      this.resultService.canGetResult(id).subscribe({
        next: (canGet: boolean) => {
          if (canGet) {
            this.fetchResult(id);
          } else {
            this.errorMessage =
              "Resultatet er ikke tilgængeligt eller ikke fuldført.";
            this.isLoading = false;
          }
        },
        error: (err: any) => {
          console.error(err);
          this.errorMessage = "Fejl ved kontrol af resultatadgang.";
          this.isLoading = false;
        },
      });
    } else {
      this.errorMessage = "Ugyldigt resultat-ID.";
      this.isLoading = false;
    }
  }

  printPage(): void {
    window.print();
  }

  toggleView(): void {
    this.isFullView = !this.isFullView;
  }

  getTemplateQuestionOptions(questionPrompt: string): any[] {
    if (!this.result) return [];
    const answer = this.result.answers.find(a => a.question === questionPrompt);
    return answer?.options?.slice(0, 15) || [];
  }

  isOptionSelected(response: string, isCustom: boolean, option: any, index: number, role?: 'student' | 'teacher'): boolean {
    if (isCustom || !response) return false;
    
    // If the option has selection information and we know the role, use it directly
    if (role === 'student' && option.isSelectedByStudent !== undefined) {
      return option.isSelectedByStudent;
    }
    if (role === 'teacher' && option.isSelectedByTeacher !== undefined) {
      return option.isSelectedByTeacher;
    }
    
    // Fallback to text matching for backward compatibility
    if (response === option.displayText) return true;
    if (response === option.optionValue?.toString()) return true;
    if (response === (index + 1).toString()) return true; // Match 1-based index
    if (response === index.toString()) return true; // Match 0-based index
    
    // Try exact text match (case insensitive)
    if (response?.toLowerCase() === option.displayText?.toLowerCase()) return true;
    
    return false;
  }

  generatePdf(): void {
    if (this.result) {
      try {
        this.pdfGenerationService.generatePdf(this.result);
      } catch (error) {
        console.error('Error generating PDF:', error);
        alert('Error generating PDF. Please try again.');
      }
    } else {
      alert('Result data not available. Please wait for data to load.');
    }
  }

  openPdf(): void {
    if (this.result) {
      try {
        this.pdfGenerationService.openPdf(this.result);
      } catch (error) {
        console.error('Error opening PDF:', error);
        alert('Error opening PDF. Please try again.');
      }
    } else {
      alert('Result data not available. Please wait for data to load.');
    }
  }



  fetchResult(id: string): void {
    this.resultService.getResultById(id).subscribe({
      next: (data: Result) => {
        if (data) {
          this.result = data;
          this.updateChart(this.chartType, data);
          this.isLoading = false;
        } else {
          this.errorMessage = "Resultat ikke fundet.";
          this.isLoading = false;
        }
      },
      error: (err: any) => {
        console.error(err);
        this.errorMessage = "Resultat ikke fundet.";
        this.isLoading = false;
      },
    });
  }



  updateChart(type: "stacked" | "donut", dataOverride?: Result): void {
    this.chartType = type;
    const data = dataOverride || this.result;
    if (!data) return;
    if (type === "stacked") {
      this.chartOptions = this.generateStackedBarOptions(data);
    } else if (type === "donut") {
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
          answer.isStudentResponseCustom
            ? "Brugerdefineret svar"
            : (answer.studentResponse || "").trim(),
          answer.isTeacherResponseCustom
            ? "Brugerdefineret svar"
            : (answer.teacherResponse || "").trim(),
        ];
        responses.forEach((resp) => {
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
    const series: AgBarSeriesOptions[] = Array.from(uniqueAnswers).map(
      (answer) => ({
        type: "bar",
        xKey: "question",
        yKey: answer,
        yName: `Svar: ${answer}`,
        stacked: true,
      })
    );
    return {
      data: chartData,
      theme: {
        baseTheme: "ag-polychroma",
        overrides: {
          bar: { series: { label: { enabled: true } } },
        },
      },
      title: { text: "Sammenligning af besvarelser", fontSize: 18 },
      series,
      axes: [
        { type: "category", position: "bottom", title: { text: "Spørgsmål" } },
        { type: "number", position: "left", title: { text: "Valgt svar" } },
      ],
    };
  }

  generateDonutOptions(data: Result): AgChartOptions {
    // Match DataCompareComponent's pie styling
    const answerCounts: Record<string, number> = {};
    data.answers.forEach((answer) => {
      const responses = [
        answer.isStudentResponseCustom
          ? "Brugerdefineret svar"
          : (answer.studentResponse || "").trim(),
        answer.isTeacherResponseCustom
          ? "Brugerdefineret svar"
          : (answer.teacherResponse || "").trim(),
      ];
      responses.forEach((resp) => {
        if (!resp) return;
        answerCounts[resp] = (answerCounts[resp] || 0) + 1;
      });
    });
    const chartData = Object.entries(answerCounts).map(([answer, count]) => ({
      answer, count,
      dates: [] // No date info in this context
    }));
    return {
      data: chartData,
      title: { text: `Svarfordeling` },
      theme: 'ag-polychroma',
      series: [
        {
          type: 'pie',
          calloutLabelKey: 'answer',
          sectorLabelKey: 'count',
          angleKey: 'count',
          calloutLabel: { offset: 20 },
          sectorLabel: {
            positionOffset: 30,
            formatter: ({ datum, angleKey }: { datum: any; angleKey: string }) => {
              const value = datum[angleKey];
              const total = chartData.reduce((sum, d) => sum + d.count, 0);
              const percentage = total ? ((value / total) * 100).toFixed(1) : '0';
              return parseFloat(percentage) >= 5 ? `${percentage}%` : '';
            },
          },
          strokeWidth: 1,
          tooltip: {
            enabled: true,
            renderer: (params: { datum: any; angleKey: string }) => {
              const { datum, angleKey } = params;
              const value = datum[angleKey];
              const total = chartData.reduce((sum, d) => sum + d.count, 0);
              const percentage = total ? ((value / total) * 100).toFixed(1) : '0';
              return {
                title: datum.answer,
                content: `Antal: ${value}<br>Andel: ${percentage}%` // No date info
              };
            },
          },
        },
      ],
      legend: { enabled: false },
      animation: { enabled: true, duration: 800 },
    };
  }
}
