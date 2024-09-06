import { Injectable } from '@angular/core';
import { MockDataService } from './mock-data.service';
import { Observable, of } from 'rxjs';
import { ActiveQuestionnaire, Question, QuestionTemplate, User } from '../../models/questionare';
import { DataService } from './data.service';

@Injectable({
  providedIn: 'root'
})
export class AppDataService {

  constructor(
    private dataService: MockDataService
  ) {}

  createActiveQuestionnaire(student: User, teacher: User, templateId: string): Observable<ActiveQuestionnaire>{
    return this.dataService.createActiveQuestionnaire(student,teacher,templateId)
  }

  getUsersFromSearch(role: string, nameString: string, page: number = 1, limit: number = 10){
    return this.dataService.getUsersFromSearch(role,nameString,page,limit);
  }
  getTemplatesFromSearch(titleString: string, page: number = 1, limit: number = 10){
    return this.dataService.getTemplatesFromSearch(titleString,page,limit);
  }

  // Template Management Methods (Newly Added)
  // Get all templates
  getTemplates(): Observable<QuestionTemplate[]> {
    return this.dataService.getTemplates();
  }

  // Create a new template
  createTemplate(template: QuestionTemplate): Observable<void> {
    return this.dataService.createTemplate(template)
  }

  // Update an existing template
  updateTemplate(template: QuestionTemplate)  {
    return this.dataService.updateTemplate(template)
  }

    // Delete a template by ID
    deleteTemplate(templateId: string): Observable<void> {
      return this.dataService.deleteTemplate(templateId)
    }
  

  // Dashboard
  getActiveQuestionnairePage(filter: any, page: number, limit: number): Observable<ActiveQuestionnaire[]> {
    return this.dataService.getActiveQuestionnairePage(filter, page, limit);
  }

  // Questionnaire Methods
  getActiveQuestionnaireById(id: string): Observable<ActiveQuestionnaire | null> {
    return this.dataService.getActiveQuestionnaireById(id);
  }

  getQuestionsForUser(templateId: string): Observable<Question[]> {
    return this.dataService.getQuestionsForUser(templateId);
  }

  submitUserAnswers(userId: number | null, role: string, answers: Question[], questionnaireId: string | null): Observable<void> {
    return this.dataService.submitData(userId, role, answers, questionnaireId!);
  }

  validateUserAccess(userId: any, role: string, questionnaireId: string): Observable<boolean> {
    return this.dataService.validateUserAccess(userId, role, questionnaireId);
  }

}
