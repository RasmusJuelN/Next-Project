export interface Option {
  id: number; // ID of the option from the database
  questionId: number; // ID of the question this option belongs to
  value: number; // Rating value (1-5)
  label: string; // Label explaining the rating
}

export interface Question {
  id: number; // ID of the question from the database
  text: string; // Text of the question
  options: Option[]; // Multiple options for the question
  selectedOption?: number; // ID of the selected option, if any
}


export interface StudentAnswer {
  id: number; // ID of the student's answer from the database
  studentId: number; // ID of the student
  questionId: number; // ID of the question
  rating: number; // Rating value (1-5)
  answerId: number; // ID of an anwser
  answerDate: Date; // Date when the answer was provided
}


export interface TeacherAnswer {
  id: number; // ID of the teacher's answer from the database
  teacherId: number; // ID of the teacher
  questionId: number; // ID of the question
  rating: number; // Rating value (1-5)
  answerId: number; // ID of an anwser
  answerDate: Date; // Date when the answer was provided
}

export interface StudentTeacherAnwser{
  anwserID: number // The id of anwser collection
  student: StudentAnswer;
  teacher: TeacherAnswer;
}

export interface User {
  id: number;
  username: string;
  role: string; // Additional roles can be defined as needed (e.g., 'student', 'teacher', 'admin')
}