import { CommonModule } from '@angular/common';
import type { AgChartOptions } from 'ag-charts-community';
import type { AgCartesianChartOptions } from 'ag-charts-community';
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




public options: AgCartesianChartOptions = {
  title: { text: 'Spørgsmål' },
  data: [],
series: [
  {
    type: 'line',
    xKey: 'quarter',
    yKey: 'TeacherAnswer',
    yName: 'Teacher',
    tooltip: {
      renderer: (params) => ({
        content: `Teacher: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`
      }),
    },
  },
  {
    type: 'line',
    xKey: 'quarter',
    yKey: 'StudentAnswer',
    yName: 'Student',
    tooltip: {
      renderer: (params) => ({
        content: `Student: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`
      }),
    },
  },
],
  axes: [
    {
      type: 'category',
      position: 'bottom',
    },
    {
      type: 'number',
      position: 'left',
      label: {
        formatter: (params) => this.answers[params.value] ?? '', // hide numeric and show text
      },
    },
  ],
};


ngOnInit(): void {
  const rawId = 'e8687c09-5256-44ea-ea18-08ddf9a65b46';
  const activeId = rawId.replace(/-+$/, '');
  const rawTemplateId = '941eb4ee-b8fa-43a7-3275-08dddf10f79e';
  const templateId = rawTemplateId.replace(/-+$/, '');
  const rawStudentId = '6a6515fd-16b6-4961-91e4-b37d6f2c2c28';
  const studentId = rawStudentId.replace(/-+$/, '');
  const rawTeacherId = 'f3229540-03d1-4f55-919c-feac1ebe0dba';
  const teacherId = rawTeacherId.replace(/-+$/, '');

  this.dataCompare.getQuestionaireDataByID(activeId).subscribe({
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

      // Now fetch the responses and build chart data
      buildChartData(this.answers, this.dataCompare, templateId, studentId, teacherId).then((data) => {
        this.options = {
          ...this.options,
          data,
        };
      });
    },
    error: (err) => {
      console.error('Failed to load questionnaire data', err);
    },
  });
}

}

function buildChartData(
  answers: string[],
  dataCompareService: DataCompareService,
  templateId: string,
  studentId: string,
  teacherId: string
): Promise<any[]> {
  return new Promise<any[]>((resolve, reject) => {
    dataCompareService.getResponsesByID(studentId, teacherId, templateId).subscribe({
      next: (res) => {
        console.log('Full getresponse API result:', res);

        if (!res || !Array.isArray(res) || res.length === 0) {
          console.warn('No valid responses found in API response');
          resolve([]);
          return;
        }

        // Map text -> numeric index
        const toNumeric = (response: string) => {
          const idx = answers.indexOf(response);
          return idx !== -1 ? idx : null; // null to show gaps instead of -1
        };

        // Flatten all responses into one array
        const data = res.map((entry, index) => {
          const a = entry.answers?.[0];
          return {
            quarter: a?.question ?? entry.title ?? `Entry ${index + 1}`,
            TeacherAnswer: toNumeric(a?.teacherResponse),
            StudentAnswer: toNumeric(a?.studentResponse),
          };
        });

        console.log('Generated chart data:', data);
        resolve(data);
      },
      error: (err) => {
        console.error('Error fetching getResponsesByID:', err);
        reject(err);
      },
    });
  });
}

  



