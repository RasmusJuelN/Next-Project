import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Answer, Question } from '../models/answer.model';

@Component({
  selector: 'app-question',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css'],
})
export class QuestionComponent implements OnChanges {
  @Input() question!: Question; // The current question to display
  @Input() answer!: Answer | undefined; // The current answer for the question
  @Output() answerChange = new EventEmitter<Answer>(); // Emits when the answer changes

  isCustomAnswerSelected: boolean = false;
  customAnswer: string = '';
  ToFewCharacters: boolean = false;
  readonly maxCustomAnswerLength: number = 500;

  ngOnChanges(changes: SimpleChanges): void {
    // When the question changes, reset the custom answer state
    if (changes['question']) {
      this.isCustomAnswerSelected = false;
      this.customAnswer = '';
    }
    
    // Update internal state if the answer changes
    if (changes['answer']) {
      const newAnswer = changes['answer'].currentValue;
      if (newAnswer) {
        if (newAnswer.customAnswer !== undefined && newAnswer.customAnswer.trim() !== '') {
          this.isCustomAnswerSelected = true;
          this.customAnswer = newAnswer.customAnswer;
        } else if (newAnswer.selectedOptionId !== undefined) {
          this.isCustomAnswerSelected = false;
          this.customAnswer = '';
        }
      } else {
        // If there's no answer, clear the state
        this.isCustomAnswerSelected = false;
        this.customAnswer = '';
      }
    }
  }
  

  // Handle standard option selection
  onOptionSelect(optionId: number): void {
    this.isCustomAnswerSelected = false;
    this.customAnswer = '';
    this.emitAnswer({ questionId: this.question.id, selectedOptionId: optionId });
  }

  // Handle custom answer selection
  onCustomAnswerSelect(): void {
    this.isCustomAnswerSelected = true; // Ensure the flag is set
    this.ToFewCharacters = true;
    this.emitAnswer({ questionId: this.question.id, customAnswer: '' });
  }

  // Handle custom answer input
  onCustomAnswerChange(): void {
    this.ToFewCharacters = this.customAnswer.trim() === ''; // Check if custom answer is required
    this.emitAnswer({ questionId: this.question.id, customAnswer: this.customAnswer });
  }

  // Emit the updated answer
  private emitAnswer(updatedAnswer: Answer): void {
    this.answerChange.emit(updatedAnswer);
  }
}