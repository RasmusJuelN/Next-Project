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
  groups: any[] = [];


  // Pagination and search state
  pageSize: number = 5;
  currentPage: number = 1;
  totalItems: number = 0;
  totalPages: number = 0;

  // Instead of a single queryCursor, we now cache cursors by page number.
  // Here we allow either a string or null.
  cachedCursors: { [pageNumber: number]: string | null } = {};

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
    // Initialize the cursor for page 1 as null (meaning no cursor)
    this.cachedCursors[1] = null;

    // this.activeService.getQuestionnaireGroup(this.groups[0].id).subscribe(group => {
    //   this.groups = [group];
    // });

    // Fetch all groups initially
    this.activeService.getQuestionnaireGroups().subscribe(groups => {
      this.groups = groups;
    });

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
        this.cachedCursors = { 1: null };
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
    // When moving forward and we don't have a cached cursor for the new page, fetch data to update it.
    if (event.direction === 'forward' && newPage > this.currentPage && !this.cachedCursors[newPage]) {
      this.currentPage = newPage;
      this.fetchActiveQuestionnaires();
      return;
    }
    this.currentPage = newPage;
    this.fetchActiveQuestionnaires();
  }
  
  onPageSizeChange(value: number): void {
    this.pageSize = value;
    this.currentPage = 1;
    this.cachedCursors = { 1: null };
    this.fetchActiveQuestionnaires();
  }

  private fetchActiveQuestionnaires(): void {
    this.isLoading = true;
    this.errorMessage = null;

    const nextCursor = this.cachedCursors[this.currentPage] ?? undefined;

    this.activeService
      .getActiveQuestionnaires(
        this.pageSize,
        nextCursor,
        this.searchStudent,
        this.searchStudentType,
        this.searchTeacher,
        this.searchTeacherType
      )
      .subscribe({
        next: (response) => {
          this.activeQuestionnaires = response.activeQuestionnaireBases;
          // If no items are returned, assume this is the last page.
          if (response.activeQuestionnaireBases.length === 0) {
            this.totalPages = this.currentPage;
            this.cachedCursors[this.currentPage + 1] = null;
          } else if (response.queryCursor) {
            this.cachedCursors[this.currentPage + 1] = response.queryCursor;
            this.totalItems = response.totalCount;
            this.totalPages = Math.ceil(response.totalCount / this.pageSize);
          } else {
            this.totalPages = this.currentPage;
          }
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = 'Kunne ikke indlæse aktive spørgeskemaer. Prøv venligst igen.';
          this.isLoading = false;
        },
      });
  }

  toggleListCollapse(): void {
    this.isListCollapsed = !this.isListCollapsed;
  }

  onCreateNewQuestionnaire(): void {
    this.createNewQuestionnaireEvent.emit();
  }
}
