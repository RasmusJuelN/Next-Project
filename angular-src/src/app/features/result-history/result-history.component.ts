import { CommonModule } from '@angular/common';
import { Component, HostListener, OnInit, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
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
import { Attempt, StudentResultHistory } from './models/result-history.model';

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
  imports: [CommonModule, FormsModule, TranslateModule, ShowResultComponent],
  templateUrl: './result-history.component.html',
  styleUrls: ['./result-history.component.css']
})
export class ResultHistoryComponent implements OnInit {
  private resultHistoryService = inject(ResultHistoryService);

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

    if (type === SearchEnum.Template) {
      this.resultHistoryService.searchTemplates(term, state.cursor).subscribe({
        next: (res: any) => {
          state.results = res.templateBases;
          state.loading = false;
        },
        error: () => handleError(`Failed to load ${type}s.`)
      });
    } else {
      this.resultHistoryService
        .searchUsers(term, 'student', 10, state.cursor)
        .subscribe({
          next: (res: any) => {
            state.results = res.userBases || [];
            state.cursor = res.sessionId;
            state.loading = false;
          },
          error: () => handleError(`Failed to load ${type}s.`)
        });
    }
  }

  select(type: SearchEnum, item: any): void {
    const state = this.getState(type);
    state.selected = item;
    state.query = '';
    this.showDropdown[type] = false;

    if (this.student.selected && this.template.selected) {
      this.fetchStudentResultsV2();
    }
  }

  clearSelected(type: SearchEnum): void {
    this.getState(type).selected = null;
    this.history = null;
    this.currentAttemptIndex = 0;
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
    if (this.currentAttemptIndex < this.history.attempts.length - 1) {
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
  getCurrentAttempt(): Attempt | null {
    if (!this.history) return null;
    return this.history.attempts[this.currentAttemptIndex] ?? null;
  }

  canShowAttempts(): boolean {
    return (
      !this.isLoading &&
      !!this.history &&
      Array.isArray(this.history.attempts) &&
      this.history.attempts.length > 0
    );
  }

  getAttempts(): Attempt[] {
    return this.history?.attempts ?? [];
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
   * based on the currently selected Attempt.
   */
  getCurrentResultLikeResult(): Result | null {
    const attempt = this.getCurrentAttempt();
    if (!attempt) return null;
    return this.attemptToResult(attempt);
  }

  /**
   * Convert a single Attempt into a synthetic Result
   * so <app-show-result> can render it with zero changes.
   */
  private attemptToResult(attempt: Attempt): Result | null {
    if (!this.history) return null;

    const template = this.history.template;
    const studentUser = this.history.student;
    const teacherUser = this.history.teacher;

    const answers = attempt.answers.map((ans): Answer => {
      // 1. find this question in the template
      const qMeta = template.questions.find(
        (q) => String(q.id) === String(ans.questionId)
      );

      const questionPrompt = qMeta?.prompt ?? '(unknown question)';

      // 2. build options[] to match Result.Answer.options
      const options: QuestionOption[] | undefined = qMeta
        ? qMeta.options.map((opt) => {
            const isSelectedByStudent =
              ans.selectedOptionIdsByStudent?.includes(opt.id) ?? false;
            const isSelectedByTeacher =
              ans.selectedOptionIdsByTeacher?.includes(opt.id) ?? false;

            return {
              displayText: opt.displayText,
              optionValue: String(opt.optionValue ?? opt.id), // legacy safe
              isSelectedByStudent,
              isSelectedByTeacher
            };
          })
        : undefined;

      // 3. return this answer in Result.Answer shape
      return {
        question: questionPrompt,
        studentResponse: ans.studentResponse ?? '',
        isStudentResponseCustom: !!ans.isStudentResponseCustom,
        teacherResponse: ans.teacherResponse ?? '',
        isTeacherResponseCustom: !!ans.isTeacherResponseCustom,
        options
      };
    });

    // 4. wrap up as a Result
    return {
      id: 'synthetic-from-history',
      title: template.title,
      description: template.description ?? null,
      student: {
        user: studentUser,
        completedAt: attempt.studentCompletedAt ?? new Date(0)
      },
      teacher: {
        user: teacherUser,
        completedAt: attempt.teacherCompletedAt ?? new Date(0)
      },
      answers
    };
  }
}
