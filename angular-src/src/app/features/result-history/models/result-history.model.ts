import { TemplateBase } from "../../../shared/models/template.model";
import { User } from "../../../shared/models/user.model";

    export interface TemplateBaseResponse {
      templateBases: TemplateBase[];
      queryCursor?: string; // Cursor for next items.
      totalCount: number;
    }

        export interface UserPaginationResult {
          userBases: User[];
          sessionId: string;
          hasMore: boolean;
        }