import { inject, Injectable } from '@angular/core';
import { Observable, of, throwError } from 'rxjs';
import { tap } from 'rxjs/operators';
import { jwtDecode } from 'jwt-decode';
import { ActiveQuestionnaire, Question, StudentTeacherAnswer, User } from '../../models/questionare';
import { LocalStorageService } from '../misc/local-storage.service';

@Injectable({
  providedIn: 'root'
})
export class MockAuthService {
  private mockToken: string;
  private localStorageService = inject(LocalStorageService)

  constructor() {
    // This token assumes that the user is "Max" and is a teacher
    this.mockToken = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZnVsbF9uYW1lIjoiTWF4Iiwic2NvcGUiOiJ0ZWFjaGVyIiwidXNlcm5hbWUiOiJNSiIsImV4cCI6MTYxNTE2MjY3MH0.LAlEc2_AYG1RuITP8a5LYdFCDj3j2FcEgZ6UT1C5OIM';
  }

  /**
   * Authenticates the user login using mock data.
   * @param userName - The username of the user.
   * @param password - The password of the user.
   * @returns An Observable that emits an access token if the login is successful, or an error message if the login fails.
   */
  loginAuthentication(userName: string, password: string): Observable<{ access_token: string } | { error: string }> {
    const premadeUsers = [
      { userName: "MJ", password: "Pa$$w0rd" }, // This user is a teacher
      { userName: "NH", password: "Pa$$w0rd" },
      { userName: "Alexander", password: "Pa$$w0rd" },
      { userName: "Johan", password: "Pa$$w0rd" }
    ];

    const matchedUser = premadeUsers.find(user => user.userName === userName && user.password === password);

    if (matchedUser) {
      return of({ access_token: this.mockToken }).pipe(
        tap(response => {
          console.log("Login success");
          localStorage.setItem('token', response.access_token);
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
   * Currently a simple implementation
   * @returns true if it contains a token
   */
  isLoggedIn(){
    return !!localStorage.getItem('token');
  }

  checkForActiveQuestionnaire(): { hasActive: boolean, urlString: string } {
    const role = this.getRole();
    const idString = this.getUserId();
    if (!idString) {
      return { hasActive: false, urlString: '' };
    }
    const id = Number(idString); // Convert string to number for now

    const mockData = localStorage.getItem('mockData');

    if (mockData) {
      const parsedData : {
        mockStudents: User[],
        mockTeachers: User[],
        mockQuestions: Question[],
        mockStudentTeacherAnswers: StudentTeacherAnswer[],
        mockActiveQuestionnaire: ActiveQuestionnaire[]
      } = JSON.parse(mockData);
      if (role === 'student') {
        const activeQuestionnaire = parsedData.mockActiveQuestionnaire.find((questionnaire:ActiveQuestionnaire) => questionnaire.student.id == id && !questionnaire.isStudentFinished);
        if (activeQuestionnaire) {
          return { hasActive: true, urlString: `${activeQuestionnaire.id}` };
        }
      } else if (role === 'teacher') {
        const activeQuestionnaire = parsedData.mockActiveQuestionnaire.find((questionnaire: ActiveQuestionnaire) => questionnaire.teacher.id == id && !questionnaire.isTeacherFinished);
        if (activeQuestionnaire) {
          return { hasActive: true, urlString: `${activeQuestionnaire.id}` };
        }
      }
    }
    return { hasActive: false, urlString: '' };
  }



  /**
   * Retrieves the user ID from the token stored in the local storage.
   * @returns The user ID if the token is valid, or null if the token is invalid or not found.
   */
  getUserId(): string | null {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = this.decodeToken(token);
        return decodedToken.sub || null;
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }

  /**
   * Retrieves the role from the token stored in the local storage.
   * @returns The role if the token is valid, or null if the token is invalid or not found.
   */
  getRole(): string | null {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken: any = this.decodeToken(token);
        return decodedToken.scope || null;
      } catch (error) {
        console.error('Invalid token', error);
        return null;
      }
    }
    return null;
  }

  getAuthToken(){
    return this.localStorageService.getToken();
  }

  decodeToken(token: string): any {
    return jwtDecode(token);
  }

  getUserFromToken(token: string):{ userId: number; role: string } | null {
    try {
      const decodedToken: any = this.decodeToken(token);
      const userId = decodedToken.sub;
      const role = decodedToken.scope;
      if (userId && role) {
        return { userId, role };
      }
      return null;
    } catch (error) {
      console.error('Invalid token', error);
      return null;
    }
  }
}