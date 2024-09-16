import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Question, QuestionTemplate } from '../../../../../models/questionare';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { QuestionEditorComponent } from './question-editor/question-editor.component';

@Component({
  selector: 'app-template-editor',
  standalone: true,
  imports: [CommonModule, FormsModule, QuestionEditorComponent],
  templateUrl: './template-editor.component.html',
  styleUrl: './template-editor.component.css'
})
export class TemplateEditorComponent {
  @Input() template!: QuestionTemplate;
  @Output() save = new EventEmitter<QuestionTemplate>();
  @Output() cancel = new EventEmitter<void>();

  selectedQuestion: Question | null = null;

  saveTemplate() {
    this.save.emit(this.template);
  }

  cancelEdit() {
    this.cancel.emit();
  }

  addQuestion() {
    const newQuestion: Question = {
      id: this.template.questions.length +1,
      title: 'New Question',
      options: [{id: 1, label: "option 1", value: 1},{id: 2, label: "option 2", value: 2}]
    };
    this.template.questions.push(newQuestion);
    this.selectQuestionToEdit(newQuestion);
  }

  selectQuestionToEdit(question: Question) {
    this.selectedQuestion = JSON.parse(JSON.stringify(question));; // Edit a copy of the question
  }

  editQuestion(question: Question) {
    const confirmed = this.selectedQuestion && window.confirm('You have unsaved changes. Are you sure you want to switch questions?');
    if (!this.selectedQuestion || confirmed) {
      this.selectedQuestion = JSON.parse(JSON.stringify(question));
    }
  }
  
  deleteQuestion(questionId: number) {
    if (this.template.questions.length <= 2) {
      alert("You need at least two questions");
      return;
    }
  
    if (window.confirm('Are you sure you want to delete this question? This action cannot be undone.')) {
      this.template.questions = this.template.questions.filter(q => q.id !== questionId);
      this.selectedQuestion = null;  // Clear selection if the deleted question was being edited
    }
  }
  

  saveQuestion(updatedQuestion: Question) {
    if (window.confirm('Are you sure you want to save changes to this question?')) {
      const index = this.template.questions.findIndex(q => q.id === this.selectedQuestion?.id);
      
      if (index !== -1) {
        // Update the question in the array
        this.template.questions[index] = updatedQuestion;
        
        // Clear the selected question after saving
        this.clearSelectedQuestion();
      }
    }
  }

  clearSelectedQuestion() {
    this.selectedQuestion = null;
  }

}
