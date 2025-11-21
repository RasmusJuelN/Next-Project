import { Component, computed, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { GuideSection } from './models/user-guide.model';
import { AuthService } from '../../core/services/auth.service';
import { Role } from '../../shared/models/user.model';

/**
 * User Guide component that provides role-specific help documentation for users
 */
@Component({
  selector: 'app-user-guide',
  standalone: true,
  imports: [CommonModule, TranslateModule],
  templateUrl: './user-guide.component.html',
  styleUrls: ['./user-guide.component.css']
})
export class UserGuideComponent {
  private authService = inject(AuthService);
  
  readonly userRole = computed(() => this.authService.user()?.role ?? null);

  /** Role-specific guide sections */
  private roleSections: Record<Role, GuideSection[]> = {
    [Role.Student]: [
      {
        titleKey: 'USER_GUIDE.STUDENT.ANSWERING_QUESTIONNAIRES.TITLE',
        contentKey: 'USER_GUIDE.STUDENT.ANSWERING_QUESTIONNAIRES.CONTENT'
      },
      {
        titleKey: 'USER_GUIDE.STUDENT.VIEWING_RESULTS.TITLE',
        contentKey: 'USER_GUIDE.STUDENT.VIEWING_RESULTS.CONTENT'
      }
    ],
    [Role.Teacher]: [
      {
        titleKey: 'USER_GUIDE.TEACHER.DASHBOARD.TITLE',
        contentKey: 'USER_GUIDE.TEACHER.DASHBOARD.CONTENT'
      },
      {
        titleKey: 'USER_GUIDE.TEACHER.DATA_COMPARE.TITLE',
        contentKey: 'USER_GUIDE.TEACHER.DATA_COMPARE.CONTENT'
      },
      {
        titleKey: 'USER_GUIDE.TEACHER.RESULT_HISTORY.TITLE',
        contentKey: 'USER_GUIDE.TEACHER.RESULT_HISTORY.CONTENT'
      }
    ],
    [Role.Admin]: [
      {
        titleKey: 'USER_GUIDE.ADMIN.TEMPLATE_MANAGEMENT.TITLE',
        contentKey: 'USER_GUIDE.ADMIN.TEMPLATE_MANAGEMENT.CONTENT'
      },
      {
        titleKey: 'USER_GUIDE.ADMIN.ACTIVE_QUESTIONNAIRES.TITLE',
        contentKey: 'USER_GUIDE.ADMIN.ACTIVE_QUESTIONNAIRES.CONTENT'
      }
    ]
  };

  /** General sections visible to all users */
  private generalSections: GuideSection[] = [
    {
      titleKey: 'USER_GUIDE.GENERAL.NAVIGATION.TITLE',
      contentKey: 'USER_GUIDE.GENERAL.NAVIGATION.CONTENT'
    }
  ];

  /**
   * Computed property that returns the appropriate guide sections based on user role
   */
  readonly guideSections = computed(() => {
    const role = this.userRole();
    const roleSections = role ? this.roleSections[role] : [];
    return [...roleSections, ...this.generalSections];
  });
}