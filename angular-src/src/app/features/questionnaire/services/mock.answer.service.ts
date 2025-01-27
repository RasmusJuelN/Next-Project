import { Injectable } from '@angular/core';
import { Questionnaire } from '../models/answer.model';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root',
})
export class MockAnswerService {
  // Mock question templates
  private mockTemplates: Questionnaire[] = [
    {
      id: 'questionnaire1',
      title: 'Programming Questionnaire',
      description: 'A short questionnaire about programming preferences.',
      questions: [
        {
          id: 1,
          text: 'What is your favorite programming language?',
          options: [
            { id: 101, text: 'JavaScript' },
            { id: 102, text: 'Python' },
            { id: 103, text: 'C#' },
            { id: 104, text: 'Java' },
          ],
          allowsCustomAnswer: true,
        },
        {
          id: 2,
          text: 'What is your preferred development environment?',
          options: [
            { id: 201, text: 'VS Code' },
            { id: 202, text: 'IntelliJ' },
            { id: 203, text: 'Eclipse' },
            { id: 204, text: 'Other' },
          ],
          allowsCustomAnswer: true,
        },
        {
          id: 3,
          text: 'What type of projects do you usually work on?',
          options: [
            { id: 301, text: 'Web Applications' },
            { id: 302, text: 'Mobile Applications' },
            { id: 303, text: 'Desktop Applications' },
            { id: 304, text: 'Embedded Systems' },
          ],
          allowsCustomAnswer: false,
        },
        {
          id: 4,
          text: 'Which version control system do you prefer?',
          options: [
            { id: 401, text: 'Git' },
            { id: 402, text: 'SVN' },
            { id: 403, text: 'Mercurial' },
            { id: 404, text: 'Other' },
          ],
          allowsCustomAnswer: true,
        },
        {
          id: 5,
          text: 'How many years of programming experience do you have?',
          options: [
            { id: 501, text: 'Less than 1 year' },
            { id: 502, text: '1-3 years' },
            { id: 503, text: '3-5 years' },
            { id: 504, text: 'More than 5 years' },
          ],
          allowsCustomAnswer: false,
        },
      ],
      createdAt: new Date(),
    },
    // Add more templates as needed
  ];

  // Get all mock templates
  getTemplates(): Observable<Questionnaire[]> {
    return of(this.mockTemplates);
  }

  // Get a specific template by ID as an Observable
  getTemplateById(templateId: string): Observable<Questionnaire | undefined> {
    const template = this.mockTemplates.find(template => template.id === templateId);
    return of(template);
  }
}
