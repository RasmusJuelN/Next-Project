import { Component, EventEmitter, inject, OnInit, Output } from '@angular/core';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { User } from '../../../../shared/models/user.model';
import { Template } from '../../../template-manager/models/template.model';


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

  // Fetch students based on search term
  fetchStudents(term: string): void {
    this.activeService.searchUsers(term, 'student', this.studentPage).subscribe((response) => {
      this.searchStudentResults = response.items;
    });
  }

  // Fetch teachers based on search term
  fetchTeachers(term: string): void {
    this.activeService.searchUsers(term, 'teacher', this.teacherPage).subscribe((response) => {
      this.searchTeacherResults = response.items;
    });
  }

  // Fetch templates based on search term
  fetchTemplates(term: string): void {
    this.activeService.searchTemplates(term, this.templatePage).subscribe((response) => {
      this.searchTemplateResults = response.items;
    });
  }

  // Handle Student Input Change
  onStudentInputChange(value: string): void {
    this.searchStudent = value;
    this.searchStudentSubject.next(value);
  }

  // Handle Teacher Input Change
  onTeacherInputChange(value: string): void {
    this.searchTeacher = value;
    this.searchTeacherSubject.next(value);
  }

  // Handle Template Input Change
  onTemplateInputChange(value: string): void {
    this.searchTemplate = value;
    this.searchTemplateSubject.next(value);
  }

  // Select Student
  selectStudent(student: User): void {
    this.selectedStudent = student;
  }

  // Select Teacher
  selectTeacher(teacher: User): void {
    this.selectedTeacher = teacher;
  }

  // Select Template
  selectTemplate(template: Template): void {
    this.selectedTemplate = template;
  }

  // Load More Students
  loadMoreStudents(): void {
    this.studentPage++;
    this.fetchStudents(this.searchStudent);
  }

  // Load More Teachers
  loadMoreTeachers(): void {
    this.teacherPage++;
    this.fetchTeachers(this.searchTeacher);
  }

  // Load More Templates
  loadMoreTemplates(): void {
    this.templatePage++;
    this.fetchTemplates(this.searchTemplate);
  }

  // Create Active Questionnaire
  createActiveQuestionnaire(): void {
    if (!this.selectedStudent || !this.selectedTeacher || !this.selectedTemplate) {
      return;
    }

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
