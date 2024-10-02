import { Component, inject } from '@angular/core';
import { ActiveQuestionnaire } from '../../../../../models/questionare';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../../../../services/data/data.service';

@Component({
  selector: 'app-active-questionnaire-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './active-questionnaire-list.component.html',
  styleUrls: ['../active-questionnaire-manager.component.css',"./active-questionnaire-list.component.css"]
})
export class ActiveQuestionnaireListComponent {
  private dataService = inject(DataService)
  activeQuestionnaires: ActiveQuestionnaire[] = [];
  searchActiveQuestionnaireStudent: string = ""; // For searching by student name
  searchActiveQuestionnaireTeacher: string = ""; // For searching by teacher name
  page: number = 1; // Pagination current page
  limit: number = 8; // Results per page

  ngOnInit(){
    this.searchActiveQuestionnaires();
  }

  // Fetch active questionnaires based on filters for student and teacher
  searchActiveQuestionnaires() {
    const filter = {
      searchStudent: this.searchActiveQuestionnaireStudent,
      searchTeacher: this.searchActiveQuestionnaireTeacher
    };
  
    // Fetch active questionnaires from the service
    this.dataService.getActiveQuestionnairePage(filter, this.page, this.limit)
      .subscribe({
        next: (results: ActiveQuestionnaire[]) => {
          if (this.page === 1) {
            this.activeQuestionnaires = results; // Replace results on the first page
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

  // Method to handle reloading after a new questionnaire is created
  onUpdate() {
    this.page = 1; // Reset to first page
    this.searchActiveQuestionnaires(); // Reload the list of active questionnaires
  }
}