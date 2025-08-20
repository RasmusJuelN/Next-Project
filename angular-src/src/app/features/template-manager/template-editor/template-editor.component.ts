import { Component, EventEmitter, Input, Output } from '@angular/core';
import { QuestionEditorComponent } from './question-editor/question-editor.component';
import { Question, Template, TemplateStatus } from '../models/template.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ModalComponent } from '../../../shared/components/modal/modal.component';

@Component({
  selector: 'app-template-editor',
  standalone: true,
  imports: [QuestionEditorComponent, CommonModule, FormsModule, ModalComponent],
  templateUrl: './template-editor.component.html',
  styleUrl: './template-editor.component.css'
})
export class TemplateEditorComponent {
  @Input() template!: Template; // Input property to receive a template
  @Output() saveTemplate = new EventEmitter<Template>(); // Output event for saving changes
  @Output() finalizeDraft  = new EventEmitter<Template>();

  @Output() cancelEdit = new EventEmitter<void>(); // Output event for canceling the edit
  selectedQuestion: Question | null = null;
  readonly = false;

  finalizeModalOpen = false;

  ngOnChanges() {
    this.readonly = this.template.draftStatus === TemplateStatus.Finalized;
  }

  // Method to emit the saveTemplate event with the updated template
  onSave() {
    this.saveTemplate.emit(this.template);
  }

  // Method to emit the cancelEdit event
  onCancel() {
    this.cancelEdit.emit();
  }
  
  addQuestion() {
    const newQuestion: Question = {
      id: -1 * (this.template.questions.length + 1), // Unique negative ID
      prompt: 'New Question',
      allowCustom: true,
      options: [],
    };
  
    this.template.questions.push(newQuestion);
  }
  // Select a question for editing
  editQuestion(question: Question): void {
    this.selectedQuestion = { ...question }; // Create a copy to avoid modifying the original directly
  }

  // Save the edited question
  onSaveQuestion(updatedQuestion: Question): void {
    console.log(updatedQuestion)
    // Find the index of the question in the template
    const questionIndex = this.template.questions.findIndex(q => q.id === updatedQuestion.id);

    if (questionIndex > -1) {
      // Update the question in the template's questions array
      this.template.questions[questionIndex] = { ...updatedQuestion };
    }

    this.selectedQuestion = null; // Close the editor
  }

  // Cancel editing
  onCancelEdit(): void {
    this.selectedQuestion = null; // Close the editor
  }

  deleteQuestion(question: Question): void {
    this.template.questions = this.template.questions.filter(q => q.id !== question.id);
  }
  
  onFinalize() { this.finalizeDraft.emit(this.template); }
  openFinalizeModal() { this.finalizeModalOpen = true; }
  closeFinalizeModal() { this.finalizeModalOpen = false; }

  confirmFinalize() {
    this.closeFinalizeModal();
    this.onFinalize();
  }

}
