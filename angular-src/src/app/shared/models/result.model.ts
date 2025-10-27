import { User } from "./user.model";



export interface Result {
  id: string;
  title: string;
  description?: string | null;
  student: {
    user: User; // Ensure User type reflects properties like guid, primaryRole, etc.
    completedAt: Date;
  };
  teacher: {
    user: User; // Ensure User type reflects properties like guid, primaryRole, etc.
    completedAt: Date;
  };
  answers: Answer[];
}

export interface Answer {
  question: string;
  studentResponse: string;
  isStudentResponseCustom: boolean;
  teacherResponse: string;
  isTeacherResponseCustom: boolean;
  options?: QuestionOption[];
}

export interface QuestionOption {
  displayText: string;
  optionValue: string;
  isSelectedByStudent: boolean;
  isSelectedByTeacher: boolean;
}