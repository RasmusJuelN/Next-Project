import { inject, Injectable } from '@angular/core';
import { Observable, of, switchMap, take } from 'rxjs';
import { ActiveQuestionnaireBase, ActiveQuestionnaireResponse, UserSpecificActiveQuestionnaireBase } from '../models/show-active.model';
import { ApiService } from '../../../core/services/api.service';
import { HttpParams } from '@angular/common/http';
import { AuthService } from '../../../core/services/auth.service';
import { Role } from '../../../shared/models/user.model';

@Injectable({
  providedIn: 'root'
})
export class ShowActiveService {
  apiService = inject(ApiService)
  authService = inject(AuthService)
  constructor() { }

  fetchActiveQuestionnaires(): Observable<ActiveQuestionnaireBase[]> {
    const params = new HttpParams().set('pageSize', '100'); // Or whatever limit you want
  
    return this.authService.userRole$.pipe(
      take(1),
      switchMap(role => {
        const endpoint = role === Role.Student
          ? 'api/user/student/activequestionnaires/pending'
          : 'api/user/teacher/activequestionnaires/pending';
        return this.apiService.get<ActiveQuestionnaireBase[]>(endpoint, params);
      })
    );
  }
  
}