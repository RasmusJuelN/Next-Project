import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActiveQuestionnaire, QuestionTemplate, User } from '../../../../../models/questionare';
import { DataService } from '../../../../../services/data/data.service';
import { debounceTime, distinctUntilChanged, Subject } from 'rxjs';

@Component({
  selector: 'app-active-questionnaire-builder',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './active-questionnaire-builder.component.html',
  styleUrls: ['../active-questionnaire-manager.component.css','./active-questionnaire-builder.component.css']
})
export class ActiveQuestionnaireBuilderComponent {
  private dataService = inject(DataService);

  // State for search results and selected entities
  searchStudentResults: User[] = [];
  searchTeacherResults: User[] = [];
  searchTemplateResults: QuestionTemplate[] = [];

  selectedStudent: User | null = null;
  selectedTeacher: User | null = null;
  selectedTemplateId: string | null = null;

  searchStudentPage: number = 1;
  searchTeacherPage: number = 1;
  searchTemplatePage: number = 1;

  hasMoreStudents: boolean = false;
  hasMoreTeachers: boolean = false;
  hasMoreTemplates: boolean = false;

  teacherCacheCookie: string | undefined = undefined;
  studentCacheCookie: string | undefined = undefined;

  limit: number = 2;

  // Subjects to manage input changes and debounce
  private studentSearchSubject = new Subject<string>();
  private teacherSearchSubject = new Subject<string>();
  private templateSearchSubject = new Subject<string>();

  @Output() questionnaireCreated = new EventEmitter<void>();

  ngOnInit(): void {
    // Debounce for student search
    this.studentSearchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((searchText) => {
        this.searchStudents(searchText);
      });
  
    // Debounce for teacher search
    this.teacherSearchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((searchText) => {
        this.searchTeachers(searchText);
      });
  
    // Debounce for template search
    this.templateSearchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((searchText) => {
        this.searchTemplates(searchText);
      });
  }

  // Trigger search for students with debounce
  onStudentInputChange(searchText: string) {
    this.studentSearchSubject.next(searchText);
  }

  // Trigger search for teachers with debounce
  onTeacherInputChange(searchText: string) {
    this.teacherSearchSubject.next(searchText);
  }

  // Trigger search for templates with debounce
  onTemplateInputChange(searchText: string) {
    this.templateSearchSubject.next(searchText);
  }

  searchStudents(name: string | null | undefined) {
    if (name && name.length > 0) {
      this.searchStudentPage = 1;
      this.searchStudentResults = [];
      this.dataService.getUsersFromSearch('student', name, this.searchStudentPage, this.limit)
        .subscribe(response => {
          this.searchStudentResults = response.users;
          this.hasMoreStudents = response.users.length >= this.limit;
        });
    } else {
      this.searchStudentResults = [];
    }
  }

  searchTeachers(name: string | null | undefined) {
    if (name && name.length > 0) {
      this.searchTeacherPage = 1;
      this.searchTeacherResults = [];

      this.dataService.getUsersFromSearch('teacher', name, this.searchTeacherPage, this.limit)
        .subscribe(response => {
          this.searchTeacherResults = response.users;
          this.hasMoreTeachers = response.users.length >= this.limit;
        });
    } else {
      this.searchTeacherResults = [];
    }
  }

  searchTemplates(title: string | null | undefined) {
    if (title && title.length > 0) {
      this.searchTemplatePage = 1;
      this.searchTemplateResults = [];

      this.dataService.getTemplates(this.searchTemplatePage, this.limit, title)
        .subscribe(results => {
          this.searchTemplateResults = results;
          this.hasMoreTemplates = results.length >= this.limit;
        });
    } else {
      this.searchTemplateResults = [];
    }
  }

  loadMore(searchType: 'student' | 'teacher' | 'template', searchQuery: string) {
    switch (searchType) {
      case 'student':
        if (this.hasMoreStudents) {
          this.searchStudentPage++;
          this.dataService.getUsersFromSearch('student', searchQuery, this.searchStudentPage, this.limit)
            .subscribe(response => {
              this.searchStudentResults = [...this.searchStudentResults, ...response.users]; // Append new results
              this.hasMoreStudents = response.users.length >= this.limit;
            });
        }
        break;
      case 'teacher':
        if (this.hasMoreTeachers) {
          this.searchTeacherPage++;
          this.dataService.getUsersFromSearch('teacher', searchQuery, this.searchTeacherPage, this.limit)
            .subscribe(response => {
              this.searchTeacherResults = [...this.searchTeacherResults, ...response.users]; // Append new results
              this.hasMoreTeachers = response.users.length >= this.limit;
            });
        }
        break;
        case 'template':
          if (this.hasMoreTemplates) {
            this.searchTemplatePage++;
            this.dataService.getTemplates(this.searchTemplatePage, this.limit, searchQuery)
              .subscribe(results => {
                this.searchTemplateResults = [...this.searchTemplateResults, ...results];
                this.hasMoreTemplates = results.length >= this.limit;
              });
          }
          break;
    }
  }

  // Handle selection of student
  selectStudent(student: User) {
    this.selectedStudent = student;
    this.searchStudentResults = []; // Clear search results
  }

  // Handle selection of teacher
  selectTeacher(teacher: User) {
    this.selectedTeacher = teacher;
    this.searchTeacherResults = []; // Clear search results
  }

  // Handle selection of template
  selectTemplate(template: QuestionTemplate) {
    this.selectedTemplateId = template.templateId;
    this.searchTemplateResults = []; // Clear search results
  }

  // Create active questionnaire
  createActiveQuestionnaire() {
    if (this.selectedStudent && this.selectedTeacher && this.selectedTemplateId) {
      this.dataService.createActiveQuestionnaire(this.selectedStudent, this.selectedTeacher, this.selectedTemplateId)
        .subscribe(data => {
          this.resetSelections();
        });
        this.questionnaireCreated.emit();
    } else {
      alert('Please select a student, teacher, and template.');
    }
  }

  // Reset selections after creation
  private resetSelections() {
    this.selectedStudent = null;
    this.selectedTeacher = null;
    this.selectedTemplateId = null;
    this.searchStudentPage = 1;
    this.searchTeacherPage = 1;
    this.searchTemplatePage = 1;
    this.hasMoreStudents = false;
    this.hasMoreTeachers = false;
    this.hasMoreTemplates = false;
  }
}