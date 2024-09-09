import { CommonModule } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { Question, QuestionnaireMetadata } from '../../../models/questionare';

@Component({
  selector: 'app-question',
  standalone: true,
  imports: [FormsModule, CommonModule],
  templateUrl: './question.component.html',
  styleUrl: './question.component.css'
})
export class QuestionComponent {
  @Input() question: Question | null = null;
  @Input() selectedOption: number | undefined = undefined;
  @Output() optionSelected = new EventEmitter<number>();

  selectOption(optionId: number): void {
    this.optionSelected.emit(optionId);
  }
}