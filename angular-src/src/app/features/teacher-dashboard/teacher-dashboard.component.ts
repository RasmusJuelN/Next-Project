import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { Clipboard, ClipboardModule } from '@angular/cdk/clipboard'; 
import { PageChangeEvent, PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { ActiveQuestionnaireBase, ActiveQuestionnaireResponse } from './models/dashboard.model';
import { TeacherService } from './services/teacher.service';
import { LoadingComponent } from '../../shared/loading/loading.component';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [ClipboardModule,FormsModule, CommonModule, PaginationComponent, RouterLink, LoadingComponent],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.css']
})
export class TeacherDashboardComponent implements OnInit {
  private teacherService = inject(TeacherService);
  private clipboard = inject(Clipboard); // For copying active questionaire id
  // Search state
  searchTerm: string = '';
  // "name" will search by student name; "id" will search by active questionnaire ID.
  searchType: 'name' | 'id' = 'name';
  private searchSubject = new Subject<string>();

  // Pagination state
  currentPage: number = 1;
  pageSize: number = 5;
  pageSizeOptions: number[] = [5, 10, 15, 20];
  totalItems: number = 0;
  totalPages: number = 1;

  // Cache cursors by page number; page 1 starts with a null cursor.
  cachedCursors: { [pageNumber: number]: string | null } = { 1: null };

  // Filters
  filterStudentCompleted = false;
  filterTeacherCompleted = false;

  // Data to display
  displayedQuestionnaires: ActiveQuestionnaireBase[] = [];

  isLoading = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    // Debounce search input to reduce API calls.
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.currentPage = 1;
        this.cachedCursors = { 1: null };
        this.updateDisplay();
      });

    // Initial load.
    this.updateDisplay();
  }

  /**
   * Calls the service with current filters/pagination and updates
   * the displayed questionnaires, cached cursors, totalItems, and totalPages.
   */
  private updateDisplay(): void {
    this.isLoading = true;
    this.errorMessage = null;

    // Get the cursor for the current page (or null for first page).
    const queryCursor = this.cachedCursors[this.currentPage] ?? null;

    this.teacherService
      .getQuestionnaires(
        this.searchTerm,
        this.searchType,
        queryCursor,
        this.pageSize,
        this.filterStudentCompleted,
        this.filterTeacherCompleted
      )
      .subscribe({
        next: (response: ActiveQuestionnaireResponse) => {
          // Update the displayed items and pagination state.
          this.displayedQuestionnaires = response.activeQuestionnaireBases;
          this.totalItems = response.totalCount;
          this.totalPages = Math.ceil(response.totalCount / this.pageSize);

          // Cache the cursor for the next page if provided.
          if (response.queryCursor !== null) {
            this.cachedCursors[this.currentPage + 1] = response.queryCursor;
          } else {
            this.cachedCursors[this.currentPage + 1] = null;
          }
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = 'Failed to load data. Please try again.';
          this.isLoading = false;
        }
      });
  }

  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }

  onSearchTypeChange(newType: string): void {
    this.searchType = newType as 'name' | 'id';
    this.currentPage = 1;
    this.cachedCursors = { 1: null };
    this.updateDisplay();
  }
  
  onCompletionFilterChange(): void {
    this.currentPage = 1;
    this.cachedCursors = { 1: null };
    this.updateDisplay();
  }
  
  onPageSizeChange(newSize: string): void {
    this.pageSize = parseInt(newSize, 10);
    this.currentPage = 1;
    this.cachedCursors = { 1: null };
    this.updateDisplay();
  }

  isSelectedPageSize(size: number): boolean {
    return size === this.pageSize;
  }

  onPageChange(event: PageChangeEvent): void {
    this.currentPage = event.page;
    this.updateDisplay();
  }

  copyAnswersUrl(id: string): void {
    const url = `${window.location.origin}/answer/${id}`;
    this.clipboard.copy(url);
    console.log(`URL ${url} copied to clipboard!`);
  }
}
