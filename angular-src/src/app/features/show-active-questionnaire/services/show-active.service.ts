import { inject, Injectable } from '@angular/core';
import { Observable, of, switchMap, take } from 'rxjs';
import { ActiveQuestionnaireResponse, UserSpecificActiveQuestionnaireBase } from '../models/show-active.model';
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

  fetchActiveQuestionnaires(queryCursor?: string): Observable<ActiveQuestionnaireResponse> {
    let params = new HttpParams().set('pageSize', '5');
    if (queryCursor) {
      params = params.set('queryCursor', queryCursor);
    }
  
    return this.authService.userRole$.pipe(
      take(1),
      switchMap(role => {
        const endpoint = role === Role.Student
          ? 'api/user/student/activequestionnaires'
          : 'api/user/teacher/activequestionnaires';
        return this.apiService.get<ActiveQuestionnaireResponse>(endpoint, params);
      })
    );
  }
}