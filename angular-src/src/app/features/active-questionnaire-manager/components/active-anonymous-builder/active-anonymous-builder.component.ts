import { Component, EventEmitter, Output } from '@angular/core';

@Component({
    selector: 'app-active-anonymous-builder',
    imports: [],
    templateUrl: './active-anonymous-builder.component.html',
    styleUrl: './active-anonymous-builder.component.css'
})
export class ActiveAnonymousBuilderComponent {
participants: any[] = []; // Load all users (students, teachers, etc.)
  templates: any[] = []; // Load templates
  selectedParticipants: any[] = [];
  selectedTemplate: any = null;

  @Output() backToListEvent = new EventEmitter<void>();

  // Load participants and templates in ngOnInit (not shown for brevity)

  createAnonymousQuestionnaireGroup() {
    const payload = {
      participantIds: this.selectedParticipants.map(p => p.id),
      templateId: this.selectedTemplate
    };
    // Call your API endpoint for anonymous group creation
    // this.activeService.createAnonymousQuestionnaireGroup(payload).subscribe(...)
    this.backToListEvent.emit();
  }

  onBackToList() {
    this.backToListEvent.emit();
  }
}
