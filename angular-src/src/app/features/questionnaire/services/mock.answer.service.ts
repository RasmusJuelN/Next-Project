import { inject, Injectable } from '@angular/core';
import { Questionnaire } from '../models/answer.model';
import { delay, Observable, of } from 'rxjs';
import { AuthService } from '../../../core/services/auth.service';

@Injectable({
  providedIn: 'root',
})
export class MockAnswerService {
  private authService = inject(AuthService);
  // Mock question templates
  private mockTemplates: Questionnaire[] = [
    {
      id: 'questionnaire1-TEST',
      title: 'Programming Questionnaire',
      description: 'A short questionnaire about programming preferences.',
      questions: [
        {
          id: 1,
          prompt: 'What is your favorite programming language?',
          options: [
            { id: 101, displayText: 'JavaScript' },
            { id: 102, displayText: 'Python' },
            { id: 103, displayText: 'C#' },
            { id: 104, displayText: 'Java' },
          ],
          allowCustom: true,
        },
        {
          id: 2,
          prompt: 'What is your preferred development environment?',
          options: [
            { id: 201, displayText: 'VS Code' },
            { id: 202, displayText: 'IntelliJ' },
            { id: 203, displayText: 'Eclipse' },
          ],
          allowCustom: true,
        },
        {
          id: 3,
          prompt: 'What type of projects do you usually work on?',
          options: [
            { id: 301, displayText: 'Web Applications' },
            { id: 302, displayText: 'Mobile Applications' },
            { id: 303, displayText: 'Desktop Applications' },
            { id: 304, displayText: 'Embedded Systems' },
          ],
          allowCustom: false,
        },
        {
          id: 4,
          prompt: 'Which version control system do you prefer?',
          options: [
            { id: 401, displayText: 'Git' },
            { id: 402, displayText: 'SVN' },
            { id: 403, displayText: 'Mercurial' },
          ],
          allowCustom: true,
        },
        {
          id: 5,
          prompt: 'How many years of programming experience do you have?',
          options: [
            { id: 501, displayText: 'Less than 1 year' },
            { id: 502, displayText: '1-3 years' },
            { id: 503, displayText: '3-5 years' },
            { id: 504, displayText: 'More than 5 years' },
          ],
          allowCustom: false,
        },
      ],
      activatedAt: new Date(),
    },
    // Add more templates as needed
    {
      id: 'questionnaire2',
      title: 'Software Development Survey',
      description: 'A brief survey on software development practices.',
      questions: [
        {
          id: 6,
          prompt: 'Which software development methodology do you follow?',
          options: [
            { id: 601, displayText: 'Agile' },
            { id: 602, displayText: 'Waterfall' },
            { id: 603, displayText: 'Scrum' },
          ],
          allowCustom: true,
        },
        {
          id: 7,
          prompt: 'What is your preferred programming paradigm?',
          options: [
            { id: 701, displayText: 'Object-Oriented Programming' },
            { id: 702, displayText: 'Functional Programming' },
            { id: 703, displayText: 'Procedural Programming' }
          ],
          allowCustom: true,
        },
        {
          id: 8,
          prompt: 'How often do you write unit tests?',
          options: [
            { id: 801, displayText: 'Always' },
            { id: 802, displayText: 'Often' },
            { id: 803, displayText: 'Sometimes' },
            { id: 804, displayText: 'Never' },
          ],
          allowCustom: false,
        },
      ],
      activatedAt: new Date(),
    },
    {
      id: 'questionnaire1',
      title: 'Evaluering af SKP-elever',
      description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
      activatedAt: new Date(),
      questions: [
        {
          id: 1,
          prompt: 'Indlæringsevne',
          allowCustom: false,
          options: [
            { id: 1, displayText: 'Viser lidt eller ingen forståelse for arbejdsopgaverne.' },
            { id: 2, displayText: 'Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden.' },
            { id: 3, displayText: 'Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.' },
            { id: 4, displayText: 'Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.' },
            { id: 5, displayText: 'Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.' }
          ]
        },
        {
          id: 2,
          prompt: 'Kreativitet og selvstændighed',
          allowCustom: false,
          options: [
            { id: 8, displayText: 'Viser intet initiativ. Er passiv, uinteresseret og uselvstændig.' },
            { id: 9, displayText: 'Viser ringe initiativ. Kommer ikke selv med løsningsforslag. Viser ingen interesse i at tilrettelægge eget arbejde.' },
            { id: 10, displayText: 'Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.' },
            { id: 11, displayText: 'Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde.' },
            { id: 12, displayText: 'Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre.' }
          ]
        },
        {
          id: 3,
          prompt: 'Arbejdsindsats',
          allowCustom: false,
          options: [
            { id: 13, displayText: 'Uacceptabel' },
            { id: 14, displayText: 'Under middel' },
            { id: 15, displayText: 'Middel' },
            { id: 16, displayText: 'Over middel' },
            { id: 17, displayText: 'Særdeles god' }
          ]
        },
        {
          id: 4,
          prompt: 'Orden og omhyggelighed',
          allowCustom: false,
          options: [
            { id: 18, displayText: 'Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig.' },
            { id: 19, displayText: 'Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed.' },
            { id: 20, displayText: 'Påpasselighed og omhyggelighed middel. Rimelig god orden.' },
            { id: 21, displayText: 'Meget påpasselig både i praktik og teori. God orden.' },
            { id: 22, displayText: 'I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden.' }
          ]
        },
        {
          id: 5,
          prompt : "Ukent",
          allowCustom: true,
          options: []
        },
        {
          id: 6,
          prompt : "Ukent",
          allowCustom: true,
          options: []
        },
        {
          id: 7,
          prompt : "Ukent",
          allowCustom: true,
          options: []
        },
        {
          id: 8,
          prompt: 'Mødestabilitet',
          allowCustom: false,
          options: [
            { id: 23, displayText: 'Du møder ikke hver dag til tiden.' },
            { id: 24, displayText: 'Du møder næsten hver dag til tiden.' },
            { id: 25, displayText: 'Du møder hver dag til tiden.' }
          ]
        },
        {
          id: 9,
          prompt: 'Sygdom',
          allowCustom: false,
          options: [
            { id: 26, displayText: 'Du melder ikke afbud ved sygdom.' },
            { id: 27, displayText: 'Du melder, for det meste afbud, når du er syg.' },
            { id: 28, displayText: 'Du melder afbud, når du er syg.' }
          ]
        },
        {
          id: 10,
          prompt: 'Fravær',
          allowCustom: false,
          options: [
            { id: 29, displayText: 'Du har et stort fravær.' },
            { id: 30, displayText: 'Du har noget fravær.' },
            { id: 31, displayText: 'Du har stort set ingen fravær.' },
            { id: 32, displayText: 'Du har ingen fravær.' }
          ]
        },
        {
          id: 11,
          prompt: 'Praktikpladssøgning',
          allowCustom: false,
          options: [
            { id: 33, displayText: 'Du søger ingen praktikpladser.' },
            { id: 34, displayText: 'Du ved, at du skal søge alle relevante praktikpladser, men det kniber med handlingen.' },
            { id: 35, displayText: 'Du søger alle relevante praktikpladser, men skal have hjælp til at søge praktikpladser, der ligger længere væk end i din bopælskommune.' },
            { id: 36, displayText: 'Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune.' },
            { id: 37, displayText: 'Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune. Du søger også praktikplads inden for en anden uddannelse, som dit GF giver adgang til.' }
          ]
        },
        {
          id: 12,
          prompt: 'Synlighed',
          allowCustom: false,
          options: [
            { id: 38, displayText: 'Du har ikke en synlig profil på praktikpladsen.dk.' },
            { id: 39, displayText: 'Du skal ofte påmindes om at synliggøre din profil på praktikpladsen.dk.' },
            { id: 40, displayText: 'Du har altid en synlig, men ikke opdateret profil på praktikpladsen.dk.' },
            { id: 41, displayText: 'Du har altid en opdateret og synlig profil på praktikpladsen.dk.' }
          ]
        }
      ]
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
    return of(template).pipe(delay(2000));
  }
}