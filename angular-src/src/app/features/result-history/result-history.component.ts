import { CommonModule } from '@angular/common';
import { Component, EventEmitter, HostListener, OnInit, Output, inject } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { User } from '../../shared/models/user.model';
import { TemplateBase } from '../active-questionnaire-manager/models/active.models';
import { ShowResultComponent, ShowResultConfig } from '../../shared/show-result/show-result.component';
import { Result } from '../result/models/result.model';
import { ResultHistoryService } from './services/result-history.service';

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

  public showDropdown = { [SearchEnum.Student]: false, [SearchEnum.Template]: false };

  public results: Result[] = [];
  public currentResultIndex = 0;
  public isLoading = false;
  public errorMessage: string | null = null;

  readonly resultConfig: ShowResultConfig = {
    showTemplate: true,
    showStudent: true,
    showTeacher: false,
    showCompletionDates: true,
    useCardStyling: true,
    showActions: false
  };

  @Output() backToListEvent = new EventEmitter<void>();

  private createSearchState<T>(): SearchState<T> {
    return { selected: null, query: '', results: [], loading: false, error: null, cursor: undefined, input$: new Subject<string>() };
  }

  /** Close dropdowns when clicking outside */
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent) {
    const el = event.target as HTMLElement;
    for (const key in this.showDropdown) {
      this.showDropdown[key as SearchEnum] = !!el.closest(`[data-search="${key}"]`);
    }
  }

  ngOnInit(): void {
    this.setupSearch(this.student, SearchEnum.Student);
    this.setupSearch(this.template, SearchEnum.Template);
  }

  private setupSearch<T>(state: SearchState<T>, type: SearchEnum): void {
    state.input$.pipe(debounceTime(300), distinctUntilChanged()).subscribe((term) => this.fetch(type, term));
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
      this.resultHistoryService.searchUsers(term, 'student', 10, state.cursor).subscribe({
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
      this.fetchStudentResults();
    }
  }

  clearSelected(type: SearchEnum): void {
    this.getState(type).selected = null;
    this.results = [];
    this.currentResultIndex = 0;
  }

  onInputChange(type: SearchEnum, value: string): void {
    const state = this.getState(type);
    state.query = value;
    state.input$.next(value);
    this.showDropdown[type] = true;
  }

  private getState<T>(type: SearchEnum): SearchState<T> {
    return type === SearchEnum.Student ? (this.student as any) : (this.template as any);
  }

  onBackToList(): void {
    this.backToListEvent.emit();
  }

  /** ✅ Now uses ResultHistoryService mock instead of inline mock */
  fetchStudentResults(): void {
    if (!this.student.selected || !this.template.selected) return;

    this.isLoading = true;
    this.errorMessage = null;

    this.resultHistoryService
      .getStudentResultHistory(this.student.selected.id, this.template.selected.id)
      .subscribe({
        next: (data) => {
          this.results = data.results ?? [];
          this.currentResultIndex = 0;
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to load student result history.';
          this.isLoading = false;
        }
      });
  }

  nextResult() {
    if (this.currentResultIndex < this.results.length - 1) this.currentResultIndex++;
  }

  prevResult() {
    if (this.currentResultIndex > 0) this.currentResultIndex--;
  }

  getCurrentResult(): Result | null {
    return this.results[this.currentResultIndex] ?? null;
  }
}
