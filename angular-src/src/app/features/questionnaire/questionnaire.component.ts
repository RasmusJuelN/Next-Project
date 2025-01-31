import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute } from '@angular/router';
import { QuestionComponent } from './question/question.component';
import { AnswerService } from './services/answer.service';
import { Answer, QuestionnaireState } from './models/answer.model';

@Component({
  selector: 'app-answer-questionnaire',
  standalone: true,
  imports: [CommonModule, QuestionComponent],
  templateUrl: './questionnaire.component.html',
  styleUrls: ['./questionnaire.component.css'],
})
export class QuestionnaireComponent implements OnInit {
  private answerService = inject(AnswerService);
  private route = inject(ActivatedRoute);

  state: QuestionnaireState = {
    template: {
      id: '',
      title: '',
      description: '',
      questions: [],
      createdAt: new Date(),
    },
    currentQuestionIndex: 0,
    answers: [],
    progress: 0,
    isCompleted: false,
  };

  ngOnInit() {
    this.route.paramMap.subscribe((params) => {
      const questionnaireId = params.get('id');
      if (questionnaireId) {
        this.loadQuestionnaire(questionnaireId);
      } else {
        console.error('No questionnaire ID found in route!');
      }
    });
  }

  private loadQuestionnaire(id: string) {
    this.answerService.getActiveQuestionnaireById(id).subscribe((template) => {
      if (template) {
        this.state.template = template;
        this.updateProgress();
      } else {
        console.error('Template not found!');
      }
    });
  }

  get currentQuestion() {
    return this.state.template.questions[this.state.currentQuestionIndex];
  }

  get currentAnswer(): Answer | undefined {
    return this.state.answers.find(
      (a) => a.questionId === this.currentQuestion.id
    );
  }

  get allQuestionsAnswered(): boolean {
    return this.state.template.questions.every((question) =>
      this.state.answers.some(
        (answer) =>
          answer.questionId === question.id &&
          (!!answer.selectedOptionId || !!answer.customAnswer?.trim())
      )
    );
  }

  get isAnswered(): boolean {
    const answer = this.currentAnswer;
    return !!answer && (!!answer.selectedOptionId || !!answer.customAnswer?.trim());
  }

  onAnswerChange(answer: Answer): void {
    const existingIndex = this.state.answers.findIndex(
      (a) => a.questionId === answer.questionId
    );
    if (existingIndex > -1) {
      this.state.answers[existingIndex] = answer;
    } else {
      this.state.answers.push(answer);
    }
    this.updateProgress();
  }

  previousQuestion(): void {
    if (this.state.currentQuestionIndex > 0) {
      this.state.currentQuestionIndex--;
      this.updateProgress();
    }
  }

  nextQuestion(): void {
    if (
      this.state.currentQuestionIndex <
      this.state.template.questions.length - 1
    ) {
      this.state.currentQuestionIndex++;
      this.updateProgress();
    }
  }

  submitQuestionnaire(): void {
    if (this.allQuestionsAnswered) {
      this.state.isCompleted = true;
      console.log('Submitting questionnaire:', this.state.answers);
      alert('Questionnaire submitted successfully!');
    } else {
      alert('Please answer all questions before submitting.');
    }
  }

  private updateProgress(): void {
    const currentQuestionAnswered = this.isAnswered ? 1 : 0;
    const totalQuestions = this.state.template.questions.length;
    const progressForCurrent = (this.state.currentQuestionIndex / totalQuestions) * 100;
    const progressForAnswer = currentQuestionAnswered * (100 / totalQuestions);
    this.state.progress = Math.min(progressForCurrent + progressForAnswer, 100);
  }
}
