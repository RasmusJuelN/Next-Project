import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Question, Option } from '../../../../../../models/questionare';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-question-editor',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './question-editor.component.html',
  styleUrl: './question-editor.component.css'
})
export class QuestionEditorComponent {
  @Input() question!: Question;
  @Output() save = new EventEmitter<Question>();
  @Output() cancel = new EventEmitter<void>();

  addOption(isCustom: boolean = false) {
    if (this.question) {
      // Check if a custom option already exists for the current question
      if (isCustom) {
        const customOptionExists = this.question.options.some(o => o.isCustom);
        if (customOptionExists) {
          alert('A custom answer option already exists for this question.');
          return;
        }
      }
  
      // Create a new option
      const newOption: Option = {
        id: 0,
        value: isCustom ? 0 : this.question.options.length + 1, // Custom answer has value 0
        label: isCustom ? 'Custom Answer' : `Option ${this.question.options.length + 1}`,
        isCustom: isCustom
      };
  
      // If the new option is custom, push it directly to the list
      if (isCustom) {
        this.question.options.push(newOption);
      } else {
        // If it is not custom, insert the normal option before any custom option, if one exists
        const customIndex = this.question.options.findIndex(o => o.isCustom);
        if (customIndex !== -1) {
          // Insert the normal option before the custom option
          this.question.options.splice(customIndex, 0, newOption);
        } else {
          // No custom option exists, push the normal option to the end
          this.question.options.push(newOption);
        }
      }
    }
  }
  

  deleteOption(optionId: number) {
    const remainingOptions = this.question.options.filter(o => o.id !== optionId);
    const hasCustomOption = remainingOptions.some(o => o.isCustom);
    const ratingOptionsCount = remainingOptions.filter(o => !o.isCustom).length;
  
    // Check first if the option can be deleted
    if (hasCustomOption || ratingOptionsCount >= 2) {
      if (window.confirm('Are you sure you want to delete this option? This action cannot be undone.')) {
        this.question.options = remainingOptions;
      }
    } else {
      alert('Cannot delete this option. Each question must have either at least two rating options or a custom answer option.');
    }
  }
  

  saveQuestion() {
    this.save.emit(this.question);
  }

  cancelEdit() {
    this.cancel.emit();
  }
}