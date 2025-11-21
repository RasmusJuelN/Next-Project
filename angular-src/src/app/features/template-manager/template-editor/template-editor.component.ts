import { Component, EventEmitter, Input, Output, OnChanges } from '@angular/core';
import { QuestionEditorComponent } from './question-editor/question-editor.component';
import { Question, Template, TemplateStatus } from '../../../shared/models/template.model';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslateModule } from '@ngx-translate/core';
import { ModalComponent } from '../../../shared/components/modal/modal.component';
import { DragDropModule, CdkDragDrop, moveItemInArray } from '@angular/cdk/drag-drop';


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
    imports: [QuestionEditorComponent, CommonModule, FormsModule, ModalComponent, TranslateModule, DragDropModule],
    templateUrl: './template-editor.component.html',
    styleUrl: './template-editor.component.css'
})
export class TemplateEditorComponent implements OnChanges {
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
  /** Set to true when order changed via drag-drop; used to avoid auto-saving on drop */
  orderChanged = false;

  ngOnChanges() {
    this.readonly = this.template.templateStatus === TemplateStatus.Finalized;
    
    // Ensure questions and options are sorted by sortOrder when template changes
    if (this.template && this.template.questions) {
      this.sortTemplateData();
    }
  }

  /** Sorts questions and options by their sortOrder values. */
  private sortTemplateData() {
    if (this.template.questions) {
      this.template.questions.sort((a, b) => a.sortOrder - b.sortOrder);
      this.template.questions.forEach(question => {
        if (question.options) {
          question.options.sort((a, b) => a.sortOrder - b.sortOrder);
        }
      });
    }
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
      sortOrder: this.template.questions.length, // Set sort order to be at the end
      options: [],
    };
  
    this.template.questions.push(newQuestion);
  }
  // Select a question for editing
  editQuestion(question: Question): void {
    // Toggle editor: if the same question is already open, close it and run cancel cleanup
    if (this.selectedQuestion && this.selectedQuestion.id === question.id) {
      this.onCancelEdit(); // ensures any cancel cleanup behavior is executed
      return;
    }

    // If another question was open, discard its edits first
    if (this.selectedQuestion && this.selectedQuestion.id !== question.id) {
      this.onCancelEdit();
    }

    // Open editor for the clicked question (work on a deep copy so edits don't mutate original until Save)
    // Use JSON clone because Question is a simple data object (id, prompt, options[])
    this.selectedQuestion = JSON.parse(JSON.stringify(question)) as Question;
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

    // Re-index sortOrder for remaining questions
    this.template.questions.forEach((q, index) => {
      q.sortOrder = index;
    });
  }

  /**
   * Handle drop event from Angular CDK drag-and-drop.
   * Reorders the `template.questions` array locally and updates sort orders.
   */
  drop(event: CdkDragDrop<Question[]>) {
    // Update the array order in-place
    moveItemInArray(this.template.questions, event.previousIndex, event.currentIndex);

    // Update sortOrder for all questions to match the new array order
    this.template.questions.forEach((question, index) => {
      question.sortOrder = index;
    });

    // Mark that order changed but do NOT auto-emit saveTemplate here.
    // Emitting saveTemplate caused the parent to treat this as a full save
    // (and in your app that likely navigated back to the templates list).
    // Leave persistence to the user's explicit Save action.
    this.orderChanged = true;
  }
  
  onFinalize() { this.finalizeDraft.emit(this.template); }
  openFinalizeModal() { this.finalizeModalOpen = true; }
  closeFinalizeModal() { this.finalizeModalOpen = false; }

  confirmFinalize() {
    this.closeFinalizeModal();
    this.onFinalize();
  }

}
