import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Question, Option } from '../../models/template.model';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-question-editor',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './question-editor.component.html',
  styleUrls: ['./question-editor.component.css']
})
export class QuestionEditorComponent {
  @Input() question!: Question; // Accept the question to edit
  @Output() save = new EventEmitter<Question>(); // Emit event when saving
  @Output() cancel = new EventEmitter<void>(); // Emit event when canceling

  validationErrors: string[] = []; // Store validation errors

  // Add a new option to the question
  addOption(): void {
    const newOption: Option = {
      id: -1 * (this.question.options.length + 1),
      displayText: 'New Option',
      optionValue: 0
    };
    this.question.options.push(newOption);
  }

  // Delete an option from the question
  deleteOption(optionId: number): void {
    this.question.options = this.question.options.filter(option => option.id !== optionId);
  }

  // Validate the question before saving
  validateQuestion(): boolean {
    this.validationErrors = []; // Clear previous errors

    // 1. Ensure the title is not empty
    if (!this.question.prompt || this.question.prompt.trim() === '') {
      this.validationErrors.push('The question title cannot be empty.');
    }

    // 2. Ensure there are options or a custom answer is allowed
    if (!this.question.allowCustom && (!this.question.options || this.question.options.length === 0)) {
      this.validationErrors.push('The question must allow a custom answer or have at least one option.');
    }

    // 3. Ensure options have unique, non-empty labels
    const labels = this.question.options.map(option => option.displayText.trim());
    const duplicateLabels = labels.filter((label, index) => labels.indexOf(label) !== index);
    if (labels.includes('') || duplicateLabels.length > 0) {
      this.validationErrors.push('All options must have unique, non-empty labels.');
    }

    return this.validationErrors.length === 0; // Return true if no errors
  }

  // Emit save event if valid
  onSave(): void {
    if (this.validateQuestion()) {
      this.save.emit(this.question);
    }
  }

  // Emit cancel event
  onCancel(): void {
    this.cancel.emit();
  }
}
