import { Component, ElementRef, EventEmitter, Input, Output, ViewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { Question, Option } from '../../../../shared/models/template.model';
import { TranslateModule } from '@ngx-translate/core';

@Component({
  selector: 'app-question-editor',
  standalone: true,
  imports: [FormsModule, CommonModule, TranslateModule],
  templateUrl: './question-editor.component.html',
  styleUrls: ['./question-editor.component.css']
})
export class QuestionEditorComponent {
  @Input() question!: Question;
  @Input() readonly = false;
  @Output() save = new EventEmitter<Question>();
  @Output() cancel = new EventEmitter<void>();

  validationErrors: string[] = [];
  @ViewChild('errorContainer') errorContainer!: ElementRef;

  addOption(): void {
    if (this.readonly) { return; }
    const newOption: Option = {
      id: -1 * (this.question.options.length + 1),
      displayText: 'New Option',
      optionValue: 0
    };
    this.question.options.push(newOption);
  }

  deleteOption(optionId: number): void {
    if (this.readonly) { return; }
    this.question.options = this.question.options.filter(option => option.id !== optionId);
  }

  validateQuestion(): boolean {
    console.log(this.question)
    this.validationErrors = []; // Clear previous errors
  
    // 1. Ensure the title is not empty
    if (!this.question.prompt || this.question.prompt.trim() === '') {
      this.validationErrors.push('The question title cannot be empty.');
    }
  
    // 2. Ensure there are options or a custom answer is allowed
    if (!this.question.allowCustom && (!this.question.options || this.question.options.length === 0)) {
      this.validationErrors.push('The question must allow a custom answer or have at least one option.');
    }
  
    // 3. Ensure options have non-empty labels
    const hasEmptyLabel = this.question.options.some(option => option.displayText.trim() === '');
    if (hasEmptyLabel) {
      this.validationErrors.push('All options must have non-empty labels.');
    }
  
    return this.validationErrors.length === 0; // Return true if no errors
  }
  

  onSave(): void {
    if (this.readonly) { return; }
    if (this.validateQuestion()) {
      this.save.emit(this.question);
    } else {
      // Delay scrolling to allow the error container to render or else requries clicking twice
      setTimeout(() => {
        if (this.errorContainer) {
          this.errorContainer.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'start' });
        }
      }, 0);
    }
  }

  // Emit cancel event
  onCancel(): void {
    this.cancel.emit();
  }
}
