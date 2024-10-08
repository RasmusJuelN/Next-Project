import { Component, inject } from '@angular/core';
import { ActiveQuestionnaire } from '../../../../../models/questionare';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { DataService } from '../../../../../services/data/data.service';
import { Subject } from 'rxjs';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'app-active-questionnaire-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './active-questionnaire-list.component.html',
  styleUrls: ['../active-questionnaire-manager.component.css', './active-questionnaire-list.component.css']
})
export class ActiveQuestionnaireListComponent {
  private dataService = inject(DataService);
  activeQuestionnaires: ActiveQuestionnaire[] = [];
  searchActiveQuestionnaireStudent: string = ''; // For searching by student name
  searchActiveQuestionnaireTeacher: string = ''; // For searching by teacher name
  page: number = 1; // Pagination current page
  limit: number = 8; // Results per page

  // Subjects for managing debounced input
  private studentSearchSubject = new Subject<string>();
  private teacherSearchSubject = new Subject<string>();

  ngOnInit() {
    this.searchActiveQuestionnaires();

    // Debounce for student search input
    this.studentSearchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((studentName) => {
        console.log(`Debounced student search triggered with name: ${studentName}`);
        this.searchActiveQuestionnaireStudent = studentName;
        this.page = 1; // Reset to the first page on new search
        this.searchActiveQuestionnaires();
      });

    // Debounce for teacher search input
    this.teacherSearchSubject
      .pipe(debounceTime(300), distinctUntilChanged())
      .subscribe((teacherName) => {
        console.log(`Debounced teacher search triggered with name: ${teacherName}`);
        this.searchActiveQuestionnaireTeacher = teacherName;
        this.page = 1; // Reset to the first page on new search
        this.searchActiveQuestionnaires();
      });
  }

  // Handle search input change for student name
  onSearchStudentChange(studentName: string) {
    this.studentSearchSubject.next(studentName);
  }

  // Handle search input change for teacher name
  onSearchTeacherChange(teacherName: string) {
    this.teacherSearchSubject.next(teacherName);
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
