import { inject, Injectable } from '@angular/core';
import { Questionnaire } from '../models/answer.model';
import { Observable, of } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class MockAnswerService {
  private authService = inject(AuthService);
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
    {
      id: 'questionnaire2',
      title: 'Software Development Survey',
      description: 'A brief survey on software development practices.',
      questions: [
        {
          id: 6,
          text: 'Which software development methodology do you follow?',
          options: [
            { id: 601, text: 'Agile' },
            { id: 602, text: 'Waterfall' },
            { id: 603, text: 'Scrum' },
          ],
          allowsCustomAnswer: true,
        },
        {
          id: 7,
          text: 'What is your preferred programming paradigm?',
          options: [
            { id: 701, text: 'Object-Oriented Programming' },
            { id: 702, text: 'Functional Programming' },
            { id: 703, text: 'Procedural Programming' }
          ],
          allowsCustomAnswer: true,
        },
        {
          id: 8,
          text: 'How often do you write unit tests?',
          options: [
            { id: 801, text: 'Always' },
            { id: 802, text: 'Often' },
            { id: 803, text: 'Sometimes' },
            { id: 804, text: 'Never' },
          ],
          allowsCustomAnswer: false,
        },
      ],
      createdAt: new Date(),
    }
  ];

  // Active questionnaire instances with allowed users
  private activeInstances: { id: string; templateId: string; allowedUsers: string[] }[] = [
    { id: 'active1', templateId: 'questionnaire1', allowedUsers: ['mockId12345', 'user2', 'user3'] },
    { id: 'active2', templateId: 'questionnaire2', allowedUsers: ['user4', 'user5', 'user6'] },
  ];

// Get active questionnaire by instance ID and check if user is allowed
getActiveQuestionnaireById(instanceId: string): Observable<Questionnaire | undefined> {
  const userId = this.authService.getUserId();
  if (!userId) {
    console.error('No user logged in.');
    return of(undefined);
  }

  const instance = this.activeInstances.find(inst => inst.id === instanceId);
  if (!instance) {
    console.error(`Instance ${instanceId} not found.`);
    return of(undefined);
  }

  if (!this.isUserAllowed(instance, userId)) {
    console.error(`User ${userId} is not allowed to access questionnaire ${instanceId}`);
    return of(undefined);
  }

  return this.getTemplateById(instance.templateId);
}

  // Helper function to check if a user is allowed
  private isUserAllowed(instance: { allowedUsers: string[] }, userId: string): boolean {
    return instance.allowedUsers.includes(userId);
  }

  // Get a specific template by ID
  private getTemplateById(templateId: string): Observable<Questionnaire | undefined> {
    const template = this.mockTemplates.find(template => template.id === templateId);
    return of(template);
  }
}