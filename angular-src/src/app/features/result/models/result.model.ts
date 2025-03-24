import { User } from "../../../shared/models/user.model";


export interface Result {
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
export interface ResultTEST {
  id: string;
  title: string;
  description?: string | null;
  student:User[]
  teacher:User[]
  answers:answer[]
}

interface answer{
  question: string;
  studentAnswer: string;
  isStudentCustomAnswer: boolean;
  teacherAnswer: string;
  isTeacherCustomAnswer: boolean;
}