import { inject, Injectable } from '@angular/core';
import { ActiveQuestionnaire, User, QuestionTemplate, AnswerSession } from '../../models/questionare';
import { LocalStorageService } from '../misc/local-storage.service';
import { LogEntry } from '../../models/log-models';


export type LogFileType = 'sql' | 'backend' | 'settings_manager';
interface MockLogs {
  sql: LogEntry[];
  backend: LogEntry[];
  settings_manager: LogEntry[];
}

@Injectable({
  providedIn: 'root'
})
export class MockDbService {
  private localStorageKey = 'mockData';
  private localStorageService = inject(LocalStorageService);

  private predefinedMockData: {
    mockUsers: User[],
    mockAnswers: AnswerSession[],
    mockActiveQuestionnaire: ActiveQuestionnaire[],
    mockQuestionTemplates: QuestionTemplate[],
    mockAppSettings: {
      settings: any;
      metadata: any;
    },
    mockLogs: MockLogs 
  } = {
    mockUsers: [
      { id: "1", userName: "MJ", fullName: "Max Jacobsen", role: "teacher" },
      { id: "2", userName: "NN", fullName: "Nicklas Nilsson", role: "student" },
      { id: "3", userName: "AS", fullName: "Alexander Svensson", role: "student" },
      { id: "4", userName: "JW", fullName: "Johan Wallin", role: "student" }
    ],
    mockAnswers: [
      {
        questionnaireId: "efgh",
        studentAnswers: {
          user: { id: "2", userName: "NN", fullName: "Nicklas Nilsson", role: "student" },
          answers: [
            { questionId: 1, selectedOptionId: 1 },
            { questionId: 2, selectedOptionId: 2 },
            { questionId: 3, customAnswer: 'More interactive activities would be great!' },
            { questionId: 4, selectedOptionId: 2 },
          ],
          answeredAt: new Date()
        },
        teacherAnswers: {
          user: { id: "1", userName: "MJ", fullName: "Max Jacobsen", role: "teacher" },
          answers: [
            { questionId: 1, selectedOptionId: 1 },
            { questionId: 2, selectedOptionId: 1 },
            { questionId: 3, customAnswer: 'Better resources and tools for students are needed.' },
            { questionId: 4, selectedOptionId: 1 },
          ],
          answeredAt: new Date()
        }
      }
    ],
    mockActiveQuestionnaire: [
      {
        id: "efgh",
        student: { id: "2", userName: "NN", fullName: "Nicklas Nilsson", role: "student" },
        teacher: { id: "1", userName: "MJ", fullName: "Max Jacobsen", role: "teacher" },
        isStudentFinished: true,
        isTeacherFinished: true,
        template: {
          id: 'template1',
          title: 'Employee Performance Review',
          description: 'A template for assessing employee performance in various aspects of their job.'
        }
      },
      {
        id: "ijkl",
        student: { id: "3", userName: "AS", fullName: "Alexander Svensson", role: "student" },
        teacher: { id: "1", userName: "MJ", fullName: "Max Jacobsen", role: "teacher" },
        isStudentFinished: false,
        isTeacherFinished: false,
        template: {
          id: 'template1',
          title: 'Employee Performance Review',
          description: 'A template for assessing employee performance in various aspects of their job.'
        }
      }
    ],
    mockQuestionTemplates: [
      {
        id: 'template1',
        title: 'Employee Performance Review',
        description: 'A template for assessing employee performance in various aspects of their job.',
        questions: [
          {
            id: 1,  // Unique ID for each question
            title: 'Indlæringsevne',
            options: [
              { id: 1, value: 1, label: 'Viser lidt eller ingen forståelse for arbejdsopgaverne' },
              { id: 2, value: 2, label: 'Forstår arbejdsopgaverne, men kan ikke anvende den i praksis. Har svært ved at tilegne sig ny viden' },
              { id: 3, value: 3, label: 'Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.' },
              { id: 4, value: 4, label: 'Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.' },
              { id: 5, value: 5, label: 'Behøver næsten ingen oplæring. Kan ved selvstudium, endog ved svært tilgængeligt materiale, tilegne sig ny viden.' }
            ]
          },
          {
            id: 2,  // Unique ID for each question
            title: 'Kreativitet og selvstændighed',
            options: [
              { id: 1, value: 1, label: 'Viser intet initiativ. Er passiv, uinteresseret og uselvstændig' },
              { id: 2, value: 2, label: 'Viser ringe initiativ. Kommer ikke med løsningsforslag. Viser ingen interesse i at tilægge eget arbejde.' },
              { id: 3, value: 3, label: 'Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.' },
              { id: 4, value: 4, label: 'Meget initiativrig. Kommer selv med løsningsforslag. Gode evner for at tilrettelægge eget og andres arbejde.' },
              { id: 5, value: 5, label: 'Overordentlig initiativrig. Løser selv problemerne. Tilrettelægger selvstændigt arbejdet for mig selv og andre.' }
            ]
          },
          {
            id: 3,  // Unique ID for each question
            title: 'Arbejdsindsats',
            options: [
              { id: 1, value: 1, label: 'Uacceptabel' },
              { id: 2, value: 2, label: 'Under middel' },
              { id: 3, value: 3, label: 'Middel' },
              { id: 4, value: 4, label: 'Over middel' },
              { id: 5, value: 5, label: 'Særdeles god' }
            ]
          },
          {
            id: 4,  // Unique ID for each question
            title: 'Orden og omhyggelighed',
            options: [
              { id: 1, value: 1, label: 'Omgås materialer, maskiner og værktøj på en sløset og ligegyldig måde. Holder ikke sin arbejdsplads ordentlig.' },
              { id: 2, value: 2, label: 'Bruger maskiner og værktøj uden megen omtanke. Mindre god orden og omhyggelighed.' },
              { id: 3, value: 3, label: 'Bruger maskiner, materialer og værktøj med påpasselighed og omhyggelighed middel. Rimelig god orden.' },
              { id: 4, value: 4, label: 'Meget påpasselig både i praktik og teori. God orden.' },
              { id: 5, value: 5, label: 'I høj grad påpasselig. God forståelse for materialevalg. Særdeles god orden.' }
            ]
          }
        ]
      }
    ],
    mockAppSettings: {
      settings: {
        auth: {
          access_token_expire_minutes: 30,
          algorithm: 'HS256',
          domain: 'localhost',
          ldap_base_dn: 'dc=example,dc=com',
          ldap_server: 'ldap://localhost',
          scopes: {
            admin: 'admin',
            student: 'student',
            teacher: 'teacher',
          },
          secret_key: null,
        },
        database: {
          database_driver: null,
          database_type: 'mysql',
          db_name: 'program.db',
          host: null,
          max_connections: null,
          min_connections: null,
          password: null,
          port: null,
          ssl_ca_cert_file: null,
          ssl_cert_file: null,
          ssl_key_file: null,
          timeout: null,
          use_ssl: false,
          user: null,
        },
      },
      metadata: {
        auth: {
          access_token_expire_minutes: {
            default: 30,
            minValue: 1,
            maxValue: 1440,
            canBeEmpty: false,
            description: 'Access token expiration in minutes',
          },
          algorithm: {
            default: 'HS256',
            canBeEmpty: false,
            description: 'Algorithm used for authentication',
          },
          domain: {
            default: 'localhost',
            canBeEmpty: false,
            description: 'Domain name',
          },
          ldap_base_dn: {
            default: 'dc=example,dc=com',
            canBeEmpty: false,
            description: 'LDAP base DN',
          },
          ldap_server: {
            default: 'ldap://localhost',
            canBeEmpty: false,
            description: 'LDAP server URL',
          },
          scopes: {
            canBeEmpty: false,
            description: 'Access scopes',
          },
          secret_key: {
            default: '',
            canBeEmpty: true,
            description: 'Secret key used for authentication',
          },
        },
        database: {
          database_driver: {
            canBeEmpty: true,
            description: 'Database driver',
          },
          database_type: {
            allowedValues: ['sqlite', 'postgresql', 'mysql'],
            canBeEmpty: false,
            description: 'Type of the database',
          },
          db_name: {
            default: 'program.db',
            canBeEmpty: false,
            description: 'Database name',
          },
          host: {
            canBeEmpty: true,
            description: 'Database host',
          },
          max_connections: {
            canBeEmpty: true,
            description: 'Maximum number of database connections',
          },
          min_connections: {
            canBeEmpty: true,
            description: 'Minimum number of database connections',
          },
          password: {
            canBeEmpty: true,
            description: 'Database password',
          },
          port: {
            canBeEmpty: true,
            description: 'Database port',
          },
          ssl_ca_cert_file: {
            canBeEmpty: true,
            description: 'SSL CA certificate file',
          },
          ssl_cert_file: {
            canBeEmpty: true,
            description: 'SSL certificate file',
          },
          ssl_key_file: {
            canBeEmpty: true,
            description: 'SSL key file',
          },
          timeout: {
            canBeEmpty: true,
            description: 'Database connection timeout',
          },
          use_ssl: {
            default: false,
            canBeEmpty: false,
            description: 'Use SSL for database connection',
          },
          user: {
            canBeEmpty: true,
            description: 'Database user',
          },
        },
      },
    },
    mockLogs: {
      sql: [
        {
          timestamp: '2024-09-20 09:53:54',
          severity: 'DEBUG',
          source: 'sqlalchemy.orm.mapper.Mapper',
          message: '(Option|options) _configure_property(options, _RelationshipDeclared)',
        },
        {
          timestamp: '2024-09-20 09:53:54',
          severity: 'DEBUG',
          source: 'sqlalchemy.orm.mapper.Mapper',
          message: '(QuestionTemplate|question_templates) Col (\'cid\', \'name\', \'type\', \'notnull\', \'dflt_value\', \'pk\')',
        },
        {
          timestamp: '2024-09-20 09:53:54',
          severity: 'INFO',
          source: 'sqlalchemy.orm.mapper.Mapper',
          message: '(Question|questions) BEGIN (implicit)',
        },
        {
          timestamp: '2024-09-20 09:53:55',
          severity: 'INFO',
          source: 'sqlalchemy.orm.mapper.Mapper',
          message: '(Question|questions) Identified primary key columns',
        },
        {
          timestamp: '2024-09-20 09:53:55',
          severity: 'DEBUG',
          source: 'sqlalchemy.orm.mapper.Mapper',
          message: '(Option|options) PRAGMA main.table_info("options")',
        }
      ],
      backend: [
        {
          timestamp: '2024-09-20 09:53:56',
          severity: 'DEBUG',
          source: 'backend.service.Service',
          message: 'Initiating request to /api/questions',
        },
        {
          timestamp: '2024-09-20 09:53:56',
          severity: 'INFO',
          source: 'backend.database.Connection',
          message: 'Created new connection to backend database',
        },
        {
          timestamp: '2024-09-20 09:53:57',
          severity: 'WARNING',
          source: 'backend.cache.CacheManager',
          message: 'Cache miss for key "question_list"',
        },
        {
          timestamp: '2024-09-20 09:53:57',
          severity: 'DEBUG',
          source: 'backend.service.Service',
          message: 'Response received from /api/questions with status 200',
        },
        {
          timestamp: '2024-09-20 09:53:58',
          severity: 'INFO',
          source: 'backend.database.Transaction',
          message: 'Transaction committed',
        }
      ],
      settings_manager: [
        {
          timestamp: '2024-09-20 09:53:59',
          severity: 'INFO',
          source: 'settings.Manager',
          message: 'Loading configuration from file settings.yaml',
        },
        {
          timestamp: '2024-09-20 09:53:59',
          severity: 'DEBUG',
          source: 'settings.Validator',
          message: 'Validating setting "max_connections"',
        },
        {
          timestamp: '2024-09-20 09:54:00',
          severity: 'INFO',
          source: 'settings.Manager',
          message: 'Applying new settings for max_connections=10',
        },
        {
          timestamp: '2024-09-20 09:54:01',
          severity: 'DEBUG',
          source: 'settings.Manager',
          message: 'Saving configuration to file settings.yaml',
        },
        {
          timestamp: '2024-09-20 09:54:02',
          severity: 'WARNING',
          source: 'settings.Validator',
          message: 'Invalid value for "timeout", using default of 30 seconds',
        }
      ]
    }
  };
  

  public mockData = { ...this.predefinedMockData };

  loadInitialMockData() {
    const savedData = this.localStorageService.getData(this.localStorageKey);
    if (savedData) {
      this.mockData = JSON.parse(savedData);
    } else {
      this.mockData = { ...this.predefinedMockData };
      this.saveData();
    }
  }

  saveData() {
    this.localStorageService.saveData(this.localStorageKey, JSON.stringify(this.mockData));
  }
}
