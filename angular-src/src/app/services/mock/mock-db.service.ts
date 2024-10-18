import { inject, Injectable } from '@angular/core';
import { ActiveQuestionnaire, User, QuestionTemplate, AnswerSession } from '../../models/questionare';
import { LocalStorageService } from '../misc/local-storage.service';
import { LogEntry } from '../../models/log-models';


export type LogFileType = 'sql' | 'backend' | 'settings_manager';
interface MockLogs {
  [key: string]: LogEntry[];
}

@Injectable({
  providedIn: 'root'
})
export class MockDbService {
  private localStorageKey = 'mockData';
  private localStorageService = inject(LocalStorageService);

  private predefinedMockData: {
    mockUsers: User[],
    mockAnswerSessions: AnswerSession[],
    mockActiveQuestionnaire: ActiveQuestionnaire[],
    mockQuestionTemplates: QuestionTemplate[],
    mockAppSettings: {
      settings: any;
      metadata: any;
    },
    mockLogs: MockLogs
  } = {
    mockUsers: [
      { id: "1", userName: "userteacher", fullName: "Teach", role: "teacher" },
      { id: "2", userName: "NH", fullName: "Nicklas Helsberg", role: "student" },
      { id: "3", userName: "MF", fullName: "Max Felding", role: "student" },
      { id: "4", userName: "AMT", fullName: "Alexander Thamdrup", role: "student" },
      { id: "5", userName: "sysadmin", fullName: "Admin", role: "admin" }
    ],
    mockAnswerSessions: [
      {
        questionnaireId: "efgh",
        users: {
          student: { id: "2", userName: "NH", fullName: "Nicklas Helsberg", role: "student" },
          teacher: { id: "1", userName: "userteacher", fullName: "Teach", role: "teacher" }
        },
        answers : [
          { questionId: 1, questionTitle: 'Indlæringsevne', studentAnswer: 'Let ved at forstå arbejdsopgaverne og anvende den i praksis. Har let ved at tilegne sig ny viden.', teacherAnswer: 'Mindre behov for oplæring end normalt. Kan selv finde/tilegne sig ny viden.' },
          { questionId: 2, questionTitle: 'Kreativitet og selvstændighed', studentAnswer: 'Viser normalt initiativ. Kommer selv med løsningsforslag. Tilrettelægger eget arbejde.', teacherAnswer: 'Viser ringe initiativ. Kommer ikke med løsningsforslag. Viser ingen interesse i at tilægge eget arbejde.' },
          { questionId: 3, questionTitle: 'Arbejdsindsats', studentAnswer: 'Middel', teacherAnswer: 'Over middel' },
          { questionId: 4, questionTitle: 'Orden og omhyggelighed', studentAnswer: 'Meget påpasselig både i praktik og teori. God orden.', teacherAnswer: 'God forståelse for materialevalg. Særdeles god orden.' }
        ],        
        studentAnsweredAt: new Date('2024-10-14T08:30:00.000Z'),
        teacherAnsweredAt: new Date('2024-10-14T09:00:00.000Z')
      },
      {
        questionnaireId: "ijkl",
        users: {
          student: { id: "4", userName: "AMT", fullName: "Alexander Thamdrup", role: "student" },
          teacher: { id: "1", userName: "userteacher", fullName: "Teach", role: "teacher" }
        },
        answers: [
          { questionId: 1, questionTitle: 'Indlæringsevne', studentAnswer: '', teacherAnswer: 'Not answered yet' },
          { questionId: 2, questionTitle: 'Kreativitet og selvstændighed', studentAnswer: '', teacherAnswer: '' },
          { questionId: 3, questionTitle: 'Arbejdsindsats', studentAnswer: '', teacherAnswer: '' },
          { questionId: 4, questionTitle: 'Orden og omhyggelighed', studentAnswer: '', teacherAnswer: '' }
        ],
        studentAnsweredAt: new Date('2024-10-14T08:30:00.000Z'),
        teacherAnsweredAt: new Date('2024-10-14T08:30:00.000Z')
      }
    ],
    mockActiveQuestionnaire: [
      {
        id: "efgh",
        student: { id: "2", userName: "NH", fullName: "Nicklas Helsberg", role: "student" },
        teacher: { id: "1", userName: "userteacher", fullName: "Teach", role: "teacher" },
        studentFinishedAt: new Date(),
        teacherFinishedAt: new Date(),
        template: {
          id: 'template1',
          title: 'Employee Performance Review',
          description: 'A template for assessing employee performance in various aspects of their job.'
        },
        createdAt: new Date()
      },
      {
        id: "ijkl",
        student: { id: "4", userName: "AMT", fullName: "Alexander Thamdrup", role: "student" },
        teacher: { id: "1", userName: "userteacher", fullName: "Teach", role: "teacher" },
        studentFinishedAt: null,
        teacherFinishedAt: null,
        template: {
          id: 'template1',
          title: 'Employee Performance Review',
          description: 'A template for assessing employee performance in various aspects of their job.'
        },
        createdAt: new Date()
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
          ad_service_account: 'svc_backend_queries',
          ad_service_password: 'Pa$$w0rd',
          algorithm: 'HS256',
          authentication_method: 'NTLM',
          domain: '10.0.1.139',
          ldap_base_dn: 'dc=next,dc=dev',
          ldap_server: 'ldap://10.0.1.139',
          salt_hash: 'b1865f9a4615cbcac2956f076a741841192dc65f4f6eebac66cc5ade440366c8',
          scopes: {
            admin: 'admin',
            student: 'student',
            teacher: 'teacher',
          },
          secret_key: 'e33228799f663a35c4be9343cecd991ffedc6d6a88e6c20463744bd6ade87108',
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
          ad_service_account: {
            default: 'svc_backend_queries',
            canBeEmpty: false,
            description: 'AD Service Account',
          },
          ad_service_password: {
            default: 'Pa$$w0rd',
            canBeEmpty: false,
            description: 'AD Service Password',
          },
          algorithm: {
            default: 'HS256',
            canBeEmpty: false,
            description: 'Algorithm used for authentication',
          },
          authentication_method: {
            default: 'NTLM',
            canBeEmpty: false,
            description: 'Authentication method used',
          },
          domain: {
            default: '10.0.1.139',
            canBeEmpty: false,
            description: 'Domain name',
          },
          ldap_base_dn: {
            default: 'dc=next,dc=dev',
            canBeEmpty: false,
            description: 'LDAP base DN',
          },
          ldap_server: {
            default: 'ldap://10.0.1.139',
            canBeEmpty: false,
            description: 'LDAP server URL',
          },
          salt_hash: {
            default: 'b1865f9a4615cbcac2956f076a741841192dc65f4f6eebac66cc5ade440366c8',
            canBeEmpty: false,
            description: 'Salt hash for authentication',
          },
          scopes: {
            canBeEmpty: false,
            description: 'Access scopes',
          },
          secret_key: {
            default: 'e33228799f663a35c4be9343cecd991ffedc6d6a88e6c20463744bd6ade87108',
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
