import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { ActiveQuestionnaire, ActiveQuestionnaireBase } from '../../models/active.models';
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

  // Pagination and search state
  pageSize: number = 5;
  currentPage: number = 1;
  totalItems: number = 0;
  totalPages: number = 0;
  queryCursor: string = '';

  // Instead of computing total pages from totalItems, we use the API value (or compute it)
  get computedTotalPages(): number {
    return this.totalPages;
  }

  // Search filters
  searchStudent: string = '';
  searchTeacher: string = '';
  searchStudentType: 'fullName' | 'userName' | 'both' = 'both';
  searchTeacherType: 'fullName' | 'userName' | 'both' = 'both';

  // UI state
  isListCollapsed: boolean = false;
  isLoading = false;
  errorMessage: string | null = null;

  // Output for creating new questionnaires
  @Output() createNewQuestionnaireEvent = new EventEmitter<void>();

  // Data list
  activeQuestionnaires: ActiveQuestionnaireBase[] = [];

  // Debounced search subject
  private searchSubject = new Subject<{
    student: string;
    studentType: string;
    teacher: string;
    teacherType: string;
  }>();

  ngOnInit(): void {
    // Debounce search inputs to avoid flooding the API
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(({ student, studentType, teacher, teacherType }) => {
        this.searchStudent = student;
        this.searchStudentType = studentType as 'fullName' | 'userName' | 'both';
        this.searchTeacher = teacher;
        this.searchTeacherType = teacherType as 'fullName' | 'userName' | 'both';
        // Reset pagination cursor and page on new search
        this.currentPage = 1;
        this.queryCursor = '';
        this.fetchActiveQuestionnaires();
      });

    // Initial data load
    this.fetchActiveQuestionnaires();
  }

  // --- Search Methods ---
  onSearchStudentChange(value: string): void {
    this.searchSubject.next({
      student: value,
      studentType: this.searchStudentType,
      teacher: this.searchTeacher,
      teacherType: this.searchTeacherType,
    });
  }

  onSearchStudentTypeChange(value: string): void {
    this.searchSubject.next({
      student: this.searchStudent,
      studentType: value,
      teacher: this.searchTeacher,
      teacherType: this.searchTeacherType,
    });
  }

  onSearchTeacherChange(value: string): void {
    this.searchSubject.next({
      student: this.searchStudent,
      studentType: this.searchStudentType,
      teacher: value,
      teacherType: this.searchTeacherType,
    });
  }

  onSearchTeacherTypeChange(value: string): void {
    this.searchSubject.next({
      student: this.searchStudent,
      studentType: this.searchStudentType,
      teacher: this.searchTeacher,
      teacherType: value,
    });
  }

  // --- Pagination Handler ---
  handlePageChange(event: PageChangeEvent): void {
    const newPage = event.page;
    if (newPage > 0 && newPage <= this.totalPages) {
      this.currentPage = newPage;
      // For cursor-based pagination, you might need to store a mapping of page numbers to cursors.
      // Here we assume that moving to a different page resets the cursor.
      this.fetchActiveQuestionnaires();
    }
  }
  
  // --- Page Size Change ---
  onPageSizeChange(value: number): void {
    this.pageSize = value;
    this.currentPage = 1; // Reset to the first page when page size changes
    this.queryCursor = '';
    this.fetchActiveQuestionnaires();
  }

  // --- Data Fetching ---
  private fetchActiveQuestionnaires(): void {
    this.isLoading = true;
    this.errorMessage = null; // Reset previous errors

    this.activeService
      .getActiveQuestionnaires(
        this.pageSize,
        this.queryCursor,
        this.searchStudent,
        this.searchStudentType,
        this.searchTeacher,
        this.searchTeacherType
      )
      .subscribe({
        next: (response) => {
          // Note: updated interface returns 'templateBases' instead of the previous property name
          this.activeQuestionnaires = response.activeQuestionnaireBase;
          // Update the query cursor from the API response (if provided)
          this.queryCursor = response.queryCursor || '';
          this.totalItems = response.totalCount;
          // Compute total pages from total count and page size
          this.totalPages = Math.ceil(response.totalCount / this.pageSize);
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = 'Failed to load active questionnaires. Please try again.';
          this.isLoading = false;
        },
      });
  }

  // --- Other UI Actions ---
  toggleListCollapse(): void {
    this.isListCollapsed = !this.isListCollapsed;
  }

  onCreateNewQuestionnaire(): void {
    this.createNewQuestionnaireEvent.emit();
  }
}
