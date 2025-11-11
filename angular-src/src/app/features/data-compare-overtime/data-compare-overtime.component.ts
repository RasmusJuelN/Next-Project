import { CommonModule } from '@angular/common';
import type { AgCartesianChartOptions } from 'ag-charts-community';
import { Component, OnInit, inject } from '@angular/core';
import { AgCharts } from 'ag-charts-angular';
import { DataCompareService } from './services/data-compare-overtime.service';

@Component({
  selector: 'app-data-compare-overtime',
  standalone: true,
  imports: [CommonModule, AgCharts],
  templateUrl: './data-compare-overtime.component.html',
  styleUrls: ['./data-compare-overtime.component.css'],
})
export class DataCompareOvertimeComponent implements OnInit {
  private dataCompare = inject(DataCompareService);

  public answers: string[] = [];
  public allQuestions: any[] = [];
  public allResponses: any[] = [];

  public currentQuestionIndex = 0;
  public totalQuestions = 0;

  public options: AgCartesianChartOptions = {
    title: { text: 'Svar over tid' },
    data: [],
    series: [
      {
        type: 'line',
        xKey: 'date',
        yKey: 'TeacherAnswer',
        yName: 'Lærer',
        tooltip: {
          renderer: (params) => ({
            content: `Lærer: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
          }),
        },
      },
      {
        type: 'line',
        xKey: 'date',
        yKey: 'StudentAnswer',
        yName: 'Elev',
        tooltip: {
          renderer: (params) => ({
            content: `Elev: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
          }),
        },
      },
    ],
    axes: [
      { type: 'time', position: 'bottom', title: { text: 'Tidspunkt for besvarelse' } },
      {
        type: 'number',
        position: 'left',
        label: {
          formatter: (params) => this.answers[params.value] ?? '',
        },
      },
    ],
  };

  ngOnInit(): void {
    // demo IDs
    const templateId = '3fa85f64-5717-4562-b3fc-2c963f66afa6';
    const studentId = '6a6515fd-16b6-4961-91e4-b37d6f2c2c28';
    const teacherId = 'f3229540-03d1-4f55-919c-feac1ebe0dba';
    
    this.getQuestionareOptions(templateId, studentId, teacherId);



  }

private getQuestionareOptions(templateId: string, studentId: string, teacherId: string){
    // get all questions
    this.dataCompare.getQuestionareOptiontsByID(templateId).subscribe({
      next: (res) => {
        if (res?.questions?.length) {
          this.allQuestions = res.questions;
          this.totalQuestions = res.questions.length;
        }
        this.loadResponses(templateId, studentId, teacherId);
      },
    });
}

private loadResponses(templateId: string, studentId: string, teacherId: string) {
  this.dataCompare.getResponsesByID(studentId, teacherId, templateId).subscribe({
    next: (res) => {
      console.log('Raw API responses:', res);
      this.allResponses = Array.isArray(res) ? res : [];
      this.updateChartForQuestion(this.currentQuestionIndex);
    },
    error: (err) => console.error('Error fetching responses:', err),
  });
}


private updateChartForQuestion(index: number) {
  const question = this.allQuestions[index];
  console.log(' Updating chart for question:', question?.text ?? question); 

  if (!question) return;

  this.answers = (question.options ?? [])
    .map((opt: any) => opt.displayText)
    .filter((t: string) => !!t);
  console.log(' Possible answers:', this.answers);

  const toNumeric = (text: string) => {
    const idx = this.answers.indexOf(text);
    return idx !== -1 ? idx : null;
  };

  const filtered = this.allResponses
    .flatMap((entry) => {
      const match = entry.answers?.filter(
    (a: any) =>
    a.question === question.prompt ||
    a.question === question.title ||
    a.question === question.text
      );
      return match?.map((a: any) => ({
        date: new Date(
          entry.student?.completedAt ?? entry.teacher?.completedAt ?? Date.now()
        ),
        TeacherAnswer: toNumeric(a.teacherResponse),
        StudentAnswer: toNumeric(a.studentResponse),
      }));
    })
    .filter((x) => x && (x.TeacherAnswer !== null || x.StudentAnswer !== null));

  console.log(' Filtered chart data:', filtered);

this.options = {
  ...this.options,
  title: { text: question.prompt || question.title || question.text || 'Spørgsmål' },
  data: filtered,
axes: [
  {
    type: 'time',
    position: 'bottom',
    title: { text: 'Tidspunkt for besvarelse' },
  },
{
  type: 'number',
  position: 'left',
  min: 0,
  max: this.answers.length - 1,
  nice: false,
  label: {
    avoidCollisions: false,
    formatter: (p) => {
      const v = p.value;
      // only label exact integers to avoid duplicates
      if (Math.abs(v - Math.round(v)) > 1e-6) return '';
      const idx = Math.round(v);
      return this.answers[idx] ?? '';
    },
  },
  tick: {
    // @ts-expect-error: supported at runtime
    step: 1, // ask for 1-per-index ticks
  },
  title: { text: 'Svarmuligheder' },
}
,
],


};


}


  public nextQuestion() {
    if (this.currentQuestionIndex < this.totalQuestions - 1) {
      this.currentQuestionIndex++;
      this.updateChartForQuestion(this.currentQuestionIndex);
    }
  }

  public prevQuestion() {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
      this.updateChartForQuestion(this.currentQuestionIndex);
    }
  }
}
