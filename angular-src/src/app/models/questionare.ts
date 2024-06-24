/**
 * Represents Active Questionnaire
 */
export interface ActiveQuestionnaire {
  id?: string;
  studentId: number;
  teacherId: number;
  isStudentFinished: boolean;
  isTeacherFinished: boolean;
}

/**
 * Represents a user like (e.g., 'student', 'teacher', 'admin').
 */
export interface User {
  id: number; // ID of the user from the database
  username: string; // Username of the user
  role: string; // Role for the user.
}

/**
 * Represents an option for a question in a questionnaire.
 */
export interface Option {
  id?: number; // ID of the option from the database
  questionId?: number; // ID of the question this option belongs to
  value: number; // Rating value (1-5)
  label: string; // Label explaining the rating
}

/**
 * Represents a question in a questionnaire.
 */
export interface Question {
  id: number; // ID of the question from the database
  text: string; // Text of the question
  options: Option[]; // Multiple options for the question
  selectedOption?: number; // ID of the selected option, if any
}

export interface userAnswer{
  questinareId: string;
  questionId: number;
  userId: number;
  role: string;
  rating: number;
}

/**
 * Represents a combined student and teacher answer to a question.
 */
export interface StudentTeacherAnswer{
  Questionnaire: string // The id of anwser collection
  student: userAnswer; // The student anwser
  teacher: userAnswer; // The teacher anwser
}