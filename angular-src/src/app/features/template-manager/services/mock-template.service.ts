import { Injectable } from '@angular/core';
import { Question, Template, TemplateBase, TemplateBaseResponse } from '../models/template.model';
import { delay, Observable, of } from 'rxjs';
import { PaginationResponse } from '../../../shared/models/Pagination.model';

@Injectable({
  providedIn: 'root'
})
export class MockTemplateService {
  private templates: Template[] = [
    {
      id: '1',
      templateTitle: 'Employee Onboarding Template',
      description: 'A template for onboarding new employees.',
      questions: [
        {
          id: 101,
          prompt: 'What is your full name?',
          allowCustom: true,
          options: [],
        },
        {
          id: 102,
          prompt: 'Select your department:',
          allowCustom: false,
          options: [
            { id: 1, optionValue: 1, displayText: 'HR' },
            { id: 2, optionValue: 2, displayText: 'Engineering' },
            { id: 3, optionValue: 3, displayText: 'Marketing' },
          ],
        },
      ],
    },
    {
      id: '2',
      templateTitle: 'Customer Feedback Template',
      description: 'A template for collecting customer feedback.',
      questions: [
        {
          id: 103,
          prompt: 'How satisfied are you with our service?',
          allowCustom: false,
          options: [
            { id: 1, optionValue: 1, displayText: 'Very Satisfied' },
            { id: 2, optionValue: 2, displayText: 'Satisfied' },
            { id: 3, optionValue: 3, displayText: 'Neutral' },
            { id: 4, optionValue: 4, displayText: 'Dissatisfied' },
            { id: 5, optionValue: 5, displayText: 'Very Dissatisfied' },
          ],
        },
        {
          id: 104,
          prompt: 'Any additional comments?',
          allowCustom: true,
          options: [],
        },
      ],
    },
    {
      id: '3',
      templateTitle: 'Project Evaluation Template',
      description: 'A template for evaluating project outcomes.',
      questions: [
        {
          id: 105,
          prompt: 'What is the name of the project?',
          allowCustom: true,
          options: [],
        },
        {
          id: 106,
          prompt: 'Rate the project success:',
          allowCustom: false,
          options: [
            { id: 1, optionValue: 1, displayText: '1 - Poor' },
            { id: 2, optionValue: 2, displayText: '2 - Below Average' },
            { id: 3, optionValue: 3, displayText: '3 - Average' },
            { id: 4, optionValue: 4, displayText: '4 - Good' },
            { id: 5, optionValue: 5, displayText: '5 - Excellent' },
          ],
        },
      ],
    },
    {
      id: '4',
      templateTitle: 'Training Feedback Template',
      description: 'A template for collecting feedback on training sessions.',
      questions: [
        {
          id: 107,
          prompt: 'How useful was the training?',
          allowCustom: false,
          options: [
            { id: 1, optionValue: 1, displayText: 'Very Useful' },
            { id: 2, optionValue: 2, displayText: 'Somewhat Useful' },
            { id: 3, optionValue: 3, displayText: 'Not Useful' },
          ],
        },
      ],
    },
    {
      id: '5',
      templateTitle: 'Event Registration Template',
      description: 'A template for registering attendees for an event.',
      questions: [
        {
          id: 108,
          prompt: 'What is your name?',
          allowCustom: true,
          options: [],
        },
        {
          id: 109,
          prompt: 'What is your contact email?',
          allowCustom: true,
          options: [],
        },
      ],
    },
    {
      id: '6',
      templateTitle: 'Survey Template',
      description: 'A simple survey template for various uses.',
      questions: [
        {
          id: 110,
          prompt: 'What is your age group?',
          allowCustom: false,
          options: [
            { id: 1, optionValue: 1, displayText: '18-24' },
            { id: 2, optionValue: 2, displayText: '25-34' },
            { id: 3, optionValue: 3, displayText: '35-44' },
            { id: 4, optionValue: 4, displayText: '45-54' },
            { id: 5, optionValue: 5, displayText: '55+' },
          ],
        },
      ],
    },
    {
      id: '7',
      templateTitle: 'Bug Report Template',
      description: 'A template for reporting bugs in a software system.',
      questions: [
        {
          id: 111,
          prompt: 'Describe the bug:',
          allowCustom: true,
          options: [],
        },
        {
          id: 112,
          prompt: 'What is the severity of the bug?',
          allowCustom: false,
          options: [
            { id: 1, optionValue: 1, displayText: 'Low' },
            { id: 2, optionValue: 2, displayText: 'Medium' },
            { id: 3, optionValue: 3, displayText: 'High' },
            { id: 4, optionValue: 4, displayText: 'Critical' },
          ],
        },
      ],
    },
    {
      id: '8',
      templateTitle: 'Team Meeting Notes Template',
      description: 'A template for recording notes during team meetings.',
      questions: [
        {
          id: 113,
          prompt: 'What is the meeting date?',
          allowCustom: true,
          options: [],
        },
        {
          id: 114,
          prompt: 'Summary of key points:',
          allowCustom: true,
          options: [],
        },
      ],
    },
    {
      id: '9',
      templateTitle: 'WHAT',
      description: 'A template for recording notes during team meetings.',
      questions: [
        {
          id: 115,
          prompt: 'What is the meeting date?',
          allowCustom: true,
          options: [],
        },
        {
          id: 116,
          prompt: 'Summary of key points:',
          allowCustom: true,
          options: [],
        },
      ],
    },
  ];
  
  
  

