import { Component, computed, DestroyRef, HostListener, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, ParamMap, Router } from '@angular/router';
import { QuestionComponent } from './question/question.component';
import { AnswerService } from './services/answer.service';
import { Answer, AnswerSubmission, QuestionnaireState } from './models/answer.model';
import { LoadingComponent } from '../../shared/loading/loading.component';
import { Role, User } from '../../shared/models/user.model';
import { AuthService } from '../../core/services/auth.service';
import { TranslateModule } from '@ngx-translate/core';
import { ModalComponent } from '../../shared/components/modal/modal.component';
import { map } from 'rxjs';


/**
 * Questionnaire component.
 *
 * Presents and submits an active questionnaire for the current user.
 *
 * Handles:
 * - Loading questionnaire by route id.
 * - Submitting answers when all questions are completed.
 */
@Component({
    selector: 'app-answer-questionnaire',
    imports: [CommonModule, QuestionComponent, TranslateModule, ModalComponent],
    templateUrl: './questionnaire.component.html',
    styleUrls: ['./questionnaire.component.css']
})
export class QuestionnaireComponent {
  private answerService = inject(AnswerService);
  private authService = inject(AuthService)
  private route = inject(ActivatedRoute);
  private router = inject(Router);
  private destroyRef = inject(DestroyRef);

  readonly user = this.authService.user;

  state: QuestionnaireState = {
    template: {
      id: '',
      title: '',
      description: '',
      questions: [],
      activatedAt: new Date(),
    },
    currentQuestionIndex: 0,
    answers: [],
    progress: 0,
    isCompleted: false,
  };

  isLoading = true;
  errorMessage: string | null = null;

  // Confirmation modal state
  showSubmitConfirmModal = false;

 @HostListener('window:keydown', ['$event'])
handleKeyboardEvent(event: KeyboardEvent): void {
  // Don't handle keyboard events if user is typing in a textarea
  const target = event.target as HTMLElement;
  if (target.tagName === 'TEXTAREA') {
    return;
  }

  switch (event.key) {
    case 'ArrowRight':
    case 'Enter':
      event.preventDefault();
      if (this.state.currentQuestionIndex === this.state.template.questions.length - 1) {
        // Last question - submit if all answered
        if (this.allQuestionsAnswered) {
          this.submitQuestionnaire();
        }
      } else if (this.isAnswered) {
        // Not last question - go to next if current is answered
        this.nextQuestion();
      }
      break;

    case 'ArrowLeft':
    case 'Backspace':
      event.preventDefault();
      this.previousQuestion();
      break;

    case 'ArrowUp':
      event.preventDefault();
      this.selectPreviousOption();
      break;

    case 'ArrowDown':
      event.preventDefault();
      this.selectNextOption();
      break;

    case 'Escape':
      event.preventDefault();
      if (confirm('Are you sure you want to leave? Your progress will be lost.')) {
        this.router.navigate(['/']);
      }
      break;
  }
}



  ngOnInit() {
    const u = this.user(); 
    if (!u) { this.router.navigate(['/'], { replaceUrl: true }); return; }

  this.route.paramMap
  .subscribe((pm) => {
    const questionnaireId = pm.get('id');
    if (questionnaireId) {
      this.checkAndLoadQuestionnaire(questionnaireId);
    } else {
      console.error('No questionnaire ID found in route!');
    }
  });
  }
  /**
 * Verifies whether the user already submitted the questionnaire;
 * loads details if not, otherwise navigates home.
 */
  private checkAndLoadQuestionnaire(id: string) {
    this.isLoading = true;
    // First, check if the user has already submitted the questionnaire
    this.answerService.hasUserSubmited(id).subscribe({
      next: (hasSubmitted: boolean) => {
        if (hasSubmitted) {
          // If already submitted, navigate back to the root
          this.router.navigate(['/']);
        } else {
          // If not submitted, load the questionnaire details
          this.loadQuestionnaire(id);
        }
      },
      error: (error) => {
        console.error('Error checking submission status:', error);
        this.errorMessage = 'An error occurred while checking the questionnaire status. Please try again later.';
        this.isLoading = false;
      }
    });
  }

  /** Loads questionnaire template data and updates progress. */
  private loadQuestionnaire(id: string) {
    this.answerService.getActiveQuestionnaireById(id)
    .subscribe({
      next: (template) => {
        if (template) {
          this.state.template = template;
          this.updateProgress();
        } else {
          this.errorMessage = 'Could not find any data with that link. Did you write it correctly?';
        }
        this.isLoading = false;
      },
      error: (error) => {
        console.error('Error loading questionnaire:', error);
        this.errorMessage = 'An error occurred while loading the questionnaire. Please try again later.';
        this.isLoading = false;
      },
    });
  }

  /** The question at the current index. */
  get currentQuestion() {
    return this.state.template.questions[this.state.currentQuestionIndex];
  }

