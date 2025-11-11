import { Component, EventEmitter, Input, Output } from '@angular/core';
import { QuestionEditorComponent } from './question-editor/question-editor.component';
import { Question, Template, TemplateStatus } from '../../../shared/models/template.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ModalComponent } from '../../../shared/components/modal/modal.component';

/**
 * Template editor component.
 *
 * Provides an interface for editing a single questionnaire template.
 *
 * Handles:
 * - Displaying and updating template title/description.
 * - Adding, editing, and deleting questions.
 * - Saving or canceling template edits.
 * - Finalizing (publishing) a draft template.
 * - Switching to readonly mode if the template is finalized.
 */
@Component({
    selector: 'app-template-editor',
    imports: [QuestionEditorComponent, CommonModule, FormsModule, ModalComponent, TranslateModule],
    templateUrl: './template-editor.component.html',
    styleUrl: './template-editor.component.css'
})
export class TemplateEditorComponent {
  /** Template being edited. */
  @Input() template!: Template;

  /** Emits when the template is saved. */
  @Output() saveTemplate = new EventEmitter<Template>();

  /** Emits when the draft is finalized. */
  @Output() finalizeDraft  = new EventEmitter<Template>();

  /** Emits when editing is canceled. */
  @Output() cancelEdit = new EventEmitter<void>();

  /** Currently selected question (if editing one). */
  selectedQuestion: Question | null = null;

  /** True if the template is finalized (readonly mode). */
  readonly = false;

  finalizeModalOpen = false;

  ngOnChanges() {
    this.readonly = this.template.templateStatus === TemplateStatus.Finalized;
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
