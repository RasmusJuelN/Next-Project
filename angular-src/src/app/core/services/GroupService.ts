// group.service.ts
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface Student {
  name: string;
}

@Injectable({ providedIn: 'root' })
export class GroupService {
  constructor(private http: HttpClient) {}

   getGroups(): Observable<{ name: string }[]> {
    return this.http.get<{ name: string }[]>(`https://localhost:7135/api/User/Groups`);
  }
  
  getStudents(groupName: string): Observable<Student[]> {
    return this.http.get<Student[]>(`https://localhost:7135/api/user/groups/${groupName}/students`);
  }
}
