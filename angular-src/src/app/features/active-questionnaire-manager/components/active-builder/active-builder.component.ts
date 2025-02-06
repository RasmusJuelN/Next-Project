import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { User } from '../../../../shared/models/user.model';
import { Template } from '../../../template-manager/models/template.model';
import { PaginationResponse } from '../../../../shared/models/Pagination.model';

@Component({
  selector: 'app-active-questionnaire-builder',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './active-builder.component.html',
  styleUrl: './active-builder.component.css'
})
export class ActiveBuilderComponent implements OnInit {
  private activeService = inject(ActiveService);

  // Selected Data
  selectedStudent: User | null = null;
  selectedTeacher: User | null = null;
  selectedTemplate: Template | null = null;

  // Search Input
  searchStudent: string = '';
  searchTeacher: string = '';
  searchTemplate: string = '';

  // Search Results
  searchStudentResults: User[] = [];
  searchTeacherResults: User[] = [];
  searchTemplateResults: Template[] = [];

  // Pagination
  studentPage: number = 1;
  teacherPage: number = 1;
  templatePage: number = 1;

  totalStudentPages: number = 1;
  totalTeacherPages: number = 1;
  totalTemplatePages: number = 1;

  isLoadingStudents = false;
  isLoadingTeachers = false;
  isLoadingTemplates = false;

  errorMessageStudents: string | null = null;
  errorMessageTeachers: string | null = null;
  errorMessageTemplates: string | null = null;

  // Search Subjects (for debouncing API calls)
  private searchStudentSubject = new Subject<string>();
  private searchTeacherSubject = new Subject<string>();
  private searchTemplateSubject = new Subject<string>();

  @Output() backToListEvent = new EventEmitter<void>();

  ngOnInit(): void {
    // Debounced search to prevent excessive API calls
    this.searchStudentSubject.pipe(debounceTime(300), distinctUntilChanged()).subscribe((term) => {
      this.fetchStudents(term);
    });

    this.searchTeacherSubject.pipe(debounceTime(300), distinctUntilChanged()).subscribe((term) => {
      this.fetchTeachers(term);
    });

    this.searchTemplateSubject.pipe(debounceTime(300), distinctUntilChanged()).subscribe((term) => {
      this.fetchTemplates(term);
    });
  }

  // Fetch students with pagination handling
  fetchStudents(term: string, isLoadMore: boolean = false): void {
    if (!term.trim()) return;
    if (!isLoadMore) {
      this.studentPage = 1;
      this.searchStudentResults = [];
    }

    this.isLoadingStudents = true;
    this.activeService.searchUsers(term, 'student', this.studentPage).subscribe({
      next: (response: PaginationResponse<User>) => {
        this.searchStudentResults = isLoadMore
          ? [...this.searchStudentResults, ...response.items]
          : response.items;

        this.totalStudentPages = response.totalPages;
        this.isLoadingStudents = false;
      },
      error: () => {
        this.errorMessageStudents = 'Failed to load students.';
        this.isLoadingStudents = false;
      }
    });
  }

  // Fetch teachers with pagination handling
  fetchTeachers(term: string, isLoadMore: boolean = false): void {
    if (!term.trim()) return;
    if (!isLoadMore) {
      this.teacherPage = 1;
      this.searchTeacherResults = [];
    }

    this.isLoadingTeachers = true;
    this.activeService.searchUsers(term, 'teacher', this.teacherPage).subscribe({
      next: (response: PaginationResponse<User>) => {
        this.searchTeacherResults = isLoadMore
          ? [...this.searchTeacherResults, ...response.items]
          : response.items;

        this.totalTeacherPages = response.totalPages;
        this.isLoadingTeachers = false;
      },
      error: () => {
        this.errorMessageTeachers = 'Failed to load teachers.';
        this.isLoadingTeachers = false;
      }
    });
  }

  // Fetch templates with pagination handling
  fetchTemplates(term: string, isLoadMore: boolean = false): void {
    if (!term.trim()) return;
    if (!isLoadMore) {
      this.templatePage = 1;
      this.searchTemplateResults = [];
    }

    this.isLoadingTemplates = true;
    this.activeService.searchTemplates(term, this.templatePage).subscribe({
      next: (response: PaginationResponse<Template>) => {
        this.searchTemplateResults = isLoadMore
          ? [...this.searchTemplateResults, ...response.items]
          : response.items;

        this.totalTemplatePages = response.totalPages;
        this.isLoadingTemplates = false;
      },
      error: () => {
        this.errorMessageTemplates = 'Failed to load templates.';
        this.isLoadingTemplates = false;
      }
    });
  }

  // Check if more data exists
  canLoadMoreStudents(): boolean {
    return this.studentPage < this.totalStudentPages;
  }

  canLoadMoreTeachers(): boolean {
    return this.teacherPage < this.totalTeacherPages;
  }

  canLoadMoreTemplates(): boolean {
    return this.templatePage < this.totalTemplatePages;
  }

  // Load more functions
  loadMoreStudents(): void {
    if (this.canLoadMoreStudents()) {
      this.studentPage++;
      this.fetchStudents(this.searchStudent, true);
    }
  }

  loadMoreTeachers(): void {
    if (this.canLoadMoreTeachers()) {
      this.teacherPage++;
      this.fetchTeachers(this.searchTeacher, true);
    }
  }

  loadMoreTemplates(): void {
    if (this.canLoadMoreTemplates()) {
      this.templatePage++;
      this.fetchTemplates(this.searchTemplate, true);
    }
  }

  // Handle search input change
  onStudentInputChange(value: string): void {
    this.searchStudent = value;
    this.searchStudentSubject.next(value);
  }

  onTeacherInputChange(value: string): void {
    this.searchTeacher = value;
    this.searchTeacherSubject.next(value);
  }

  onTemplateInputChange(value: string): void {
    this.searchTemplate = value;
    this.searchTemplateSubject.next(value);
  }

  // Select Student
  selectStudent(student: User): void {
    this.selectedStudent = student;
    this.searchStudent = ''; // Clear input but KEEP search results
  }

  // Select Teacher
  selectTeacher(teacher: User): void {
    this.selectedTeacher = teacher;
    this.searchTeacher = ''; // Clear input but KEEP search results
  }

  // Select Template
  selectTemplate(template: Template): void {
    this.selectedTemplate = template;
    this.searchTemplate = ''; // Clear input but KEEP search results
  }

  // Clear selections
  clearSelectedStudent(): void {
    this.selectedStudent = null;
  }

  clearSelectedTeacher(): void {
    this.selectedTeacher = null;
  }

  clearSelectedTemplate(): void {
    this.selectedTemplate = null;
  }

  // Create Active Questionnaire
  createActiveQuestionnaire(): void {
    if (!this.selectedStudent || !this.selectedTeacher || !this.selectedTemplate) return;

    const newQuestionnaire = {
      studentId: this.selectedStudent.id,
      teacherId: this.selectedTeacher.id,
      templateId: this.selectedTemplate.id,
    };

    this.activeService.createActiveQuestionnaire(newQuestionnaire).subscribe(() => {
      alert('Active Questionnaire Created Successfully!');
      this.backToListEvent.emit();
    });
  }

  onBackToList(): void {
    this.backToListEvent.emit();
  }
}