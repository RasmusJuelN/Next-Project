import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { Answer, AnswerSession } from '../models/questionare';
// Define the User interface
export interface User {
  id: number; // ID of the user from the database
  userName: string; // Username of the user
  fullName: string; // Full name of the user
  role: string; // Role of the user (e.g., 'student', 'teacher')
}


@Component({
  selector: 'app-show-answers',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './show-answers.component.html',
  styleUrls: ['./show-answers.component.css']
})
export class ShowAnswersComponent {
   // Mock data for example purposes
   answerSession: AnswerSession = {
    questionnaireId: 'Q12345',
    studentAnswers: {
      user: { id: 1, userName: 'JD', fullName: 'John Doe', role: 'student' },
      answers: [
        { questionId: 1, selectedOptionId: 1 },
        { questionId: 2, selectedOptionId: 2 },
        { questionId: 3, customAnswer: 'More interactive activities would be great!' }
      ],
      answeredAt: new Date()
    },
    teacherAnswers: {
      user: { id: 2, userName: 'JS', fullName: 'Jane Smith', role: 'teacher' },
      answers: [
        { questionId: 1, selectedOptionId: 4 },
        { questionId: 2, selectedOptionId: 4 },
        { questionId: 3, customAnswer: 'Better resources and tools for students are needed.' }
      ],
      answeredAt: new Date()
    }
  };

  // Mock questions data
  questions = [
    { id: 1, title: 'What is your favorite subject?' },
    { id: 2, title: 'Creativity and Independence' },
    { id: 3, title: 'Any additional feedback?' }
  ];

  // Option labels for selected options (1-5 scale)
  optionLabels: Record<number, string> = {
    1: '1 - Very low',
    2: '2 - Below average',
    3: '3 - Shows normal initiative',
    4: '4 - Very initiative-driven',
    5: '5 - Excellent initiative'
  };

  // Helper function to get the question title by its ID
  getQuestionTitle(questionId: number): string {
    const question = this.questions.find(q => q.id === questionId);
    return question ? question.title : 'Unknown question';
  }

  // Helper function to get the label of the selected option
  getOptionLabel(optionId?: number): string {
    if (!optionId) return 'No Answer';
    return this.optionLabels[optionId] || 'Unknown option';
  }

  // Helper function to compare student and teacher answers
  compareAnswers(studentAnswer: Answer, teacherAnswer: Answer): string | null {
    if (studentAnswer.selectedOptionId !== teacherAnswer.selectedOptionId) {
      return 'Both rated differently.';
    }
    if (studentAnswer.customAnswer && teacherAnswer.customAnswer && studentAnswer.customAnswer !== teacherAnswer.customAnswer) {
      return 'Both provided different feedback.';
    }
    return null;
  }

  // Helper function to handle "No Answer" scenarios
  displayAnswer(answer: Answer): string {
    return answer.customAnswer ? answer.customAnswer : this.getOptionLabel(answer.selectedOptionId);
  }

  // Helper function to show comparison notes
  showComparison(studentAnswer: Answer, teacherAnswer: Answer): string {
    const comparison = this.compareAnswers(studentAnswer, teacherAnswer);
    return comparison ? comparison : 'Both answered the same.';
  }
}