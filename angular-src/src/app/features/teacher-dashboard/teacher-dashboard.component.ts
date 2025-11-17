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
import { TranslateModule } from '@ngx-translate/core';

/**
 * Teacher dashboard component.
 *
 * Provides an overview of active questionnaires for teachers.
 *
 * Handles:
 * - Debounced search by student name or questionnaire id.
 * - Cursor-based pagination with cached cursors per page.
 * - Filtering by student/teacher completion.
 * - Copy-to-clipboard for the questionnaire answer link.
 * - Navigation to answer/results when applicable.
 */
@Component({
    selector: 'app-teacher-dashboard',
    imports: [ClipboardModule, FormsModule, CommonModule, PaginationComponent, RouterLink, LoadingComponent, TranslateModule],
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
  // cachedCursors: { [pageNumber: number]: string | null } = { 1: null };

  // Filters
  filterStudentCompleted = false;
  filterTeacherCompleted = false;

  // Data to display
  // displayedQuestionnaires: ActiveQuestionnaireBase[] = [];
  displayedGroups: {
    groupId: string;
    groupName: string; 
    questionnaires: ActiveQuestionnaireBase[];
  }[] = [];

  groupCollapsed: { [groupId: string]: boolean } = {};

  isLoading = false;
  errorMessage: string | null = null;

  ngOnInit(): void {
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((term) => {
        this.searchTerm = term;
        this.currentPage = 1; // Reset to first page on search
        this.updateDisplay();
      });

    this.updateDisplay();
  }

  private updateDisplay(): void {
    this.isLoading = true;
    this.errorMessage = null;
    console.log('🔵 Frontend sending:', { currentPage: this.currentPage, pageSize: this.pageSize });
    this.teacherService
      .getQuestionnaireGroups(
        
        this.currentPage,      // Pass page number directly
        this.pageSize,
        this.searchTerm,
        this.filterStudentCompleted,
        this.filterTeacherCompleted
      )
      .subscribe({
        next: (res) => {
          console.log('🟢 Backend returned:', { currentPage: res.currentPage, totalPages: res.totalPages, totalCount: res.totalCount });
          const groups = res.groups ?? [];
          this.displayedGroups = groups.map((g: any) => ({
            groupId: g.groupId,
            groupName: g.groupName ?? g.name ?? g.Name ?? 'Ungrouped',
            templateId: g.templateId ?? g.TemplateId ?? null,
            questionnaires: (g.questionnaires ?? []).map((q: any) => ({
              id: q.id,
              title: q.title,
              description: q.description,
              activatedAt: new Date(q.activatedAt),
              studentCompletedAt: q.studentCompletedAt ? new Date(q.studentCompletedAt) : null,
              teacherCompletedAt: q.teacherCompletedAt ? new Date(q.teacherCompletedAt) : null,
              student: q.student,
              teacher: q.teacher,
              templateId: q.templateId ?? q.TemplateId ?? null
            }))
          }));

          // Initialize collapse state
          this.displayedGroups.forEach(g => {
            if (this.groupCollapsed[g.groupId] === undefined) {
              this.groupCollapsed[g.groupId] = true;
            }
          });

          this.totalItems = res.totalCount ?? 0;
          this.totalPages = res.totalPages ?? Math.ceil(this.totalItems / this.pageSize);

          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Failed to load data. Please try again.';
          this.isLoading = false;
        }
      });
  }

  onSearchChange(term: string): void {
    this.searchSubject.next(term);
  }

  onCompletionFilterChange(): void {
    this.currentPage = 1;
    this.updateDisplay();
  }

  onPageSizeChange(newSize: string): void {
    this.pageSize = parseInt(newSize, 10);
    this.currentPage = 1;
    this.updateDisplay();
  }

  isSelectedPageSize(size: number): boolean {
    return size === this.pageSize;
  }

  onPageChange(event: PageChangeEvent): void {
    this.currentPage = event.page;  // Jump to any page directly!
    this.updateDisplay();
    console.log('Page changed to:', this.currentPage);
  }

  copyAnswersUrl(id: string): void {
    const url = `${window.location.origin}/answer/${id}`;
    this.clipboard.copy(url);
  }

  toggleGroupCollapse(groupId: string) {
    this.groupCollapsed[groupId] = !this.groupCollapsed[groupId];
  }

  getAnsweredCount(questionnaires: ActiveQuestionnaireBase[] = [], role: 'student' | 'teacher'): number {
    if (!questionnaires || questionnaires.length === 0) return 0;
    return role === 'student'
      ? questionnaires.filter(q => !!q.studentCompletedAt).length
      : questionnaires.filter(q => !!q.teacherCompletedAt).length;

  }
}
