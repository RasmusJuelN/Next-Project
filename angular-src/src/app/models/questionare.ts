

/**
 * Represents a user like (e.g., 'student', 'teacher', 'admin').
 */
export interface User {
  id: number; // ID of the user from the database
  userName: string; // Username of the user
  fullName: string
  role: string; // Role for the user.
}

export interface QuestionnaireMetadata {
  questionnaireId: string;
  totalQuestions: number;
  questionIds: string[];
  currentIndex: number;
  progress?: number;
}

/**
 * Represents an option for a question in a questionnaire.
 */
export interface Option {
  id: number; // ID of the option from the database
  value: number; // Rating value (1-5)
  label: string; // Label explaining the rating
  isCustom?: boolean; // Indicates if this option is a custom answer
}

/**
 * Represents a question in a questionnaire.
 */
export interface Question {
  id: number; // ID of the question from the database
  title: string; // Text of the question
  options: Option[]; // Multiple options for the question
  selectedOption?: number; // ID of the selected option, if any
  customAnswer?: string; // The user's custom answer, if any
}

export interface QuestionTemplate {
  templateId: string;
  title: string;
  description: string;
  questions: Question[];
  createdAt?: Date;
}

export interface ActiveQuestionnaire {
  id: string; // ID of the active questionnaire instance
  student: User; // The student involved in the questionnaire
  teacher: User; // The teacher involved in the questionnaire
  isStudentFinished: boolean; // Whether the student has finished the questionnaire
  isTeacherFinished: boolean; // Whether the teacher has finished the questionnaire
  questionnaireTemplate: {
    templateId: string; // ID of the template used
    title: string; // Title of the template
    description: string; // Description of the template
  };
  createdAt?: Date;
}


export interface Answer {
  questionId: number;
  selectedOptionId?: number;
  customAnswer?: string;
}

export interface UserAnswerSheet {
  user: User; // Reference to the User object instead of just ID
  answers: Answer[];
  answeredAt: Date;
}

export interface AnswerSession {
  questionnaireId: string;
  studentAnswers: UserAnswerSheet;
  teacherAnswers: UserAnswerSheet;
}


// Used to display details of the question.
export interface QuestionDetails {
  questionId: number;
  questionTitle: string;
  studentAnswer: string;
  teacherAnswer: string;
}



export interface UserSearchResponse {
  users: User[];
  cacheCookie: string;
}
