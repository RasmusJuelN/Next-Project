import { Component } from '@angular/core';
import { ActiveQuestionnaire } from './models/active.models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveListComponent } from './components/active-list/active-list.component';
import { ActiveBuilderComponent } from './components/active-builder/active-builder.component';


/**
 * Switching between the list view and builder view
 */
@Component({
    selector: 'app-active-questionnaire-manager',
    imports: [CommonModule, FormsModule, ActiveListComponent, ActiveBuilderComponent],
    templateUrl: './active-questionnaire-manager.component.html',
    styleUrl: './active-questionnaire-manager.component.css'
})
export class ActiveQuestionnaireManagerComponent {
  searchStudent: string = '';
  searchTeacher: string = '';
  pageSize: number = 5;

  /** Whether to show the builder instead of the list view. */
  showBuilder: boolean = false;

  onSearchStudentChange(value: string) {
    this.searchStudent = value;
  }

  onSearchTeacherChange(value: string) {
    this.searchTeacher = value;
  }

  onPageSizeChange(value: number) {
    this.pageSize = value;
  }

  /** Switches to the builder view
   * after creating new Questionnaire. */
  handleCreateNewQuestionnaire() {
    this.showBuilder = true;
  }

  showList() {
    this.showBuilder = false;
  }
}
