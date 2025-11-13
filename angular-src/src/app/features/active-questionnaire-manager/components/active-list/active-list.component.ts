import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { QuestionnaireGroupResult, QuestionnaireGroupKeysetPaginationResult } from '../../models/active.models';

import { PageChangeEvent, PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { LoadingComponent } from '../../../../shared/loading/loading.component';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-active-list',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent, LoadingComponent, TranslateModule],
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
  // cachedCursors: { [pageNumber: number]: string | null } = {1: null};

  // Search filters
  searchTitle: string = '';

  // UI state
  isListCollapsed: boolean = false;
  isLoading = false;
  errorMessage: string | null = null;
  filterPendingStudent = false;
  filterPendingTeacher = false;

  @Output() createNewQuestionnaireEvent = new EventEmitter<void>();

  private searchSubject = new Subject<{ title: string }>();

  getAnsweredCount(questionnaires: any[], role: 'student' | 'teacher'): number {
    return questionnaires.filter(q =>
      role === 'student' ? q.studentCompletedAt : q.teacherCompletedAt
    ).length;
  }

  ngOnInit(): void {
    // Debounced search
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(({ title }) => {
        this.searchTitle = title;
        this.currentPage = 1; 
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
    this.fetchGroups();
  }

  // --- Fetch groups ---
  private fetchGroups(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.activeService
      .getQuestionnaireGroupsPaginated(
        this.currentPage,
        this.pageSize,
        this.searchTitle,
        this.filterPendingStudent,
        this.filterPendingTeacher
      )
      .subscribe({
        next: (res) => {
          this.groups = res.groups;
          
          // Initialize collapse state
          res.groups.forEach(g => {
            if (this.groupCollapsed[g.groupId] === undefined) {
              this.groupCollapsed[g.groupId] = true;
            }
          });

          this.totalItems = res.totalCount;
          this.totalPages = res.totalPages;
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Kunne ikke indlæse spørgeskemagrupper.';
          this.isLoading = false;
        },
      });
  }

  onFilterChange(): void {
    this.currentPage = 1;
    this.fetchGroups();
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
