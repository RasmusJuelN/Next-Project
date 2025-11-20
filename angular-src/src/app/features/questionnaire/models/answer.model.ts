import { User } from "../../../shared/models/user.model";

/**Represents a single answer option for a question.*/
export interface Option {
  /** Unique identifier of the option. */
  id: number;

  /** Display text for the option. */
  displayText: string;

  /** Sort order for this option within the question. */
  sortOrder: number;
}


/**Represents a single question in a questionnaire. */
export interface Question {
  /** Unique identifier of the question. */
  id: number;

  /** Question prompt text. */
  prompt: string;

  /** Available answer options. */
  options: Option[];

  /** Whether custom free-text answers are allowed. */
  allowCustom: boolean;

  /** Sort order for this question within the questionnaire. */
  sortOrder: number;
}

/** Represents a questionnaire template or active questionnaire. */
export interface Questionnaire {
  /** Unique identifier of the questionnaire. */
  id: string;

  /** Title of the questionnaire. */
  title: string;

  /** Description of the questionnaire. */
  description: string;

  /** List of questions included. */
  questions: Question[];

  /** When the questionnaire was activated/created. */
  activatedAt?: Date;

  /** Student assigned to this questionnaire (if any). */
  student?: User;

  /** Teacher assigned to this questionnaire (if any). */
  teacher?: User;
}

/**
 * Represents an answer to a single question.
 */
export interface Answer {
  /** The ID of the related question. */
  questionId: number;

  /** The selected option ID (if any). */
  optionId?: number;

  /** A custom text answer (chosen if not an option). */
  customAnswer?: string;
}

/** Payload for submitting a full set of questionnaire answers. */
export interface AnswerSubmission {
  /** List of answers to submit. */
  answers: Answer[];
}


/** Holds state for questionaire and answers */
export interface QuestionnaireState {
  /** The questionnaire being answered. */
  template: Questionnaire;

  /** Current index of the active question. */
  currentQuestionIndex: number;

  /** Collected answers so far. */
  answers: Answer[];

  /** Percentage of completion progress (0–100). */
  progress: number;

  /** True once the questionnaire is fully submitted. */
  isCompleted: boolean;
}