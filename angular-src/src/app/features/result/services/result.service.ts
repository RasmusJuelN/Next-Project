import { inject, Injectable } from '@angular/core';
import { ApiService } from '../../../core/services/api.service';
import { environment } from '../../../../environments/environment';
import { Observable, forkJoin, map } from 'rxjs';
import { Result } from '../../../shared/models/result.model';
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

  private getTemplateByResultId(id: string): Observable<Template> {
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

    
    /**
     * 
    // Fetch both result and template, then merge the template options into the result
    return forkJoin({
      result: this.apiService.get<Result>(`${this.apiUrl}/${id}/getresponse`),
      template: this.getTemplateByResultId(id)
    }).pipe(
      map(({ result, template }) => {
        // Populate QuestionOption[] for each answer from template
        if (result && template) {
          result.answers = result.answers.map(answer => {
            const templateQuestion = template.questions.find(q => q.prompt === answer.question);
            if (templateQuestion && templateQuestion.options) {
              // Convert template options to QuestionOption[] and determine selection status
              answer.options = templateQuestion.options.map((option, index) => ({
                displayText: option.displayText,
                optionValue: option.optionValue?.toString() || '',
                isSelectedByStudent: this.isOptionSelectedByResponse(answer.studentResponse, answer.isStudentResponseCustom, option, index),
                isSelectedByTeacher: this.isOptionSelectedByResponse(answer.teacherResponse, answer.isTeacherResponseCustom, option, index)
              }));
            }
            return answer;
          });
        }
        return result;
      })
    );
     */
  }

  getCompletedStudentsByGroup(activeQuestionnaireId: string) {
    return this.apiService.get<Array<{ id: string; student: { fullName: string; userName?: string } }>>(
      `${this.apiUrl}/${activeQuestionnaireId}/completedStudentsByGroup`
    );
  }

  private isOptionSelectedByResponse(response: string, isCustom: boolean, option: any, index: number): boolean {
    if (isCustom || !response) return false;
    
    // Try to match by option value, display text
    if (response === option.displayText) return true;
    if (response === option.optionValue?.toString()) return true;
    if (response === (index + 1).toString()) return true; // Match 1-based index
    if (response === index.toString()) return true; // Match 0-based index
    
    // Try exact text match (case insensitive)
    if (response?.toLowerCase() === option.displayText?.toLowerCase()) return true;
    
    return false;
  }
}
