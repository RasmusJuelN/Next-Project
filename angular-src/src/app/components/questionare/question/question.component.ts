import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Question } from '../../../models/questionare';

@Component({
  selector: 'app-question',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css']
})
export class QuestionComponent {
  @Input() question: Question | null = null; // The question input
  @Input() selectedOption: number | undefined = undefined; // Selected option ID
  @Output() optionSelected = new EventEmitter<{ optionId: number | undefined, customAnswer?: string }>();

  customAnswer: string | undefined = undefined; // Store custom answer text (string | undefined)
  isCustomInput: boolean = false; // Track if custom input is selected

  // Select option or enable custom input if needed
  selectOption(optionId: number | undefined, isCustom: boolean | undefined) {
    this.selectedOption = optionId;
    this.isCustomInput = !!isCustom; // Set isCustomInput based on the selected option's isCustom flag

    if (!isCustom) {
      this.optionSelected.emit({ optionId: optionId, customAnswer: undefined }); // Emit selected option
    }
  }

  // Handle custom input
  handleCustomInput(value: string) {
    this.customAnswer = value;
    this.optionSelected.emit({ optionId: undefined, customAnswer: value }); // Emit custom answer
  }
}
