import { Injectable } from '@angular/core';
import { Question, Template, TemplateBase, TemplateBaseResponse, TemplateStatus } from '../models/template.model';
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
      draftStatus: 'finalized',
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
      title: 'Customer Feedback Template',
      description: 'A template for collecting customer feedback.',
      draftStatus: 'finalized',
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
      title: 'Project Evaluation Template',
      description: 'A template for evaluating project outcomes.',
      draftStatus: 'finalized',
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
      title: 'Training Feedback Template',
      description: 'A template for collecting feedback on training sessions.',
      draftStatus: 'finalized',
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
      title: 'Event Registration Template',
      description: 'A template for registering attendees for an event.',
      draftStatus: 'draft',
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
      title: 'Survey Template',
      description: 'A simple survey template for various uses.',
      draftStatus: 'draft',
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
      title: 'Bug Report Template',
      description: 'A template for reporting bugs in a software system.',
      draftStatus: 'draft',
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
      title: 'Team Meeting Notes Template',
      draftStatus: 'draft',
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
      title: 'WHAT',
      description: 'A template for recording notes during team meetings.',
      draftStatus: 'finalized',
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
    {
      id: '10',
      title: 'Evaluering af SKP-elever',
      description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
      createdAt: new Date().toISOString(),
      lastUpdated: new Date().toISOString(),
      isLocked: false,
      draftStatus: 'finalized',
      questions: [
        {
          id: 1,
          prompt: 'Indlæringsevne',
          allowCustom: false,
          options: [
            { id: 1, optionValue: 1, displayText: 'Viser lidt eller ingen forståelse for arbejdsopgaverne.' },
            { id: 2, optionValue: 2, displayText: 'Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden.' },
            { id: 3, optionValue: 3, displayText: 'Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.' },
            { id: 4, optionValue: 4, displayText: 'Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.' },
            { id: 5, optionValue: 5, displayText: 'Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.' }
          ]
        },
        {
          id: 2,
          prompt: 'Kreativitet og selvstændighed',
          allowCustom: false,
          options: [
            { id: 8, optionValue: 1, displayText: 'Viser intet initiativ. Er passiv, uinteresseret og uselvstændig.' },
            { id: 9, optionValue: 2, displayText: 'Viser ringe initiativ. Kommer ikke selv med løsningsforslag. Viser ingen interesse i at tilrettelægge eget arbejde.' },
            { id: 10, optionValue: 3, displayText: 'Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.' },
            { id: 11, optionValue: 4, displayText: 'Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde.' },
            { id: 12, optionValue: 5, displayText: 'Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre.' }
          ]
        },
        {
          id: 3,
          prompt: 'Arbejdsindsats',
          allowCustom: false,
          options: [
            { id: 13, optionValue: 1, displayText: 'Uacceptabel' },
            { id: 14, optionValue: 2, displayText: 'Under middel' },
            { id: 15, optionValue: 3, displayText: 'Middel' },
            { id: 16, optionValue: 4, displayText: 'Over middel' },
            { id: 17, optionValue: 5, displayText: 'Særdeles god' }
          ]
        },
        {
          id: 4,
          prompt: 'Orden og omhyggelighed',
          allowCustom: false,
          options: [
            { id: 18, optionValue: 1, displayText: 'Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig.' },
            { id: 19, optionValue: 2, displayText: 'Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed.' },
            { id: 20, optionValue: 3, displayText: 'Påpasselighed og omhyggelighed middel. Rimelig god orden.' },
            { id: 21, optionValue: 4, displayText: 'Meget påpasselig både i praktik og teori. God orden.' },
            { id: 22, optionValue: 5, displayText: 'I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden.' }
          ]
        },
        {
          id: 8,
          prompt: 'Mødestabilitet',
          allowCustom: false,
          options: [
            { id: 23, optionValue: 1, displayText: 'Du møder ikke hver dag til tiden.' },
            { id: 24, optionValue: 2, displayText: 'Du møder næsten hver dag til tiden.' },
            { id: 25, optionValue: 3, displayText: 'Du møder hver dag til tiden.' }
          ]
        },
        {
          id: 9,
          prompt: 'Sygdom',
          allowCustom: false,
          options: [
            { id: 26, optionValue: 1, displayText: 'Du melder ikke afbud ved sygdom.' },
            { id: 27, optionValue: 2, displayText: 'Du melder, for det meste afbud, når du er syg.' },
            { id: 28, optionValue: 3, displayText: 'Du melder afbud, når du er syg.' }
          ]
        },
        {
          id: 10,
          prompt: 'Fravær',
          allowCustom: false,
          options: [
            { id: 29, optionValue: 1, displayText: 'Du har et stort fravær.' },
            { id: 30, optionValue: 2, displayText: 'Du har noget fravær.' },
            { id: 31, optionValue: 3, displayText: 'Du har stort set ingen fravær.' },
            { id: 32, optionValue: 4, displayText: 'Du har ingen fravær.' }
          ]
        },
        {
          id: 11,
          prompt: 'Praktikpladssøgning',
          allowCustom: false,
          options: [
            { id: 33, optionValue: 1, displayText: 'Du søger ingen praktikpladser.' },
            { id: 34, optionValue: 2, displayText: 'Du ved, at du skal søge alle relevante praktikpladser, men det kniber med handlingen.' },
            { id: 35, optionValue: 3, displayText: 'Du søger alle relevante praktikpladser, men skal have hjælp til at søge praktikpladser, der ligger længere væk end i din bopælskommune.' },
            { id: 36, optionValue: 4, displayText: 'Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune.' },
            { id: 37, optionValue: 5, displayText: 'Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune. Du søger også praktikplads inden for en anden uddannelse, som dit GF giver adgang til.' }
          ]
        },
        {
          id: 12,
          prompt: 'Synlighed',
          allowCustom: false,
          options: [
            { id: 38, optionValue: 1, displayText: 'Du har ikke en synlig profil på praktikpladsen.dk.' },
            { id: 39, optionValue: 2, displayText: 'Du skal ofte påmindes om at synliggøre din profil på praktikpladsen.dk.' },
            { id: 40, optionValue: 3, displayText: 'Du har altid en synlig, men ikke opdateret profil på praktikpladsen.dk.' },
            { id: 41, optionValue: 4, displayText: 'Du har altid en opdateret og synlig profil på praktikpladsen.dk.' }
          ]
        }
      ]
    }
  ];
  
  
  private ensureDraft(t: Template & { draftStatus: TemplateStatus }) {
  if (t.draftStatus !== 'draft') {
    throw new Error('Can’t modify a finalized template – copy it first.');
  }
}

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
      const templateTitle = t.title.toLowerCase();
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
      title: t.title,
      createdAt: t.createdAt ?? new Date().toISOString(), // ✅ Preserve original `createdAt` if available
      lastUpdated: t.lastUpdated ?? new Date().toISOString(),
      isLocked: t.draftStatus === 'finalized',
      draftStatus: t.draftStatus
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
  const draft: Template = {
    ...template,
    id: Date.now().toString(),
    draftStatus: 'draft',                       // always start in draft
    createdAt: Date.now().toString(),
    lastUpdated: Date.now().toString(),
  };
  this.templates.push(draft);
  return of(draft).pipe(delay(300));
}


// ugrade to finlize state
upgradeTemplate(templateId: string): Observable<Template> {
  const tmpl = this.templates.find(t => t.id === templateId);
  if (!tmpl) throw new Error('Template not found');
  this.ensureDraft(tmpl);
  tmpl.draftStatus = 'finalized';
  tmpl.lastUpdated = new Date().toISOString();
  return of(tmpl).pipe(delay(300));
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