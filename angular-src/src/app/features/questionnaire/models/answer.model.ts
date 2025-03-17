export interface Option {
  id: number;
  text: string;
}
  
export interface Question {
  id: number;
  text: string;
  options: Option[];
  allowsCustomAnswer: boolean;
}

export interface Questionnaire {
  id: string;
  title: string;
  description: string;
  questions: Question[];
  createdAt?: Date;
}

export interface Answer {
  questionId: number;
  selectedOptionId?: number; // The ID of the selected option (if any)
  customAnswer?: string; // The custom answer provided by the user (if any)
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