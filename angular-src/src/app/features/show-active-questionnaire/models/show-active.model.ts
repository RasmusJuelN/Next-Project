import { User } from "../../../shared/models/user.model";

export interface ActiveQuestionnaireBase {
    id: string;
    title?: string
    description?: string
    activatedAt: Date
    student: User
    teacher: User
    studentCompletedAt: Date | null;
    teacherCompletedAt: Date | null;
  }