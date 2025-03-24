import { User } from "../../../shared/models/user.model";

export interface UserSpecificActiveQuestionnaireBase {
  id: string;
  title: string;
  description?: string;
  activatedAt: Date;
  completedAt: Date | null;
}