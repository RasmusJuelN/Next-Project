import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule, TranslateService } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

import { User } from '../../shared/models/user.model';
import { TemplateBase } from '../active-questionnaire-manager/models/active.models';

import {
  ShowResultComponent,
  ShowResultConfig
} from '../../shared/show-result/show-result.component';

import {
  Answer,
  QuestionOption,
  Result
} from '../../shared/models/result.model';
import { ResultHistoryService } from './services/result-history.service';
import { Attempt, StudentResultHistory, AnswerInfo } from './models/result-history.model';
import { PdfGenerationService } from '../result/services/pdf-generation.service';
import { AgCharts } from 'ag-charts-angular';
import type { AgCartesianChartOptions } from 'ag-charts-community';
import { DataCompareService } from '../data-compare-overtime/services/data-compare-overtime.service';

enum SearchEnum {
  Student = 'student',
  Template = 'template'
}

interface SearchState<T> {
  selected: T | null;
  query: string;
  results: T[];
  loading: boolean;
  error: string | null;
  cursor?: string;
  input$: Subject<string>;
}

@Component({
  selector: 'app-result-history',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule, ShowResultComponent, TranslateModule, AgCharts],
  templateUrl: './result-history.component.html',
  styleUrls: ['./result-history.component.css']
})
export class ResultHistoryComponent implements OnInit {
  private resultHistoryService = inject(ResultHistoryService);
  private pdfGenerationService = inject(PdfGenerationService);
  private dataCompareService = inject(DataCompareService);


  public translate = inject(TranslateService)
  
  public searchEnum = SearchEnum;
  public student = this.createSearchState<User>();
  public template = this.createSearchState<TemplateBase>();
  public showGraph = false;
  public currentQuestionIndex = 0;
  public totalQuestions = 0;
  public allQuestions: any[] = [];
  public allResponses: any[] = [];
  public answers: string[] = [];
  public chartOptions: AgCartesianChartOptions = {
    title: { text: 'Svar over tid' },
    data: [],
    series: [],
    axes: []
  };

  public showDropdown = {
    [SearchEnum.Student]: false,
    [SearchEnum.Template]: false
  };

  public history: StudentResultHistory | null = null;

  // which attempt the UI is currently showing
  public currentAttemptIndex = 0;

  public isLoading = false;
  public errorMessage: string | null = null;

  // config passed to <app-show-result>
  readonly resultConfig: ShowResultConfig = {
    showTemplate: true,
    showStudent: true,
    showTeacher: true,
    showCompletionDates: true,
    useCardStyling: false,
    showActions: false
  };
  public isFullView = true;

  private createSearchState<T>(): SearchState<T> {
    return {
      selected: null,
      query: '',
      results: [],
      loading: false,
      error: null,
      cursor: undefined,
      input$: new Subject<string>()
    };
  }

  /** Close dropdowns when clicking outside */
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const el = event.target as HTMLElement;
    for (const key in this.showDropdown) {
      this.showDropdown[key as SearchEnum] = !!el.closest(
        `[data-search="${key}"]`
      );
    }
  }

  ngOnInit(): void {
    this.setupSearch(this.student, SearchEnum.Student);
    this.setupSearch(this.template, SearchEnum.Template);
  }

  downloadPdf(): void {
    const result = this.getCurrentResultLikeResult();
    if (!result) return;
    // assume service takes the current result and triggers browser download
    this.pdfGenerationService.generatePdf(result);
  }

  openPdf(): void {
    const result = this.getCurrentResultLikeResult();
    if (!result) return;
    this.pdfGenerationService.openPdf(result);
  }

  private setupSearch<T>(state: SearchState<T>, type: SearchEnum): void {
    state.input$
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => this.fetch(type, term));
  }

