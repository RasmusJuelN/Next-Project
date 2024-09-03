import { Component } from '@angular/core';
import { AdminDashboardService } from '../../../../services/dashboard/admin-dashboard.service';
import { Question, QuestionTemplate, Option } from '../../../../models/questionare';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-template-manager',
  standalone: true,
  imports: [CommonModule, FormsModule],
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
      questions: []
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

  saveTemplate() {
    const confirmed = window.confirm('Are you sure you want to save changes to this template?');
    if (confirmed && this.selectedTemplate) {
      this.adminDashboardService.updateTemplate(this.selectedTemplate).subscribe(() => {
        this.loadTemplates();
        this.selectedTemplate = null; // Clear selection after saving
        this.selectedQuestion = null; // Clear any selected question after saving
      });
    }
  }

  deleteTemplate(templateId: string) {
    const confirmed = window.confirm('Are you sure you want to delete this template? This action cannot be undone.');
    if (confirmed) {
      this.adminDashboardService.deleteTemplate(templateId).subscribe(() => {
        this.loadTemplates(); // Reload the list after deletion
      });
    }
  }

  addQuestion() {
    if (this.selectedTemplate) {
      const newQuestion: Question = {
        id: this.selectedTemplate.questions.length + 1,
        title: 'New Question',
        options: [{id: 1, label: "option 1", value: 1},{id: 2, label: "option 2", value: 2}]
      };
      this.selectedTemplate.questions.push(newQuestion);
      this.editQuestion(newQuestion);
    }
  }

  editQuestion(question: Question) {
    const confirmed = this.selectedQuestion && window.confirm('You have unsaved changes. Are you sure you want to switch questions?');
    if (!this.selectedQuestion || confirmed) {
      this.selectedQuestion = JSON.parse(JSON.stringify(question));
    }
  }

  saveQuestion() {
    if (this.selectedTemplate && this.selectedQuestion) {
      const confirmed = window.confirm('Are you sure you want to save changes to this question?');
      if (confirmed) {
        const index = this.selectedTemplate.questions.findIndex(q => q.id === this.selectedQuestion!.id);
        if (index !== -1) {
          this.selectedTemplate.questions[index] = { ...this.selectedQuestion }; // Save changes to the template's question list
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

  addOption(isCustom: boolean = false) {
    if (this.selectedQuestion) {
      if (isCustom) {
        const customOptionExists = this.selectedQuestion.options.some(o => o.isCustom);
        if (customOptionExists) {
          alert('A custom answer option already exists for this question.');
          return;
        }
      }

      const newOption: Option = {
        id: this.selectedQuestion.options.length + 1,
        value: isCustom ? 0 : this.selectedQuestion.options.length + 1, // Custom answer has value 0
        label: isCustom ? 'Custom Answer' : `Option ${this.selectedQuestion.options.length + 1}`,
        isCustom: isCustom
      };

      if (isCustom) {
        this.selectedQuestion.options.push(newOption);
      } else {
        const customIndex = this.selectedQuestion.options.findIndex(o => o.isCustom);
        if (customIndex !== -1) {
          this.selectedQuestion.options.splice(customIndex, 0, newOption);
        } else {
          this.selectedQuestion.options.push(newOption);
        }
      }
    }
  }

  deleteOption(optionId: number) {
    const confirmed = window.confirm('Are you sure you want to delete this option? This action cannot be undone.');
    if (confirmed && this.selectedQuestion) {
      const optionToDelete = this.selectedQuestion.options.find(o => o.id === optionId);
      const remainingOptions = this.selectedQuestion.options.filter(o => o.id !== optionId);
      const hasCustomOption = remainingOptions.some(o => o.isCustom);
      const ratingOptions = remainingOptions.filter(o => !o.isCustom);

      if ((hasCustomOption || ratingOptions.length >= 2) && optionToDelete) {
        this.selectedQuestion.options = remainingOptions;
      } else {
        alert('Cannot delete this option. Each question must have either at least two rating options or a custom answer option.');
      }
    }
  }
}
