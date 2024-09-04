import { Component } from '@angular/core';
import { ActiveQuestionnaire, QuestionTemplate, User } from '../../../../models/questionare';
import { AdminDashboardService } from '../../../../services/dashboard/admin-dashboard.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-active-questionnaire-manager',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './active-questionnaire-manager.component.html',
  styleUrl: './active-questionnaire-manager.component.css'
})
export class ActiveQuestionnaireManagerComponent {
  activeQuestionnaires: ActiveQuestionnaire[] = [];
  searchActiveQuestionnaireStudent: string = ""; // For searching by student name
  searchActiveQuestionnaireTeacher: string = ""; // For searching by teacher name
  page: number = 1; // Pagination current page
  limit: number = 10; // Results per page

  // State for search results and selected entities
  searchStudentResults: User[] = [];
  selectedStudent: User | null = null;

  searchTeacherResults: User[] = [];
  selectedTeacher: User | null = null;

  searchTemplateResults: QuestionTemplate[] = [];
  selectedTemplateId: string | null = null;

  constructor(private adminDashboardService: AdminDashboardService) {}

  ngOnInit(){
    this.searchActiveQuestionnaires();
  }

  // Fetch active questionnaires based on filters for student and teacher
  searchActiveQuestionnaires() {
    const filter = {
      searchStudent: this.searchActiveQuestionnaireStudent,
      searchTeacher: this.searchActiveQuestionnaireTeacher
    };

    this.adminDashboardService.getActiveQuestionnairePage(filter, this.page, this.limit)
      .subscribe({
        next: (results) => {
          if (this.page === 1) {
            this.activeQuestionnaires = results; // Replace results on first page
          } else {
            this.activeQuestionnaires = [...this.activeQuestionnaires, ...results]; // Append for subsequent pages
          }
        },
        error: (err) => {
          console.error('Error fetching active questionnaires', err);
        }
      });
  }

  // Handle search input change for student name
  onSearchStudentChange(studentName: string) {
    this.searchActiveQuestionnaireStudent = studentName;
    this.page = 1; // Reset to first page on new search
    this.searchActiveQuestionnaires();
  }

  // Handle search input change for teacher name
  onSearchTeacherChange(teacherName: string) {
    this.searchActiveQuestionnaireTeacher = teacherName;
    this.page = 1; // Reset to first page on new search
    this.searchActiveQuestionnaires();
  }

  // Load more results for pagination
  loadMore() {
    this.page++;
    this.searchActiveQuestionnaires();
  }

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
        this.searchActiveQuestionnaires();
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
