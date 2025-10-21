import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Observable, forkJoin, map } from 'rxjs';
import { Result } from '../models/result.model';
import { Template } from '../../../shared/models/template.model';

@Injectable({
  providedIn: 'root'
})
export class ResultService {
  private apiUrl = `${environment.apiUrl}/active-questionnaire`;

  private apiService = inject(ApiService);
  
  canGetResult(id:string){
    return this.apiService.get<boolean>(`${this.apiUrl}/${id}/IsCompleted`);
  }

  getTemplateByResultId(id: string): Observable<Template> {
    // Get the active questionnaire first, then extract template information
    return this.apiService.get<any>(`${this.apiUrl}/${id}`).pipe(
      map((activeQuestionnaire: any) => {
        // Convert the active questionnaire template data to our Template interface
        return {
          id: activeQuestionnaire.template?.id || id,
          title: activeQuestionnaire.template?.title || activeQuestionnaire.title,
          description: activeQuestionnaire.template?.description || activeQuestionnaire.description,
          templateStatus: 'Finalized' as any,
          questions: activeQuestionnaire.template?.questions || activeQuestionnaire.questions || []
        } as Template;
      })
    );
  }

  getResultById(id: string): Observable<Result> {
    return this.apiService.get<Result>(`${this.apiUrl}/${id}/getresponse`);
  }
}
