import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

import { PageChangeEvent, PaginationComponent } from '../../shared/components/pagination/pagination.component';
import { Dashboard } from './models/dashboard.model';
import { TeacherService } from './services/teacher.service';
import { PaginationResponse } from '../../shared/models/Pagination.model';
import { LoadingComponent } from '../../shared/loading/loading.component';

@Component({
  selector: 'app-teacher-dashboard',
  standalone: true,
  imports: [FormsModule, CommonModule, PaginationComponent, RouterLink, LoadingComponent],
  templateUrl: './teacher-dashboard.component.html',
  styleUrls: ['./teacher-dashboard.component.css']
})
export class TeacherDashboardComponent implements OnInit {
  private teacherService = inject(TeacherService)
  // Search
  searchTerm: string = '';
  searchType: string = 'name';
  private searchSubject = new Subject<string>();

  // Pagination
  currentPage: number = 1;
  pageSize: number = 5;
  pageSizeOptions: number[] = [5, 10, 15, 20];
  totalItems: number = 0;
  totalPages: number = 1; // We'll store totalPages from the service response

  filterStudentCompleted = false;
  filterTeacherCompleted = false;
  // Data to display
  displayedQuestionnaires: Dashboard[] = [];

  isLoading = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    // Debounce search
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.currentPage = 1;
        this.updateDisplay();
      });

    // Initial load
    this.updateDisplay();
  }

  /**
   * Calls the service with current filters/pagination,
   * and populates this.displayedQuestionnaires, totalItems, totalPages, etc.
   */
  private updateDisplay(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.teacherService
      .getQuestionnaires(
        this.searchTerm,
        this.searchType,
        this.currentPage,
        this.pageSize,
        this.filterStudentCompleted,
        this.filterTeacherCompleted
      )
      .subscribe({
        next: (response: PaginationResponse<Dashboard>) => {
          this.displayedQuestionnaires = response.items;
          this.totalItems = response.totalItems;
          this.totalPages = response.totalPages;
          this.isLoading = false;
        },
        error: (err) => {
          this.errorMessage = 'Failed to load data. Please try again.';
          this.isLoading = false;
        }
      });
  }

  // Called when search input changes
  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }

  // Called when user changes the search type dropdown
  onSearchTypeChange(newType: string): void {
    this.searchType = newType;
    this.currentPage = 1;
    this.updateDisplay();
  }
  
  onCompletionFilterChange(): void {
    this.currentPage = 1; // reset page
    this.updateDisplay();
  }

  // Called when user changes page size
  onPageSizeChange(newSize: string): void {
    this.pageSize = parseInt(newSize, 10);
    this.currentPage = 1;
    this.updateDisplay();
  }

  isSelectedPageSize(size: number): boolean {
    return size === this.pageSize;
  }

  // Called when pagination component changes page
  onPageChange(event: PageChangeEvent): void {
    this.currentPage = event.page;
    this.updateDisplay();
  }
  
}
