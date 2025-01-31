import { User } from "../../../shared/models/user.model";

export interface QuestionnaireSession {
    id: string;
    templateId: string;
    templateName: string;
    createdAt: Date;
    updatedAt: Date;
  
    student: {
      user: User;
      answered: boolean;
      answeredWhen: Date | null;
    };
  
    teacher: {
      user: User;
      answered: boolean;
      answeredWhen: Date | null;
    };
  }
  