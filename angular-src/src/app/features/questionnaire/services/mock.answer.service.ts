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
    },
    {
      id: 'questionnaire1',
      title: 'Evaluering af SKP-elever',
      description: 'Gennemførelsesprocedure for SKP-elever ved PRAKTIK NORD',
      createdAt: new Date(),
      questions: [
        {
          id: 1,
          text: 'Indlæringsevne',
          allowsCustomAnswer: false,
          options: [
            { id: 1, text: 'Viser lidt eller ingen forståelse for arbejdsopgaverne.' },
            { id: 2, text: 'Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden.' },
            { id: 3, text: 'Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.' },
            { id: 4, text: 'Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.' },
            { id: 5, text: 'Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.' }
          ]
        },
        {
          id: 2,
          text: 'Kreativitet og selvstændighed',
          allowsCustomAnswer: false,
          options: [
            { id: 8, text: 'Viser intet initiativ. Er passiv, uinteresseret og uselvstændig.' },
            { id: 9, text: 'Viser ringe initiativ. Kommer ikke selv med løsningsforslag. Viser ingen interesse i at tilrettelægge eget arbejde.' },
            { id: 10, text: 'Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.' },
            { id: 11, text: 'Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde.' },
            { id: 12, text: 'Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre.' }
          ]
        },
        {
          id: 3,
          text: 'Arbejdsindsats',
          allowsCustomAnswer: false,
          options: [
            { id: 13, text: 'Uacceptabel' },
            { id: 14, text: 'Under middel' },
            { id: 15, text: 'Middel' },
            { id: 16, text: 'Over middel' },
            { id: 17, text: 'Særdeles god' }
          ]
        },
        {
          id: 4,
          text: 'Orden og omhyggelighed',
          allowsCustomAnswer: false,
          options: [
            { id: 18, text: 'Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig.' },
            { id: 19, text: 'Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed.' },
            { id: 20, text: 'Påpasselighed og omhyggelighed middel. Rimelig god orden.' },
            { id: 21, text: 'Meget påpasselig både i praktik og teori. God orden.' },
            { id: 22, text: 'I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden.' }
          ]
        },
        {
          id: 5,
          text : "Ukent",
          allowsCustomAnswer: true,
          options: []
        },
        {
          id: 6,
          text : "Ukent",
          allowsCustomAnswer: true,
          options: []
        },
        {
          id: 7,
          text : "Ukent",
          allowsCustomAnswer: true,
          options: []
        },
        {
          id: 8,
          text: 'Mødestabilitet',
          allowsCustomAnswer: false,
          options: [
            { id: 23, text: 'Du møder ikke hver dag til tiden.' },
            { id: 24, text: 'Du møder næsten hver dag til tiden.' },
            { id: 25, text: 'Du møder hver dag til tiden.' }
          ]
        },
        {
          id: 9,
          text: 'Sygdom',
          allowsCustomAnswer: false,
          options: [
            { id: 26, text: 'Du melder ikke afbud ved sygdom.' },
            { id: 27, text: 'Du melder, for det meste afbud, når du er syg.' },
            { id: 28, text: 'Du melder afbud, når du er syg.' }
          ]
        },
        {
          id: 10,
          text: 'Fravær',
          allowsCustomAnswer: false,
          options: [
            { id: 29, text: 'Du har et stort fravær.' },
            { id: 30, text: 'Du har noget fravær.' },
            { id: 31, text: 'Du har stort set ingen fravær.' },
            { id: 32, text: 'Du har ingen fravær.' }
          ]
        },
        {
          id: 11,
          text: 'Praktikpladssøgning',
          allowsCustomAnswer: false,
          options: [
            { id: 33, text: 'Du søger ingen praktikpladser.' },
            { id: 34, text: 'Du ved, at du skal søge alle relevante praktikpladser, men det kniber med handlingen.' },
            { id: 35, text: 'Du søger alle relevante praktikpladser, men skal have hjælp til at søge praktikpladser, der ligger længere væk end i din bopælskommune.' },
            { id: 36, text: 'Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune.' },
            { id: 37, text: 'Du søger alle relevante praktikpladser også dem der ligger uden for din bopælskommune. Du søger også praktikplads inden for en anden uddannelse, som dit GF giver adgang til.' }
          ]
        },
        {
          id: 12,
          text: 'Synlighed',
          allowsCustomAnswer: false,
          options: [
            { id: 38, text: 'Du har ikke en synlig profil på praktikpladsen.dk.' },
            { id: 39, text: 'Du skal ofte påmindes om at synliggøre din profil på praktikpladsen.dk.' },
            { id: 40, text: 'Du har altid en synlig, men ikke opdateret profil på praktikpladsen.dk.' },
            { id: 41, text: 'Du har altid en opdateret og synlig profil på praktikpladsen.dk.' }
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