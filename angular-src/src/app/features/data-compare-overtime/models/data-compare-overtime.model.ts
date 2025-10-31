import { User } from "../../../shared/models/user.model";



export interface DataCompareOverTime {
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