  /** The saved answer for the current question (if any). */
  get currentAnswer(): Answer | undefined {
    return this.state.answers.find(
      (a) => a.questionId === this.currentQuestion.id
    );
  }

  /** True if every question has either an option selected or a non-empty custom answer. */
  get allQuestionsAnswered(): boolean {
    return this.state.template.questions.every((question) =>
      this.state.answers.some(
        (answer) =>
          answer.questionId === question.id &&
          (!!answer.optionId || !!answer.customAnswer?.trim())
      )
    );
  }

  get isAnswered(): boolean {
    const answer = this.currentAnswer;
    return !!answer && (!!answer.optionId || !!answer.customAnswer?.trim());
  }

  /** True if the current question has an answer (option or non-empty custom text). */
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

  /** Moves to the previous question and updates progress. */
  previousQuestion(): void {
    if (this.state.currentQuestionIndex > 0) {
      this.state.currentQuestionIndex--;
      this.updateProgress();
    }
  }

  /** Moves to the next question and updates progress. */
  nextQuestion(): void {
    if (
      this.state.currentQuestionIndex <
      this.state.template.questions.length - 1
    ) {
      this.state.currentQuestionIndex++;
      this.updateProgress();
    }
  }

  /**
 * Submits all answers when the questionnaire is complete.
 * On success, marks as completed and navigates home.
 */
  submitQuestionnaire(): void {
    if (!this.allQuestionsAnswered) {
      alert('Please answer all questions before submitting.');
      return;
    }
    this.showSubmitConfirmModal = true;
  }

  // ✅ Simple confirm handler
  onConfirmSubmit(): void {
    this.showSubmitConfirmModal = false;
    
    const submission: AnswerSubmission = { answers: this.state.answers };
    this.answerService.submitAnswers(this.state.template.id, submission).subscribe({
      next: () => {
        this.state.isCompleted = true;
        alert('Questionnaire submitted successfully!');
        this.router.navigate(['/']);
      },
      error: (error) => {
        console.error('Error submitting questionnaire:', error);
        alert('There was an error submitting your questionnaire. Please try again later.');
      }
    });
  }

  // ✅ Simple cancel handler
  onCancelSubmit(): void {
    this.showSubmitConfirmModal = false;
  }


  /** Recomputes progress percentage based on index and whether the current question is answered. */
  private updateProgress(): void {
    const currentQuestionAnswered = this.isAnswered ? 1 : 0;
    const totalQuestions = this.state.template.questions.length;
    const progressForCurrent = (this.state.currentQuestionIndex / totalQuestions) * 100;
    const progressForAnswer = currentQuestionAnswered * (100 / totalQuestions);
    this.state.progress = Math.min(progressForCurrent + progressForAnswer, 100);
  }

  /** Select previous option in current question (wraps around) */
  private selectPreviousOption(): void {
    const question = this.currentQuestion;
    if (!question || question.options.length === 0) return;

    const currentAnswer = this.currentAnswer;
    let currentIndex = -1;

    if (currentAnswer?.optionId) {
      currentIndex = question.options.findIndex(opt => opt.id === currentAnswer.optionId);
    }

    // Move to previous option (wrap around to last if at first)
    const newIndex = currentIndex <= 0 
      ? question.options.length - 1 
      : currentIndex - 1;

    const selectedOption = question.options[newIndex];
    
    // Reuse onAnswerChange
    this.onAnswerChange({
      questionId: question.id,
      optionId: selectedOption.id,
      customAnswer: undefined
    });
  }

  /** Select next option in current question (wraps around) */
  private selectNextOption(): void {
    const question = this.currentQuestion;
    if (!question || question.options.length === 0) return;

    const currentAnswer = this.currentAnswer;
    let currentIndex = -1;

    if (currentAnswer?.optionId) {
      currentIndex = question.options.findIndex(opt => opt.id === currentAnswer.optionId);
    }

    // Move to next option (wrap around to first if at last)
    const newIndex = currentIndex >= question.options.length - 1 
      ? 0 
      : currentIndex + 1;

    const selectedOption = question.options[newIndex];
    
    // Reuse onAnswerChange
    this.onAnswerChange({
      questionId: question.id,
      optionId: selectedOption.id,
      customAnswer: undefined
    });
  }
  /**
 * Returns collaborator display text based on the viewer's role:
 * - Student sees teacher, teacher sees student.
 */
getCollaboratorInfo(): string | null {
  const user = this.user();
  const role = user?.role;
  const q = this.state.template;
  const student = q?.student;
  const teacher = q?.teacher;

  if (!student || !teacher || !role) return null;

  switch (role) {
    case Role.Student:
      return `${teacher.fullName} (${teacher.userName})`;
    case Role.Teacher:
      return `${student.fullName} (${student.userName})`;
    case Role.Admin:
      return `Student: ${student.fullName} (${student.userName}), Teacher: ${teacher.fullName} (${teacher.userName})`;
    default:
      return null;
  }
}
}