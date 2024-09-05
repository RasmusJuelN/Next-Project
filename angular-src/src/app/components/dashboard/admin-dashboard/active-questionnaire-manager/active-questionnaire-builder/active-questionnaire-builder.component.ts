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

  searchTeacherResults: User[] = [];
  selectedTeacher: User | null = null;

  searchTemplateResults: QuestionTemplate[] = [];
  selectedTemplateId: string | null = null;

  @Output() questionnaireCreated = new EventEmitter<void>();

  constructor(private adminDashboardService: AdminDashboardService) {}

  // Search students by name
  searchStudents(name: string | null | undefined) {
    if (name && name.length > 0) {
      this.adminDashboardService.getUsersFromSearch('student', name)
        .subscribe(results => {
          this.searchStudentResults = results;
        });
    } else {
      this.searchStudentResults = [];
    }
  }

  // Search templates by title
searchTemplates(title: string | null | undefined) {
  if (title && title.length > 0) {
    this.adminDashboardService.getTemplatesFromSearch(title)
      .subscribe(results => {
        this.searchTemplateResults = results;
      });
  } else {
    this.searchTemplateResults = [];
  }
}

  // Search teachers by name
  searchTeachers(name: string | null | undefined) {
    if (name && name.length > 0) {
      this.adminDashboardService.getUsersFromSearch('teacher', name)
        .subscribe(results => {
          this.searchTeacherResults = results;
        });
    } else {
      this.searchTeacherResults = [];
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
  }
}