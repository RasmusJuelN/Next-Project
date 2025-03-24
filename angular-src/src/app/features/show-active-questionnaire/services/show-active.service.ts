import { inject, Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';
import { UserSpecificActiveQuestionnaireBase } from '../models/show-active.model';
import { ApiService } from '../../../core/services/api.service';

@Injectable({
  providedIn: 'root'
})
export class ShowActiveService {
  apiService = inject(ApiService)
  constructor() { }

  fetchActiveQuestionnaires(): Observable<UserSpecificActiveQuestionnaireBase[]> {
    return this.apiService.get<UserSpecificActiveQuestionnaireBase[]>('/api/user/activequestionnaires');
  }
}