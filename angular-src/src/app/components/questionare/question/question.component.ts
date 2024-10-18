import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { Question } from '../../../models/questionare';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-question',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css'],
})
export class QuestionComponent implements OnChanges {
  @Input() question: Question | null = null;
  @Input() selectedOption: number | undefined = undefined;
  @Output() optionSelected = new EventEmitter<{ optionId: number | undefined; customAnswer?: string }>();

  customAnswer: string | undefined = undefined;
  isCustomInput: boolean = false;

  ngOnChanges(changes: SimpleChanges): void {
    if (changes['question']) {
      const currentQuestion = this.question;
      if (currentQuestion) {
        const selectedOption = currentQuestion.options.find(
          option => option.id === currentQuestion.selectedOption
        );
        if (selectedOption?.isCustom) {
          this.selectedOption = undefined;
          this.isCustomInput = true;
          this.customAnswer = currentQuestion.customAnswer;
        } else {
          this.selectedOption = currentQuestion.selectedOption;
          this.isCustomInput = false;
          this.customAnswer = undefined;
        }
      } else {
        this.selectedOption = undefined;
        this.customAnswer = undefined;
        this.isCustomInput = false;
      }
    }
  }

  selectOption(optionId: number | undefined, isCustom: boolean | undefined) {
    if (isCustom) {
      this.selectedOption = undefined;
      this.isCustomInput = true;
      this.optionSelected.emit({ optionId: undefined });
    } else {
      this.selectedOption = optionId;
      this.isCustomInput = false;
      this.customAnswer = undefined;
      this.optionSelected.emit({ optionId: optionId });
    }
  }

  handleCustomInput(value: string) {
    this.customAnswer = value;
    this.optionSelected.emit({ optionId: undefined, customAnswer: value });
  }
}