private fetch(type: SearchEnum, term: string): void {
  const state = this.getState(type);
  if (!term.trim()) return;

  state.loading = true;
  state.error = null;
  state.results = [];

  const handleError = (msg: string) => {
    state.error = msg;
    state.loading = false;
  };

  if (type === SearchEnum.Student) {
    this.resultHistoryService.searchStudentsRelatedToTeacher(term).subscribe({
      next: (users: User[]) => {
        state.results = users.map(user => ({
          id: user.id,
          userName: user.userName,
          fullName: user.fullName,
          role: 'student' as const
        }));
        state.loading = false;
      },
      error: () => handleError(`Failed to load related ${type}s.`)
    });
  }
}


  select(type: SearchEnum, item: any): void {
    const state = this.getState(type);
    state.selected = item;
    state.query = '';
    this.showDropdown[type] = false;

    // If selecting a student, clear template selection and load available templates
    if (type === SearchEnum.Student) {
      this.clearSelected(SearchEnum.Template);
      this.loadAvailableTemplates();
    }

    if (this.student.selected && this.template.selected) {
      this.fetchStudentResultsV2();
    }
  }

  clearSelected(type: SearchEnum): void {
    const state = this.getState(type);
    state.selected = null;
    state.results = [];
    state.query = '';
    this.history = null;
    this.currentAttemptIndex = 0;

    if (type === SearchEnum.Template && this.student.selected) {
      this.loadAvailableTemplates();
    }
  }

  /**
   * Load available templates for the selected student.
   * This shows templates where both the teacher and student have completed responses.
   */
  loadAvailableTemplates(): void {
    if (!this.student.selected) return;

    this.template.loading = true;
    this.template.error = null;

    this.resultHistoryService.getTemplateBasesAnsweredByStudent(this.student.selected.id).subscribe({
      next: (res: any) => {
        this.template.results = res.templateBases || [];
        this.template.loading = false;
        
        if (this.template.results.length === 0) {
          this.template.error = 'No shared questionnaire completions found with this student.';
        }
      },
      error: () => {
        this.template.error = 'Failed to load available templates.';
        this.template.loading = false;
      }
    });
  }

  onInputChange(type: SearchEnum, value: string): void {
    const state = this.getState(type);
    state.query = value;
    state.input$.next(value);
    this.showDropdown[type] = true;
  }

  private getState<T>(type: SearchEnum): SearchState<T> {
    return type === SearchEnum.Student
      ? (this.student as any)
      : (this.template as any);
  }

  /**
   * Fetch using the new normalized "history with attempts" shape
   */
 fetchStudentResultsV2(): void {
    if (!this.student.selected || !this.template.selected) return;

    this.isLoading = true;
    this.errorMessage = null;

    this.resultHistoryService
      .getStudentResultHistory(
        this.student.selected.id,
        this.template.selected.id
      )
      .subscribe({
        next: (data: StudentResultHistory) => {
          this.history = data;
          this.currentAttemptIndex = 0;
          this.isLoading = false;
          
          console.log('History loaded:', this.history);
          if (this.shouldLoadGraphAfterHistory) {
            this.shouldLoadGraphAfterHistory = false;
            this.TestGraf();
          }
        },
        error: () => {
          this.errorMessage = 'Failed to load student result history.';
          this.isLoading = false;
          this.shouldLoadGraphAfterHistory = false;
        }
      });
  }

  // ---- timeline navigation ----
  nextAttempt() {
    if (!this.history) return;
    if (this.currentAttemptIndex < this.history.answersInfo.length - 1) {
      this.currentAttemptIndex++;
    }
  }

  prevAttempt() {
    if (!this.history) return;
    if (this.currentAttemptIndex > 0) {
      this.currentAttemptIndex--;
    }
  }

  // ---- accessors / helpers for template ----
  getCurrentAttempt(): AnswerInfo | null {
    if (!this.history) return null;
    return this.history.answersInfo[this.currentAttemptIndex] ?? null;
  }

  canShowAttempts(): boolean {
    return (
      !this.isLoading &&
      !!this.history &&
      Array.isArray(this.history.answersInfo) &&
      this.history.answersInfo.length > 0
    );
  }

  getAttempts(): AnswerInfo[] {
    return this.history?.answersInfo ?? [];
  }

  getTemplateTitle(): string {
    return this.history?.template?.title ?? '';
  }

  getStudentName(): string {
    return this.history?.student?.fullName ?? '';
  }

  getTeacherName(): string {
    return this.history?.teacher?.fullName ?? '';
  }

  // resolve question text from the template using questionId
  getQuestionPrompt(questionId: string): string {
    if (!this.history) return '';
    const q = this.history.template.questions.find(
      (q) => String(q.id) === String(questionId)
    );
    return q?.prompt ?? '';
  }

  // resolve answer option labels from template option IDs
  getOptionLabels(optionIds: number[] | undefined): string[] {
    if (!this.history || !optionIds?.length) return [];
    const allOptions = this.history.template.questions.flatMap(
      (q) => q.options
    );
    return optionIds
      .map((id) => allOptions.find((o) => o.id === id)?.displayText)
      .filter((txt): txt is string => !!txt);
  }

  /**
   * Expose a Result-shaped object for <app-show-result>
   * based on the currently selected AnswerInfo.
   */
  getCurrentResultLikeResult(): Result | null {
    const answerInfo = this.getCurrentAttempt();
    if (!answerInfo) return null;
    return this.attemptToResult(answerInfo);
  }

  /**
   * Convert a single AnswerInfo into a synthetic Result
   * so <app-show-result> can render it with zero changes.
   */
  private attemptToResult(answerInfo: AnswerInfo): Result | null {
    if (!this.history) return null;

    const template = this.history.template;
    const studentUser = this.history.student;
    const teacherUser = this.history.teacher;

    const answers = answerInfo.answers.map((answerDetail): Answer => {
      // 1. find this question in the template
      const qMeta = template.questions.find(
        (q) => String(q.id) === String(answerDetail.questionId)
      );

      const questionPrompt = qMeta?.prompt ?? '(unknown question)';

      // 2. build options[] to match Result.Answer.options
      const options: QuestionOption[] | undefined = qMeta
        ? qMeta.options.map((opt) => {
            const isSelectedByStudent =
              answerDetail.selectedOptionIdsByStudent?.includes(opt.id) ?? false;
            const isSelectedByTeacher =
              answerDetail.selectedOptionIdsByTeacher?.includes(opt.id) ?? false;

            return {
              displayText: opt.displayText,
              optionValue: String(opt.optionValue ?? opt.id), // legacy safe
              isSelectedByStudent,
              isSelectedByTeacher,
              sortOrder: opt.sortOrder
            };
          })
        : undefined;

      // 3. Determine the actual response text for student and teacher
      const studentResponse = answerDetail.isStudentResponseCustom && answerDetail.studentResponse
        ? answerDetail.studentResponse
        : this.getSelectedOptionText(answerDetail.selectedOptionIdsByStudent, qMeta?.options);

      const teacherResponse = answerDetail.isTeacherResponseCustom && answerDetail.teacherResponse
        ? answerDetail.teacherResponse
        : this.getSelectedOptionText(answerDetail.selectedOptionIdsByTeacher, qMeta?.options);

      // 4. return this answer in Result.Answer shape
      return {
        question: questionPrompt,
        studentResponse: studentResponse ?? '',
        isStudentResponseCustom: !!answerDetail.isStudentResponseCustom,
        teacherResponse: teacherResponse ?? '',
        isTeacherResponseCustom: !!answerDetail.isTeacherResponseCustom,
        options
      };
    });

    // 5. wrap up as a Result
    return {
      id: answerInfo.activeQuestionnaireId,
      title: template.title,
      description: template.description ?? null,
      student: {
        user: studentUser,
        completedAt: answerInfo.studentCompletedAt ? new Date(answerInfo.studentCompletedAt) : new Date(0)
      },
      teacher: {
        user: teacherUser,
        completedAt: answerInfo.teacherCompletedAt ? new Date(answerInfo.teacherCompletedAt) : new Date(0)
      },
      answers
    };
  }

  /**
   * Helper method to get the display text for selected option IDs
   */

  private getSelectedOptionText(selectedOptionIds: number[] | null | undefined, options: any[] | undefined): string | null {
    if (!selectedOptionIds?.length || !options?.length) return null;
    
    const selectedOption = options.find(opt => selectedOptionIds.includes(opt.id));
    return selectedOption?.displayText ?? null;
  }

  public hasResult(): boolean {
    return !!this.getCurrentResultLikeResult();
  }

