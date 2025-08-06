import { Component } from '@angular/core';
import { ActiveQuestionnaire } from './models/active.models';

import { FormsModule } from '@angular/forms';
import { ActiveListComponent } from './components/active-list/active-list.component';
import { ActiveBuilderComponent } from './components/active-builder/active-builder.component';

@Component({
    selector: 'app-active-questionnaire-manager',
    imports: [FormsModule, ActiveListComponent, ActiveBuilderComponent],
    templateUrl: './active-questionnaire-manager.component.html',
    styleUrl: './active-questionnaire-manager.component.css'
})
export class ActiveQuestionnaireManagerComponent {
  searchStudent: string = '';
  searchTeacher: string = '';
  pageSize: number = 5;

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

  handleCreateNewQuestionnaire() {
    this.showBuilder = true;
  }

  showList() {
    this.showBuilder = false;
  }
}
