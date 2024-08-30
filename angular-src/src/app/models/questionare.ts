

/**
 * Represents a user like (e.g., 'student', 'teacher', 'admin').
 */
export interface User {
  id: number; // ID of the user from the database
  userName: string; // Username of the user
  fullName: string
  role: string; // Role for the user.
}

/**
 * Represents Active Questionnaire
 */
export interface ActiveQuestionnaire {
  id: string;
  student: User;
  teacher: User;
  isStudentFinished: boolean;
  isTeacherFinished: boolean;
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

export interface userAnswer{
  questionnaireId: string;
  questionId: number;
  userId: number;
  role: string;
  rating: number;
}

/**
 * Represents a combined student and teacher answer to a question.
 */
export interface StudentTeacherAnswer{
  questionnaireId: string // The id of anwser collection
  student: userAnswer; // The student anwser
  teacher: userAnswer; // The teacher anwser
}