public TestGraf() {
    console.log("TestGraf");
    
    if (!this.student.selected || !this.template.selected) {
      console.warn('Missing student or template selection');
      return;
    }

    // Get IDs from selected items and history
    const templateId = this.template.selected.id;
    const studentId = this.student.selected.id;
    
        // The teacher ID should come from the history object
    // If history isn't loaded yet, we need to wait for it
    if (!this.history) {
      console.warn('History not loaded yet, fetching it first...');
      // Fetch the history first, then load graph
      this.fetchStudentResultsV2();
      // Set a flag to load graph after history loads
      this.shouldLoadGraphAfterHistory = true;
      return;
    }

    const teacherId = this.history.teacher.id;

    if (!teacherId) {
      console.error('Teacher ID is undefined!');
      return;
    }

    this.showGraph = !this.showGraph;

    if (this.showGraph) {
      this.loadGraphData(templateId, studentId, teacherId);
    }

  }

    private shouldLoadGraphAfterHistory = false;


  private loadGraphData(templateId: string, studentId: string, teacherId: string) {
    // Load questions
    this.dataCompareService.getQuestionareOptiontsByID(templateId).subscribe({
      next: (res) => {
        if (res?.questions?.length) {
          this.allQuestions = res.questions;
          this.totalQuestions = res.questions.length;
        }
        this.loadGraphResponses(templateId, studentId, teacherId);
      },
      error: (err) => console.error('Error loading questions:', err)
    });
  }

  private loadGraphResponses(templateId: string, studentId: string, teacherId: string) {
    this.dataCompareService.getResponsesByID(studentId, teacherId, templateId).subscribe({
      next: (res) => {
        console.log('Raw API responses:', res);
        this.allResponses = Array.isArray(res) ? res : [];
        this.currentQuestionIndex = 0;
        this.updateChartForQuestion(this.currentQuestionIndex);
      },
      error: (err) => console.error('Error fetching responses:', err),
    });
  }

  private updateChartForQuestion(index: number) {
    const question = this.allQuestions[index];
    console.log('Updating chart for question:', question?.text ?? question);

    if (!question) return;

    this.answers = (question.options ?? [])
      .map((opt: any) => opt.displayText)
      .filter((t: string) => !!t);
    console.log('Possible answers:', this.answers);

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
        return match?.map((a: any) => {
          const completedDate = new Date(
            entry.student?.completedAt ?? entry.teacher?.completedAt ?? Date.now()
          );
          return {
            date: completedDate,
            dateLabel: completedDate.toLocaleString('da-DK', {
              year: 'numeric',
              month: 'short',
              day: 'numeric',
            }),
            TeacherAnswer: toNumeric(a.teacherResponse),
            StudentAnswer: toNumeric(a.studentResponse),
          };
        });
      })
      .filter((x) => x && (x.TeacherAnswer !== null || x.StudentAnswer !== null))
      .sort((a, b) => a.date.getTime() - b.date.getTime());

    const indexedData = filtered.map((item, idx) => ({
      ...item,
      index: idx,
      displayLabel: `${item.dateLabel} (#${idx + 1})`
    }));

    console.log('Filtered chart data:', indexedData);

    this.chartOptions = {
      title: { text: question.prompt || question.title || question.text || 'Spørgsmål' },
      data: indexedData,
      series: [
        {
          type: 'line',
          xKey: 'index',
          yKey: 'TeacherAnswer',
          yName: 'Lærer',
          tooltip: {
            renderer: (params) => ({
              content: `${params.datum.dateLabel}<br/>Lærer: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
            }),
          },
        },
        {
          type: 'line',
          xKey: 'index',
          yKey: 'StudentAnswer',
          yName: 'Elev',
          tooltip: {
            renderer: (params) => ({
              content: `${params.datum.dateLabel}<br/>Elev: ${this.answers[params.datum[params.yKey]] ?? 'Ukendt'}`,
            }),
          },
        },
      ],
      axes: [
        {
          type: 'category',
          position: 'bottom',
          title: { text: 'Tidspunkt for besvarelse' },
          label: {
            rotation: 0,
            formatter: (params) => {
              const dataPoint = indexedData[params.value];
              return dataPoint ? dataPoint.dateLabel : '';
            }
          }
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
              if (Math.abs(v - Math.round(v)) > 1e-6) return '';
              const idx = Math.round(v);
              const text = this.answers[idx] ?? '';
              return text.length > 25 ? text.substring(0, 22) + '...' : text;
            },
          },
          tick: {
            // @ts-expect-error: supported at runtime
            step: 1,
          },
          title: { text: 'Svarmuligheder' },
        },
      ],
    };
  }

  public nextGraphQuestion() {
    if (this.currentQuestionIndex < this.totalQuestions - 1) {
      this.currentQuestionIndex++;
      this.updateChartForQuestion(this.currentQuestionIndex);
    }
  }

  public prevGraphQuestion() {
    if (this.currentQuestionIndex > 0) {
      this.currentQuestionIndex--;
      this.updateChartForQuestion(this.currentQuestionIndex);
    }
  }

}


