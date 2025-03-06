import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { QuestionnaireSession } from '../../models/active.models';
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
  totalPages: number = 0; // Now we store the API-provided total pages

  // Instead of computing total pages from totalItems, we use the API value.
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
  activeQuestionnaires: QuestionnaireSession[] = [];

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
        this.currentPage = 1; // Reset page on new search
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
      this.fetchActiveQuestionnaires();
    }
  }
  

  // --- Page Size Change ---
  onPageSizeChange(value: number): void {
    this.pageSize = value;
    this.currentPage = 1; // Reset to the first page when page size changes
    this.fetchActiveQuestionnaires();
  }

  // --- Data Fetching ---
  private fetchActiveQuestionnaires(): void {
    this.isLoading = true;
    this.errorMessage = null; // Reset previous errors

    this.activeService
      .getActiveQuestionnaires(
        this.currentPage,
        this.pageSize,
        this.searchStudent,
        this.searchStudentType,
        this.searchTeacher,
        this.searchTeacherType
      )
      .subscribe({
        next: (response) => {
          this.activeQuestionnaires = response.items;
          this.totalItems = response.totalItems;
          this.totalPages = response.totalPages; // Use the API value here
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
