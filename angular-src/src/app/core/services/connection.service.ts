import { Injectable } from '@angular/core';
import { BehaviorSubject, fromEvent, merge, Observable, of } from 'rxjs';
import { mapTo, startWith } from 'rxjs/operators';

/**
 * @deprecated This service has been replaced by built-in connectivity checks inside `AuthService`.
 * Use `AuthService.isOnline$` instead.
 *
 * Old responsibility:
 * - Monitored `navigator.onLine` and browser online/offline events.
 * - Exposed an `isOnline$` observable with the connectivity status.
 */
@Injectable({
  providedIn: 'root',
})
export class ConnectionService {
  private isOnlineSubject = new BehaviorSubject<boolean>(navigator.onLine); // Initial state
  isOnline$ = this.isOnlineSubject.asObservable();

  constructor() {
    // Monitor online/offline events
    const online$ = fromEvent(window, 'online').pipe(mapTo(true));
    const offline$ = fromEvent(window, 'offline').pipe(mapTo(false));

    // Merge online and offline streams and update the BehaviorSubject
    merge(online$, offline$)
      .pipe(startWith(navigator.onLine)) // Start with the current status
      .subscribe((status) => this.isOnlineSubject.next(status));
  }
}
