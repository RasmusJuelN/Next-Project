import { Component, EventEmitter, Input, Output, OnChanges, SimpleChanges } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Answer, Question } from '../models/answer.model';
import { TranslateModule } from '@ngx-translate/core';


/**
 * Question component.
 *
 * Renders a single questionnaire question with options and optional
 * custom answer support. Emits answer changes back to the parent.
 *
 * Handles:
 * - Displaying the question prompt and available options.
 * - Tracking custom answer selection and input.
 * - Validating custom answer presence.
 * - Emitting updated `Answer` objects when selection changes.
 */
@Component({
  selector: 'app-question',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslateModule],
  templateUrl: './question.component.html',
  styleUrls: ['./question.component.css'],
})
export class QuestionComponent implements OnChanges {
  /** The current question to display. */
  @Input() question!: Question;

  /** The current answer for the question (if any). */
  @Input() answer!: Answer | undefined;

  /** Emits when the answer changes. */
  @Output() answerChange = new EventEmitter<Answer>();

  isCustomAnswerSelected: boolean = false;

  /** Value of the custom free-text answer. */
  customAnswer: string = '';

  /** Whether the custom answer is too short (validation flag). */
  ToFewCharacters: boolean = false;
  readonly maxCustomAnswerLength: number = 500;


  /**
   * Resets local state when question or answer input changes.
   * Keeps `isCustomAnswerSelected` and `customAnswer` in sync.
   */
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
    this.emitAnswer({ questionId: this.question.id, optionId: optionId });
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