import { User } from "../../../shared/models/user.model";

  export interface ActiveQuestionnaireBase {
    id: string;
    title: string;
    description?: string;
    activatedAt: Date;
    studentCompletedAt: Date | null;
    teacherCompletedAt: Date | null;
    student?: User
    teacher?: User
  }
  
  export interface ActiveQuestionnaireResponse {
    activeQuestionnaireBases: ActiveQuestionnaireBase[];
    queryCursor: string | null;
    totalCount: number;
  }