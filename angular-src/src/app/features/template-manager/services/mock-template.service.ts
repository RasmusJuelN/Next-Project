import { Injectable } from '@angular/core';
import { NextCursor, Question, Template, TemplateBase, TemplateBaseResponse } from '../models/template.model';
import { delay, Observable, of } from 'rxjs';
import { PaginationResponse } from '../../../shared/models/Pagination.model';

@Injectable({
  providedIn: 'root'
})
export class MockTemplateService {
  private templates: Template[] = [
    {
      id: '1',
      title: 'Employee Onboarding Template',
      description: 'A template for onboarding new employees.',
      questions: [
        {
          id: 1,
          title: 'What is your full name?',
          customAnswer: true,
          options: [],
        },
        {
          id: 2,
          title: 'Select your department:',
          customAnswer: false,
          options: [
            { id: 1, label: 'HR' },
            { id: 2, label: 'Engineering' },
            { id: 3, label: 'Marketing' },
          ],
        },
      ],
    },
    {
      id: '2',
      title: 'Customer Feedback Template',
      description: 'A template for collecting customer feedback.',
      questions: [
        {
          id: 3,
          title: 'How satisfied are you with our service?',
          customAnswer: false,
          options: [
            { id: 1, label: 'Very Satisfied' },
            { id: 2, label: 'Satisfied' },
            { id: 3, label: 'Neutral' },
            { id: 4, label: 'Dissatisfied' },
            { id: 5, label: 'Very Dissatisfied' },
          ],
        },
        {
          id: 4,
          title: 'Any additional comments?',
          customAnswer: true,
          options: [],
        },
      ],
    },
    {
      id: '3',
      title: 'Project Evaluation Template',
      description: 'A template for evaluating project outcomes.',
      questions: [
        {
          id: 5,
          title: 'What is the name of the project?',
          customAnswer: true,
          options: [],
        },
        {
          id: 6,
          title: 'Rate the project success:',
          customAnswer: false,
          options: [
            { id: 1, label: '1 - Poor' },
            { id: 2, label: '2 - Below Average' },
            { id: 3, label: '3 - Average' },
            { id: 4, label: '4 - Good' },
            { id: 5, label: '5 - Excellent' },
          ],
        },
      ],
    },
    {
      id: '4',
      title: 'Training Feedback Template',
      description: 'A template for collecting feedback on training sessions.',
      questions: [
        {
          id: 7,
          title: 'How useful was the training?',
          customAnswer: false,
          options: [
            { id: 1, label: 'Very Useful' },
            { id: 2, label: 'Somewhat Useful' },
            { id: 3, label: 'Not Useful' },
          ],
        },
      ],
    },
    {
      id: '5',
      title: 'Event Registration Template',
      description: 'A template for registering attendees for an event.',
      questions: [
        {
          id: 8,
          title: 'What is your name?',
          customAnswer: true,
          options: [],
        },
        {
          id: 9,
          title: 'What is your contact email?',
          customAnswer: true,
          options: [],
        },
      ],
    },
    {
      id: '6',
      title: 'Survey Template',
      description: 'A simple survey template for various uses.',
      questions: [
        {
          id: 10,
          title: 'What is your age group?',
          customAnswer: false,
          options: [
            { id: 1, label: '18-24' },
            { id: 2, label: '25-34' },
            { id: 3, label: '35-44' },
            { id: 4, label: '45-54' },
            { id: 5, label: '55+' },
          ],
        },
      ],
    },
    {
      id: '7',
      title: 'Bug Report Template',
      description: 'A template for reporting bugs in a software system.',
      questions: [
        {
          id: 11,
          title: 'Describe the bug:',
          customAnswer: true,
          options: [],
        },
        {
          id: 12,
          title: 'What is the severity of the bug?',
          customAnswer: false,
          options: [
            { id: 1, label: 'Low' },
            { id: 2, label: 'Medium' },
            { id: 3, label: 'High' },
            { id: 4, label: 'Critical' },
          ],
        },
      ],
    },
    {
      id: '8',
      title: 'Team Meeting Notes Template',
      description: 'A template for recording notes during team meetings.',
      questions: [
        {
          id: 13,
          title: 'What is the meeting date?',
          customAnswer: true,
          options: [],
        },
        {
          id: 14,
          title: 'Summary of key points:',
          customAnswer: true,
          options: [],
        },
      ],
    },
    {
      id: '8',
      title: 'Team Meeting Notes Template',
      description: 'A template for recording notes during team meetings.',
      questions: [
        {
          id: 13,
          title: 'What is the meeting date?',
          customAnswer: true,
          options: [],
        },
        {
          id: 14,
          title: 'Summary of key points:',
          customAnswer: true,
          options: [],
        },
      ],
    },
    {
      id: '8',
      title: 'WHAT',
      description: 'A template for recording notes during team meetings.',
      questions: [
        {
          id: 13,
          title: 'What is the meeting date?',
          customAnswer: true,
          options: [],
        },
        {
          id: 14,
          title: 'Summary of key points:',
          customAnswer: true,
          options: [],
        },
      ],
    },
    {
      id: '8',
      title: 'WHAT',
      description: 'A template for recording notes during team meetings.',
      questions: [
        {
          id: 13,
          title: 'What is the meeting date?',
          customAnswer: true,
          options: [],
        },
        {
          id: 14,
          title: 'Summary of key points:',
          customAnswer: true,
          options: [],
        },
      ],
    },
  ];
  

