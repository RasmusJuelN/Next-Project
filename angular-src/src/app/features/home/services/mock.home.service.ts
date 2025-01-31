import { Injectable } from '@angular/core';
import { Observable, of } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class MockHomeService {
  // Mock method to simulate active questionnaire check
  checkForExistingActiveQuestionnaires(userId: string): Observable<{ exists: boolean; id: string | null }> {
    if (userId === 'mockId12345') {
      return of({ exists: true, id: 'active1' });
    }
    return of({ exists: false, id: null });
  }
}
