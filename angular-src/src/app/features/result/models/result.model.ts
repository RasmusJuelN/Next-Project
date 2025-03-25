import { User } from "../../../shared/models/user.model";


export interface ResultOLD {
  id: string;
  templateName: string;
  student: {
    user: User;
    answeredWhen: Date;
  };
  teacher: {
    user: User;
    answeredWhen: Date;
  };
  answers: {
    question: string;
    studentAnswer: string;
    isStudentCustomAnswer: boolean;
    teacherAnswer: string;
    isTeacherCustomAnswer: boolean;
  }[];
}
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
}