import { CommonModule } from '@angular/common';
import type { AgChartOptions } from 'ag-charts-community';
import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  OnDestroy,
  OnInit,
  ViewChild,
  inject,
  Output,
} from "@angular/core";
import { AgCharts } from "ag-charts-angular";
import { HttpClient } from "@angular/common/http";
import { ActiveService } from "../active-questionnaire-manager/services/active.service";
import { User } from "../../shared/models/user.model";
import { TemplateBase } from "../active-questionnaire-manager/models/active.models";
import { DataCompareService } from './services/data-compare-overtime.service';

@Component({
  selector: 'app-data-compare-overtime',
  standalone: true,
  imports: [CommonModule, AgCharts,DataCompareOvertimeComponent],
   template: `
    <ag-charts-angular [options]="chartOptions"></ag-charts-angular>
  `,
  templateUrl: './data-compare-overtime.component.html',
  styleUrl: './data-compare-overtime.component.css'
})

export class DataCompareOvertimeComponent implements OnInit{
private dataCompare = inject(DataCompareService);


public answers: string[] = [];




  public options: AgChartOptions = {
    title: { text: 'Spørgsmål' },
    data: [],
    series: [
      { type: 'line', xKey: 'quarter', yKey: 'TeacherAnswer', yName: 'Teacher' },
      { type: 'line', xKey: 'quarter', yKey: 'StudentAnswer', yName: 'Student' },
    ],
  };

 ngOnInit(): void {

      const rawId = 'e8687c09-5256-44ea-ea18-08ddf9a65b46-';
      const templateId = rawId.replace(/-+$/, '');

    this.dataCompare.getQuestionaireDataByID(templateId).subscribe({
      next: (res) => {
        console.log('Full questionnaire response:', res);

        if (res && Array.isArray(res.questions) && res.questions.length > 0) {
          const firstQuestion = res.questions[0];
          if (Array.isArray(firstQuestion.options)) {
            this.answers = firstQuestion.options
              .map((opt: any) => opt.displayText)
            .filter((text: string) => !!text);
            console.log('First question possible answers:', this.answers);
          }
        } else {
          console.warn('No questions found in response.');
        }
      const data = buildChartData(this.answers);
              this.options = {
          ...this.options,
          data,
    };
      },
      error: (err) => {
        console.error('Failed to load questionnaire data', err);
      },
    });

    
}
}

function buildChartData(answers: string[]) {
    const studentAnswers = ['Lilla', 'Purple', 'Purple', 'Lilla', 'Lilla'];
    const teacherAnswers = ['Purple', 'Purple', 'Lilla', 'Violet', 'Violet'];

    const toNumbers = (arr: string[]) =>
      arr.map((ans) => {
        const idx = answers.indexOf(ans);
        return idx !== -1 ? idx : -1; // keep -1 for “not found”
      });

    const student = toNumbers(studentAnswers);
    const teacher = toNumbers(teacherAnswers);

    console.log('Student numeric answers:', student);
    console.log('Teacher numeric answers:', teacher);

    // Return chart rows (replace dates with your real x-values when ready)
    return [
      { quarter: '02-02-2020', TeacherAnswer: teacher[0], StudentAnswer: student[0] },
      { quarter: '05-03-2020', TeacherAnswer: teacher[1], StudentAnswer: student[1] },
      { quarter: '23-04-2020', TeacherAnswer: teacher[2], StudentAnswer: student[2] },
      { quarter: '02-05-2020', TeacherAnswer: teacher[3], StudentAnswer: student[3] },
    ];
}

  



