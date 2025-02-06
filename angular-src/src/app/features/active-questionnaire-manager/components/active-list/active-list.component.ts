import { Component, EventEmitter, inject, Input, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { QuestionnaireSession } from '../../models/active.models';
import { PaginationComponent } from '../../../../shared/components/pagination/pagination.component';
import { LoadingComponent } from '../../../../shared/loading/loading.component';

@Component({
  selector: 'app-active-list',
  standalone: true,
  imports: [CommonModule, FormsModule, PaginationComponent, LoadingComponent],
  templateUrl: './active-list.component.html',
  styleUrl: './active-list.component.css'
})
export class ActiveListComponent implements OnInit {
  private activeService = inject(ActiveService);

  pageSize: number = 5;
  searchStudent: string = '';
  searchTeacher: string = '';
  searchStudentType: 'fullName' | 'userName' | 'both' = 'both';
  searchTeacherType: 'fullName' | 'userName' | 'both' = 'both';
  currentPage: number = 1;
  totalItems: number = 0;
  hasNextPage: boolean = false;
  hasPreviousPage: boolean = false;
  isListCollapsed: boolean = false;

  isLoading = false; // Track loading state
  errorMessage: string | null = null; // Store error messages

  @Output() createNewQuestionnaireEvent = new EventEmitter<void>();

  activeQuestionnaires: QuestionnaireSession[] = [];
  private searchSubject = new Subject<{ student: string; studentType: string; teacher: string; teacherType: string }>();

  ngOnInit(): void {
    // Debounced search to prevent excessive API calls
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe(({ student, studentType, teacher, teacherType }) => {
        this.searchStudent = student;
        this.searchStudentType = studentType as 'fullName' | 'userName' | 'both';
        this.searchTeacher = teacher;
        this.searchTeacherType = teacherType as 'fullName' | 'userName' | 'both';
        this.currentPage = 1;
        this.fetchActiveQuestionnaires();
        console.log(this.currentPage)
      });

    this.fetchActiveQuestionnaires();
  }

  get totalPages(): number {
    return this.totalItems > 0 ? Math.ceil(this.totalItems / this.pageSize) : 0;
  }

  get pages(): number[] {
    const maxVisiblePages = 5;
    const pages: number[] = [];
  
    if (this.totalPages <= maxVisiblePages) {
      return Array.from({ length: this.totalPages }, (_, i) => i + 1);
    }
  
    const startPage = Math.max(1, this.currentPage - Math.floor(maxVisiblePages / 2));
    const endPage = Math.min(this.totalPages, startPage + maxVisiblePages - 1);
  
    if (startPage > 1) {
      pages.push(1);
      if (startPage > 2) {
        pages.push(-1); // Ellipsis
      }
    }
  
    for (let i = startPage; i <= endPage; i++) {
      pages.push(i);
    }
  
    if (endPage < this.totalPages) {
      if (endPage < this.totalPages - 1) {
        pages.push(-1);
      }
      pages.push(this.totalPages);
    }
  
    return pages;
  }
  toggleListCollapse(): void {
    this.isListCollapsed = !this.isListCollapsed;
  }
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

  onPageChange(newPage: number): void {
    if (newPage > 0 && newPage <= this.totalPages) {
      this.currentPage = newPage;
      this.fetchActiveQuestionnaires();
    }
  }

  onPageSizeChange(value: number): void {
    this.pageSize = value;
    this.currentPage = 1;
    this.fetchActiveQuestionnaires();
  }

  private fetchActiveQuestionnaires(): void {
    this.isLoading = true;
    this.errorMessage = null; // Reset any previous error
  
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
          this.hasNextPage = this.currentPage < this.totalPages;
          this.hasPreviousPage = this.currentPage > 1;
  
          this.isLoading = false;
        },
        error: (err) => {
          console.error('Error fetching active questionnaires:', err);
          this.errorMessage = 'Failed to load active questionnaires. Please try again.';
          this.isLoading = false;
        },
      });
  }

  previousPage(): void {
    if (this.currentPage > 1) {
      this.currentPage--;
      this.fetchActiveQuestionnaires();
    }
  }
  
  nextPage(): void {
    if (this.currentPage < this.totalPages) {
      this.currentPage++;
      this.fetchActiveQuestionnaires();
    }
  }

  jumpToPage(page: number): void {
    if (page > 0 && page <= this.totalPages) {
      this.currentPage = page;
      this.fetchActiveQuestionnaires();
    }
  }

  onCreateNewQuestionnaire(): void {
    this.createNewQuestionnaireEvent.emit();
  }
}
