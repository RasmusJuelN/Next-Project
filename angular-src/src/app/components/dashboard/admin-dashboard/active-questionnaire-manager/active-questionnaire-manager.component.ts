import { Component, ViewChild, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActiveQuestionnaireBuilderComponent } from './active-questionnaire-builder/active-questionnaire-builder.component';
import { ActiveQuestionnaireListComponent } from './active-questionnaire-list/active-questionnaire-list.component';

@Component({
  selector: 'app-active-questionnaire-manager',
  standalone: true,
  imports: [CommonModule, ActiveQuestionnaireBuilderComponent, ActiveQuestionnaireListComponent],
  templateUrl: './active-questionnaire-manager.component.html',
  styleUrls: ['./active-questionnaire-manager.component.css', ]
})
export class ActiveQuestionnaireManagerComponent implements AfterViewInit {
  @ViewChild('activeList') activeList!: ActiveQuestionnaireListComponent; 

  
  ngAfterViewInit() {
    if(this.activeList){
      this.activeList?.searchActiveQuestionnaires();
    }
  }
  // Will Update the list of questonares.
  onQuestionnaireCreated() {
    if(this.activeList){
      this.activeList.onUpdate(); 
    }
  }
}
