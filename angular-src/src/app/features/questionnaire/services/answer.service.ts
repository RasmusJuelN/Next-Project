import { Injectable } from '@angular/core';
import { Questionnaire } from '../models/answer.model';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../../environments/environment';

@Injectable({
  providedIn: 'root',
})
export class AnswerService {
    private apiUrl = environment.apiUrl; // Replace with your actual API URL

    constructor(private http: HttpClient) {}
  
    getTemplates(): Observable<Questionnaire[]> {
      return this.http.get<Questionnaire[]>(this.apiUrl);
    }

    getTemplateById(templateId: string): Observable<Questionnaire> {
      return this.http.get<Questionnaire>(`${this.apiUrl}/${templateId}`);
    }
}