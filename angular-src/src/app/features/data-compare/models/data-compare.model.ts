import { User } from "../../../shared/models/user.model";



export interface DataCompare {
  id: string;
  title: string;
  description?: string | null;
  student: {
    user: User; // Ensure User type reflects properties like guid, primaryRole, etc.
    completedAt: Date;
  };
  answers: Answer[];
}

export interface Answer {
  question: string;
  studentResponse: string;
  isStudentResponseCustom: boolean;
}