import { Component, inject, Input, OnInit } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { QuestionnaireSession } from '../../models/active.models';

@Component({
  selector: 'app-active-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './active-list.component.html',
  styleUrl: './active-list.component.css'
})
export class ActiveListComponent implements OnInit {
  private activeService = inject(ActiveService);

  @Input() pageSize: number = 5;
  searchStudent: string = '';
  searchTeacher: string = '';
  searchStudentType: 'fullName' | 'userName' | 'both' = 'both';
  searchTeacherType: 'fullName' | 'userName' | 'both' = 'both';
  currentPage: number = 1;
  totalItems: number = 0;
  hasNextPage: boolean = false;
  hasPreviousPage: boolean = false;

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
      });

    this.fetchActiveQuestionnaires();
  }

  get totalPages(): number {
    return Math.ceil(this.totalItems / this.pageSize);
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

  onPageSizeChange(value: string): void {
    this.pageSize = parseInt(value, 10);
    this.currentPage = 1;
    this.fetchActiveQuestionnaires();
  }

  private fetchActiveQuestionnaires(): void {
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
        },
        error: () => console.error('Error fetching active questionnaires'),
      });
  }

  previousPage(): void {
    if (this.hasPreviousPage) {
      this.currentPage--;
      this.fetchActiveQuestionnaires();
    }
  }

  nextPage(): void {
    if (this.hasNextPage) {
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
}
