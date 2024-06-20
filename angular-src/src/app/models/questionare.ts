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


/**
 * Represents a student's answer to a question.
 */
export interface StudentAnswer {
  id: number; // ID of the student's answer from the database
  studentId: number; // ID of the student
  questionId: number; // ID of the question
  rating: number; // Rating value (1-5)
  answerId: number; // ID of an anwser
  answerDate: Date; // Date when the answer was provided
}


/**
 * Represents a teacher's answer to a question.
 */
export interface TeacherAnswer {
  id?: number; // ID of the teacher's answer from the database
  teacherId: number; // ID of the teacher
  questionId?: number; // ID of the question
  rating: number; // Rating value (1-5)
  answerId?: number; // ID of an anwser
  answerDate?: Date; // Date when the answer was provided
}

/**
 * Represents a a combined student and teacher answer to a question.
 */
export interface StudentTeacherAnwser{
  anwserID: number // The id of anwser collection
  student: StudentAnswer; // The student anwser
  teacher: TeacherAnswer; // The teacher anwser
}


/**
 * Represents a user like (e.g., 'student', 'teacher', 'admin').
 */
export interface User {
  id: number; // ID of the user from the database
  username: string; // Username of the user
  role: string; // Role for the user.
}