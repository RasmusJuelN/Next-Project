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
  imports: [CommonModule, FormsModule, TranslateModule, ShowResultComponent, TranslateModule],
  templateUrl: './result-history.component.html',
  styleUrls: ['./result-history.component.css']
})
export class ResultHistoryComponent implements OnInit {
  private resultHistoryService = inject(ResultHistoryService);
  private pdfGenerationService = inject(PdfGenerationService);

  public translate = inject(TranslateService)
  
  public searchEnum = SearchEnum;
  public student = this.createSearchState<User>();
  public template = this.createSearchState<TemplateBase>();

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
        },
        error: () => {
          this.errorMessage = 'Failed to load student result history.';
          this.isLoading = false;
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
}
