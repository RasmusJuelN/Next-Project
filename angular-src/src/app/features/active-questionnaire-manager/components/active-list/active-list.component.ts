import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { QuestionnaireGroupResult, QuestionnaireGroupKeysetPaginationResult } from '../../models/active.models';

import { PageChangeEvent, PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { LoadingComponent } from '../../../../shared/loading/loading.component';

@Component({
  selector: 'app-active-list',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent, LoadingComponent],
  templateUrl: './active-list.component.html',
  styleUrls: ['./active-list.component.css']
})
export class ActiveListComponent implements OnInit {
  private activeService = inject(ActiveService);
  groups: QuestionnaireGroupResult[] = [];

  // Collapse state per groupId
  groupCollapsed: { [id: string]: boolean } = {};

  toggleGroupCollapse(groupId: string) {
    this.groupCollapsed[groupId] = !this.groupCollapsed[groupId];
  }

  // Pagination
  pageSize: number = 5;
  currentPage: number = 1;
  totalItems: number = 0;
  totalPages: number = 0;

  // Cache cursors
  cachedCursors: { [pageNumber: number]: string | null } = {};

  // Search filters
  searchTitle: string = '';

  // UI state
  isListCollapsed: boolean = false;
  isLoading = false;
  errorMessage: string | null = null;

  @Output() createNewQuestionnaireEvent = new EventEmitter<void>();

  private searchSubject = new Subject<{ title: string }>();

  getAnsweredCount(questionnaires: any[], role: 'student' | 'teacher'): number {
    return questionnaires.filter(q =>
      role === 'student' ? q.studentCompletedAt : q.teacherCompletedAt
    ).length;
  }

  ngOnInit(): void {
    this.cachedCursors[1] = null;

    // Debounced search
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(({ title }) => {
        this.searchTitle = title;
        this.currentPage = 1;
        this.cachedCursors = { 1: null };
        this.fetchGroups();
      });

    // Initial fetch
    this.fetchGroups();
  }

  // --- Search handler ---
  onSearchTitleChange(value: string): void {
    this.searchSubject.next({ title: value });
  }

  // --- Pagination handlers ---
  handlePageChange(event: PageChangeEvent): void {
    this.currentPage = event.page;
    this.fetchGroups();
  }

  onPageSizeChange(value: number): void {
    this.pageSize = value;
    this.currentPage = 1;
    this.cachedCursors = { 1: null };
    this.fetchGroups();
  }

  // --- Fetch groups ---
  private fetchGroups(): void {
    this.isLoading = true;
    this.errorMessage = null;

    const nextCursor = this.cachedCursors[this.currentPage] ?? undefined;

    this.activeService
      .getQuestionnaireGroupsPaginated(this.pageSize, nextCursor, this.searchTitle)
      .subscribe({
        next: (res: QuestionnaireGroupKeysetPaginationResult) => {
          this.groups = res.groups;
          res.groups.forEach(g => {
            if (this.groupCollapsed[g.groupId] === undefined) {
              this.groupCollapsed[g.groupId] = true; // collapsed by default
            }
          });

          if (res.queryCursor) {
            this.cachedCursors[this.currentPage + 1] = res.queryCursor;
          }

          this.totalItems = res.totalCount;
          this.totalPages = Math.ceil(res.totalCount / this.pageSize);
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Kunne ikke indlæse spørgeskemagrupper.';
          this.isLoading = false;
        },
      });
  }

  toggleListCollapse(): void {
    Object.keys(this.groupCollapsed).forEach(id => {
      this.groupCollapsed[id] = !this.groupCollapsed[id];
    });
  }

  onCreateNewQuestionnaire(): void {
    this.createNewQuestionnaireEvent.emit();
  }
}
