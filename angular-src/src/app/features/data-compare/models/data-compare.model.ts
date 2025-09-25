import { User } from "../../../shared/models/user.model";



export interface DataCompare {
  id: string;
  title: string;
  description?: string | null;
  studentCompletedAt: Date;
  answers: Answer[];
}

export interface Answer {
  question: string;
  studentResponse: string;
  isStudentResponseCustom: boolean;
}