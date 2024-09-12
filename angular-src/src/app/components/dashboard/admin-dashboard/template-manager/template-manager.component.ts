import { Component } from '@angular/core';
import { AdminDashboardService } from '../../../../services/dashboard/admin-dashboard.service';
import { Question, QuestionTemplate, Option } from '../../../../models/questionare';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TemplateEditorComponent } from './template-editor/template-editor.component';
import { QuestionEditorComponent } from './template-editor/question-editor/question-editor.component';

@Component({
  selector: 'app-template-manager',
  standalone: true,
  imports: [CommonModule, FormsModule, TemplateEditorComponent, QuestionEditorComponent],
  templateUrl: './template-manager.component.html',
  styleUrl: './template-manager.component.css'
})
export class TemplateManagerComponent {
  templates: QuestionTemplate[] = [];
  selectedTemplate: QuestionTemplate | null = null;
  selectedQuestion: Question | null = null;

  constructor(private adminDashboardService: AdminDashboardService) {}

  ngOnInit(): void {
    this.loadTemplates();
  }


  handleAction(actionType: string, entity: any) {
    switch(actionType) {
      case 'editTemplate':
        this.editTemplate(entity);
        break;
      case 'deleteTemplate':
        this.deleteTemplate(entity.templateId);
        break;
      case 'editQuestion':
        this.editQuestion(entity);
        break;
      case 'deleteQuestion':
        this.deleteQuestion(entity.id);
        break;
    }
  }
  

  loadTemplates() {
    this.adminDashboardService.getTemplates().subscribe((templates) => {
      this.templates = templates;
    });
  }
  
  hasCustomOption(): boolean {
    return this.selectedQuestion ? this.selectedQuestion.options.some(o => o.isCustom) : false;
  }

  generateTemplateId(): string {
    const prefix = 'template';
    const timestamp = new Date().getTime(); // Current timestamp in milliseconds
    return `${prefix}_${timestamp}`;
  }

  createTemplate() {
    const newTemplate: QuestionTemplate = {
      templateId: this.generateTemplateId(),
      title: 'New Template',
      description: 'A description of the template',
      questions: [
        {
          id: 1,
          title: 'New Question 1',
          options: [{id: 1, label: "option 1", value: 1},{id: 2, label: "option 2", value: 2}]
        },
        {
          id: 2,
          title: 'New Question 2',
          options: [{id: 1, label: "option 1", value: 1},{id: 2, label: "option 2", value: 2}]
        }
      ],
      createdAt: new Date()
    };
  
    this.adminDashboardService.createTemplate(newTemplate).subscribe({
      complete: () => {
        this.loadTemplates();
      },
      error: (err) => {
        console.error('Error creating template:', err);
      }
    });
  }

  editTemplate(template: QuestionTemplate) {
    const confirmed = this.selectedTemplate && window.confirm('You have unsaved changes. Are you sure you want to switch templates?');
    if (!this.selectedTemplate || confirmed) {
      this.selectedTemplate = JSON.parse(JSON.stringify(template));
      this.selectedQuestion = null; // Clear any selected question when switching templates
    }
  }

  saveTemplate(updatedTemplate: QuestionTemplate) {
    const confirmed = window.confirm('Are you sure you want to save changes to this template?');
    if (confirmed) {
      this.adminDashboardService.updateTemplate(updatedTemplate).subscribe({
        error: (err) => console.error('Error updating template:', err),
        complete: () => {
          console.log('Template update complete.');
          this.loadTemplates();
          this.clearSelectedTemplate()
        }
      });
    }
  }

  clearSelectedTemplate() {
    this.selectedTemplate = null;
    this.selectedQuestion = null;
  }

  clearSelectedQuestion(){
    this.selectedQuestion = null;
  }

  deleteTemplate(templateId: string) {
    alert('Are you sure you want to delete this template? This action cannot be undone.')
    const confirmed = window.confirm('Remember, this is perment');
    if (confirmed) {
      this.adminDashboardService.deleteTemplate(templateId).subscribe({
        error: (err) => {
          console.error('Error deleting template:', err);
        },
        complete: () => {
          console.log('Template deletion complete.');
          this.loadTemplates(); // Reload the list after deletion
        }
      });
    }
  }

  editQuestion(question: Question) {
    const confirmed = this.selectedQuestion && window.confirm('You have unsaved changes. Are you sure you want to switch questions?');
    if (!this.selectedQuestion || confirmed) {
      this.selectedQuestion = JSON.parse(JSON.stringify(question));
    }
  }

  saveQuestion(updatedQuestion:Question) {
    if (this.selectedTemplate) {
      const confirmed = window.confirm('Are you sure you want to save changes to this question?');
      if (confirmed) {
        const index = this.selectedTemplate.questions.findIndex(q => q.id === this.selectedQuestion!.id);
        if (index !== -1) {
          this.selectedTemplate.questions[index] = { ...updatedQuestion }; // Save changes to the template's question list
          this.selectedQuestion = null; // Clear the selection after saving
        }
      }
    }
  }

  deleteQuestion(questionId: number) {
    const confirmed = window.confirm('Are you sure you want to delete this question? This action cannot be undone.');
    if (confirmed && this.selectedTemplate) {
      this.selectedTemplate.questions = this.selectedTemplate.questions.filter(q => q.id !== questionId);
      this.selectedQuestion = null; // Clear selection if deleting the edited question
    }
  }
}
