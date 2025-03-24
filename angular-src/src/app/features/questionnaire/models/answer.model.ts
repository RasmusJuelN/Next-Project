export interface Option {
  id: number;
  displayText: string;
}

export interface Question {
  id: number;
  prompt: string;         // Updated from "text" to "prompt"
  options: Option[];
  allowCustom: boolean;   // Updated from "allowsCustomAnswer" to "allowCustom"
}

export interface Questionnaire {
  id: string;
  title: string;
  description: string;
  questions: Question[];
  activatedAt?: Date; // Updated from "createdAt" to "activatedAt" to match the API response
}

export interface Answer {
  questionId: number;
  optionId?: number; // A selected ID
  customAnswer?: string;
}

export interface AnswerSubmission{
  answers: Answer[]
}

export interface QuestionnaireState {
  template: Questionnaire;
  currentQuestionIndex: number;
  answers: Answer[];
  progress: number;
  isCompleted: boolean;
}