  constructor() {}
  
  getTemplateBases(
    pageSize: number = 5,
    nextCursorCreatedAt?: string,
    nextCursorId?: string,
    searchTerm: string = '',
    searchType: 'name' | 'id' = 'name'
  ): Observable<TemplateBaseResponse> {
    // 1. Filter based on searchTerm
    let filtered = [...this.templates];
    if (searchTerm.trim() !== '') {
      if (searchType === 'name') {
        filtered = filtered.filter((t) =>
          t.title.toLowerCase().includes(searchTerm.toLowerCase())
        );
      } else {
        filtered = filtered.filter((t) =>
          t.id.toLowerCase().includes(searchTerm.toLowerCase())
        );
      }
    }
  
    // 2. Determine the effective page from the provided cursor.
    //    If no nextCursorId is provided, we are on page 1.
    let effectivePage = 1;
    if (nextCursorId) {
      // Assume cursor format: 'dummy-cursor-for-page-X'
      const match = nextCursorId.match(/dummy-cursor-for-page-(\d+)/);
      if (match && match[1]) {
        effectivePage = parseInt(match[1]) - 1; // The current page is one less than the next page.
      }
    }
  
    // 3. Use effectivePage to calculate start and end indexes.
    const startIndex = (effectivePage - 1) * pageSize;
    const endIndex = startIndex + pageSize;
    const pageItems = filtered.slice(startIndex, endIndex);
  
    // 4. Map full Template items to TemplateBase objects.
    const templateBases: TemplateBase[] = pageItems.map((t) => ({
      id: t.id,
      templateTitle: t.title,
      createdAt: '2025-02-17T09:11:51.7366667', // or new Date().toISOString()
      lastUpdated: '2025-02-17T09:11:51.7366667',
      isLocked: false,
    }));
  
    // 5. Determine if there is more data beyond the current page.
    const hasMore = endIndex < filtered.length;
  
    // 6. If more data exists, create a dummy nextCursor.
    const newNextCursor: NextCursor | null = hasMore
      ? {
          createdAt: '2025-02-17T09:12:00.0000000',
          // The next cursor indicates the next page number.
          id: `dummy-cursor-for-page-${effectivePage + 2}`,
        }
      : null;
  
    // Build the response object.
    const response: TemplateBaseResponse = {
      templateBases,
      nextCursor: newNextCursor,
    };
  
    // Simulate network delay.
    return of(response).pipe(delay(1000));
  }
  
  

  getTemplateDetails(templateId: string): Observable<Template> {
    const found = this.templates.find((t) => t.id === templateId);
    if (!found) {
      throw new Error(`Template with ID ${templateId} not found`);
    }
    return of(found).pipe(delay(500));
  }


  addTemplate(template: Template): Observable<Template> {
    const newTemplate = { ...template, id: Date.now().toString() };
    this.templates.push(newTemplate);
    return of(newTemplate).pipe(delay(500));
  }

  updateTemplate(templateId: string, updatedTemplate: Template): Observable<void> {
    const index = this.templates.findIndex((t) => t.id === templateId);
    if (index === -1) {
      throw new Error(`Template with ID ${templateId} not found`);
    }
    this.templates[index] = { ...updatedTemplate };
    return of(void 0).pipe(delay(500));
  }

  /**
   * Delete a template by ID
   */
  deleteTemplate(templateId: string): Observable<void> {
    this.templates = this.templates.filter((t) => t.id !== templateId);
    return of(void 0).pipe(delay(500));
  }
}