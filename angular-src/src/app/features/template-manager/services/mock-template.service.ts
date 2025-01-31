import { Injectable } from '@angular/core';
import { PaginationResponse, Question, Template } from '../models/template.model';
import { Observable, of } from 'rxjs';

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
  ];
  

  constructor() {}

  getTemplates(
    page: number,
    pageSize: number,
    searchTerm: string = '',
    searchType: string = 'name' // Default search type
  ): Observable<PaginationResponse<Template>> {
    let filteredTemplates = this.templates;
  
    // Filter templates based on the search term and search type
    if (searchTerm) {
      if (searchType === 'name') {
        filteredTemplates = filteredTemplates.filter((template) =>
          template.title.toLowerCase().includes(searchTerm.toLowerCase())
        );
      } else if (searchType === 'id') {
        filteredTemplates = filteredTemplates.filter((template) =>
          template.id.toLowerCase().includes(searchTerm.toLowerCase())
        );
      }
    }
  
    // Calculate pagination
    const totalItems = filteredTemplates.length;
    const totalPages = Math.ceil(totalItems / pageSize);
    const start = (page - 1) * pageSize;
    const paginatedTemplates = filteredTemplates.slice(start, start + pageSize);
  
    // Return the paginated response
    return of({
      items: paginatedTemplates, // The current page of templates
      totalItems: totalItems, // Total number of filtered items
      currentPage: page, // The current page
      pageSize: pageSize, // Items per page
      totalPages: totalPages, // Total number of pages
    });
  }
  
  updateTemplate(templateId: string, updatedTemplate: Template): Observable<void> {
    // Find the index of the template to update
    const templateIndex = this.templates.findIndex((t) => t.id === templateId);
  
    if (templateIndex === -1) {
      throw new Error(`Template with ID ${templateId} not found`);
    }
  
    // Update the template
    this.templates[templateIndex] = { ...updatedTemplate };
  
    // Simulate async response
    return of();
  }
  
  

  addTemplate(template: Template): Observable<Template> {
    const newTemplate = { ...template, id: Date.now().toString() }; // Generate a unique ID for the template
    this.templates.push(newTemplate); // Add the new template to the list
    return of(newTemplate); // Return the newly added template
  }
  

  deleteTemplate(templateId: string) {
    this.templates = this.templates.filter((template) => template.id !== templateId);
    return of();
  }
}