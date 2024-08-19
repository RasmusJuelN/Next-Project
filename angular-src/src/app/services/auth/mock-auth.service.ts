import { inject, Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ActiveQuestionnaire, Question, StudentTeacherAnswer, User } from '../../models/questionare';
import { LocalStorageService } from '../misc/local-storage.service';
import { JWTTokenService } from './jwt-token.service';

@Injectable({
  providedIn: 'root'
})
export class MockAuthService {
  private adminMockToken: string;
  private teacherMockToken: string;
  private localStorageService = inject(LocalStorageService);
  private jwtTokenService = inject(JWTTokenService);

  constructor() {
    // This token assumes that the user is "Max" and is a teacher
    this.teacherMockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZnVsbF9uYW1lIjoiTWF4Iiwic2NvcGUiOiJ0ZWFjaGVyIiwidXNlcm5hbWUiOiJNSiIsImV4cCI6MTYxNTE2MjY3MH0.LAlEc2_AYG1RuITP8a5LYdFCDj3j2FcEgZ6UT1C5OIM';
    this.adminMockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZnVsbF9uYW1lIjoiTWF4Iiwic2NvcGUiOiJhZG1pbiIsInVzZXJuYW1lIjoiTUoiLCJleHAiOjE2MTUxNjI2NzB9.KG-epxKAUF3zWIPvKNt_rlkiHFuN0sUPYrpGLe8_MFc'
  }

  /**
   * Authenticates the user login using mock data.
   * @param userName - The username of the user.
   * @param password - The password of the user.
   * @returns An Observable that emits an access token if the login is successful, or an error message if the login fails.
   */
  loginAuthentication(userName: string, password: string): Observable<{ access_token: string } | { error: string }> {
    const premadeUsers = [
      {userName: "Admin", password: "Pa$$w0rd" },
      { userName: "MJ", password: "Pa$$w0rd" }, // This user is a teacher
      { userName: "NH", password: "Pa$$w0rd" },
      { userName: "Alexander", password: "Pa$$w0rd" },
      { userName: "Johan", password: "Pa$$w0rd" }
    ];

    const matchedUser = premadeUsers.find(user => user.userName === userName && user.password === password);

    if (matchedUser) {
      const token = matchedUser.userName === 'Admin' ? this.adminMockToken : this.teacherMockToken;
      return of({ access_token: token }).pipe(
        tap(response => {
          console.log("Login success");
          this.jwtTokenService.setToken(response.access_token); // Use JWTTokenService to store the token
        })
      );
    } else {
      return throwError(() => new Error('Login failed. Please check your credentials.')).pipe(
        tap(() => {
          console.log("Login failure");
        })
      );
    }
  }

  /**
   * Checks if the current user has a specific role.
   * @param role - The role to check against.
   * @returns True if the user has the specified role, false otherwise.
   */
  hasRole(role: string): boolean {
    const userRole = this.getRole();
    return userRole === role;
  }

  /**
   * Checks if the user is logged in by verifying if the token is present.
   * @returns true if the user is logged in, otherwise false.
   */
  isLoggedIn(): boolean {
    return !!this.jwtTokenService.getDecodeToken();
  }

  /**
   * Checks if there is an active questionnaire for the user.
   * @returns An object containing `hasActive` (boolean) and `urlString` (string).
   */
  checkForActiveQuestionnaire(): { hasActive: boolean, urlString: string } {
    const role = this.getRole();
    const idString = this.getUserId();
    if (!idString) {
      return { hasActive: false, urlString: '' };
    }
    const id = Number(idString); // Convert string to number for now

    const mockData = this.localStorageService.getData('mockData');

    if (mockData) {
      const parsedData: {
        mockStudents: User[],
        mockTeachers: User[],
        mockQuestions: Question[],
        mockStudentTeacherAnswers: StudentTeacherAnswer[],
        mockActiveQuestionnaire: ActiveQuestionnaire[]
      } = JSON.parse(mockData);

      if (role === 'student') {
        const activeQuestionnaire = parsedData.mockActiveQuestionnaire.find((questionnaire: ActiveQuestionnaire) => questionnaire.student.id === id && !questionnaire.isStudentFinished);
        if (activeQuestionnaire) {
          return { hasActive: true, urlString: `${activeQuestionnaire.id}` };
        }
      } else if (role === 'teacher') {
        const activeQuestionnaire = parsedData.mockActiveQuestionnaire.find((questionnaire: ActiveQuestionnaire) => questionnaire.teacher.id === id && !questionnaire.isTeacherFinished);
        if (activeQuestionnaire) {
          return { hasActive: true, urlString: `${activeQuestionnaire.id}` };
        }
      }
    }
    return { hasActive: false, urlString: '' };
  }

  /**
   * Retrieves the user ID from the token stored in the JWTTokenService.
   * @returns The user ID if the token is valid, or null if the token is invalid or not found.
   */
  getUserId(): string | null {
    const decodedToken = this.jwtTokenService.getDecodeToken();
    return decodedToken ? decodedToken['sub'] : null; // Access with ['sub'] to avoid index signature issues
  }

  /**
   * Retrieves the role from the token stored in the JWTTokenService.
   * @returns The role if the token is valid, or null if the token is invalid or not found.
   */
  getRole(): string | null {
    const decodedToken = this.jwtTokenService.getDecodeToken();
    return decodedToken ? decodedToken['scope'] : null; // Access with ['scope'] to avoid index signature issues
  }
}