  getTemplateBases(
    pageSize: number = 5,
    queryCursor?: string,
    searchTerm: string = '',
    searchType: 'name' | 'id' = 'name'
  ): Observable<TemplateBaseResponse> {
    // ✅ Normalize search term
    const normalizedSearch = searchTerm.trim().toLowerCase();
  
    // ✅ Filter templates based on search
    let filteredTemplates = this.templates.filter((t) => {
      const templateTitle = t.templateTitle.toLowerCase();
      const templateId = t.id?.toLowerCase() || ''; // Ensure `id` is always a string
  
      return (
        normalizedSearch === '' ||
        (searchType === 'name' && templateTitle.includes(normalizedSearch)) ||
        (searchType === 'id' && templateId.includes(normalizedSearch))
      );
    });
  
    // ✅ Determine pagination start index
    let startIndex = 0;
    if (queryCursor) {
      const cursorParts = queryCursor.split('_'); // ✅ Split cursor format: "createdAt_id"
      if (cursorParts.length === 2) {
        const cursorId = cursorParts[1];
  
        // ✅ Find the index of the cursor's ID in the filtered list
        const cursorIndex = filteredTemplates.findIndex((t) => t.id === cursorId);
        if (cursorIndex !== -1) {
          startIndex = cursorIndex + 1; // ✅ Start after the last cursor
        }
      }
    }
  
    // ✅ Slice for pagination
    const pageItems = filteredTemplates.slice(startIndex, startIndex + pageSize);
  
    // ✅ Convert to `TemplateBase`
    const templateBases: TemplateBase[] = pageItems.map((t) => ({
      id: t.id ?? `temp-${Date.now()}`, // ✅ Ensure `id` is always a string
      templateTitle: t.templateTitle,
      createdAt: t.createdAt ?? new Date().toISOString(), // ✅ Preserve original `createdAt` if available
      lastUpdated: t.lastUpdated ?? new Date().toISOString(),
      isLocked: false,
    }));
  
    // ✅ Determine next cursor
    const hasMore = startIndex + pageSize < filteredTemplates.length;
    const newNextCursor: string | undefined = hasMore
      ? `${filteredTemplates[startIndex + pageSize]?.createdAt}_${filteredTemplates[startIndex + pageSize]?.id}`
      : undefined; // ✅ Use `undefined` instead of `null`
  
    // ✅ Return response
    const response: TemplateBaseResponse = {
      templateBases,
      queryCursor: newNextCursor, // ✅ No more type errors
      totalCount: filteredTemplates.length, // ✅ Ensure total count is accurate
    };
  
    console.log('Returning Response:', response); // ✅ Debugging log
  
    return of(response).pipe(delay(500));
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