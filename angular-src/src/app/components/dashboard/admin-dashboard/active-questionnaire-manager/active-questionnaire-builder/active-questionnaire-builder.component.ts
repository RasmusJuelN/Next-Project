import { CommonModule } from '@angular/common';
import { Component, EventEmitter, inject, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AdminDashboardService } from '../../../../../services/dashboard/admin-dashboard.service';
import { ActiveQuestionnaire, QuestionTemplate, User } from '../../../../../models/questionare';

@Component({
  selector: 'app-active-questionnaire-builder',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './active-questionnaire-builder.component.html',
  styleUrls: ['../active-questionnaire-manager.component.css','./active-questionnaire-builder.component.css']
})
export class ActiveQuestionnaireBuilderComponent {
  // State for search results and selected entities
  searchStudentResults: User[] = [];
  selectedStudent: User | null = null;
  searchStudentPage: number = 1; // Pagination for students
  hasMoreStudents: boolean = false;
  studentCacheCookie: string | undefined = undefined;
  
  searchTeacherResults: User[] = [];
  selectedTeacher: User | null = null;
  searchTeacherPage: number = 1; // Pagination for teachers
  hasMoreTeachers: boolean = false;
  teacherCacheCookie: string | undefined = undefined;

  searchTemplateResults: QuestionTemplate[] = [];
  selectedTemplateId: string | null = null;
  searchTemplatePage: number = 1; // Pagination for templates
  hasMoreTemplates: boolean = false;

  limit: number = 2; // How many students, teachers, and templates to load per page

  @Output() questionnaireCreated = new EventEmitter<void>();

  constructor(private adminDashboardService: AdminDashboardService) {}

  // Search students by name
  searchStudents(name: string | null | undefined) {
    if (name && name.length > 0) {
      this.searchStudentPage = 1; // Reset page number to 1
      this.searchStudentResults = []; // Clear previous search results

      this.adminDashboardService.getUsersFromSearch('student', name, this.searchStudentPage, this.limit)
        .subscribe(response => {
          this.searchStudentResults = response.users; // Access 'users' from the response
          this.studentCacheCookie = response.cacheCookie; // Store cache cookie
          this.hasMoreStudents = response.users.length >= this.limit; // Check if more results exist
        });
    } else {
      this.searchStudentResults = [];
    }
  }

  // Search templates by title
  searchTemplates(title: string | null | undefined) {
    if (title && title.length > 0) {
      this.searchTemplatePage = 1; // Reset page number to 1
      this.searchTemplateResults = []; // Clear previous search results

      this.adminDashboardService.getTemplatesFromSearch(title, this.searchTemplatePage, this.limit)
        .subscribe(results => {
          this.searchTemplateResults = results; // Replace with new results
          this.hasMoreTemplates = results.length >= this.limit; // Check if more results exist
        });
    } else {
      this.searchTemplateResults = [];
    }
  }

  // Search teachers by name
  searchTeachers(name: string | null | undefined) {
    if (name && name.length > 0) {
      this.searchTeacherPage = 1; // Reset page number to 1
      this.searchTeacherResults = []; // Clear previous search results

      this.adminDashboardService.getUsersFromSearch('teacher', name, this.searchTeacherPage, this.limit)
        .subscribe(response => {
          this.searchTeacherResults = response.users; // Access 'users' from the response
          this.teacherCacheCookie = response.cacheCookie; // Store cache cookie
          this.hasMoreTeachers = response.users.length >= this.limit; // Check if more results exist
        });
    } else {
      this.searchTeacherResults = [];
    }
  }

  loadMore(searchType: 'student' | 'teacher' | 'template', searchQuery: string) {
    switch (searchType) {
      case 'student':
        if (this.hasMoreStudents) {
          this.searchStudentPage++;
          this.adminDashboardService.getUsersFromSearch('student', searchQuery, this.searchStudentPage, this.limit, this.studentCacheCookie)
            .subscribe(response => {
              this.searchStudentResults = [...this.searchStudentResults, ...response.users]; // Append new results
              this.hasMoreStudents = response.users.length >= this.limit;
            });
        }
        break;
      case 'teacher':
        if (this.hasMoreTeachers) {
          this.searchTeacherPage++;
          this.adminDashboardService.getUsersFromSearch('teacher', searchQuery, this.searchTeacherPage, this.limit, this.teacherCacheCookie)
            .subscribe(response => {
              this.searchTeacherResults = [...this.searchTeacherResults, ...response.users]; // Append new results
              this.hasMoreTeachers = response.users.length >= this.limit;
            });
        }
        break;
        case 'template':
          if (this.hasMoreTemplates) {
            this.searchTemplatePage++;
            this.adminDashboardService.getTemplatesFromSearch(searchQuery, this.searchTemplatePage, this.limit)
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
      this.adminDashboardService.createActiveQuestionnaire(this.selectedStudent, this.selectedTeacher, this.selectedTemplateId)
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
    this.hasMoreStudents = true;
    this.hasMoreTeachers = true;
    this.hasMoreTemplates = true;
  }
}