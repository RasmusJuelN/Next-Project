// websocket.service.ts
import { Injectable } from '@angular/core';
import { webSocket, WebSocketSubject } from 'rxjs/webSocket';
import { Observable, Subject, timer } from 'rxjs';
import { retryWhen, switchMap, tap } from 'rxjs/operators';

/**
 * @deprecated This WebSocket service is no longer used.
 * It has been superseded by HTTP-based APIs and/or other real-time mechanisms.
 *
 * Old responsibilities:
 * - Opened a WebSocket connection.
 * - Retried on disconnect with exponential backoff.
 * - Exposed connection status as an observable.
 * - Supported optional heartbeat messages.
 */
@Injectable({
  providedIn: 'root',
})
export class WebsocketService {
  private wsUrl = 'ws://your-websocket-url'; // Replace with your WebSocket URL
  private socket$!: WebSocketSubject<any>;
  private reconnectDelay = 5000; // 5 seconds delay before reconnecting
  private connectionStatusSubject = new Subject<boolean>();
  public connectionStatus$ = this.connectionStatusSubject.asObservable();

  constructor() {
    this.connect();
  }

  private connect(): void {
    this.socket$ = webSocket(this.wsUrl);

    this.socket$
      .pipe(
        // When an error occurs, wait for reconnectDelay and then try to reconnect.
        retryWhen((errors) =>
          errors.pipe(
            tap((err) => {
              console.error('WebSocket error, reconnecting in 5 seconds', err);
              this.connectionStatusSubject.next(false);
            }),
            switchMap(() => timer(this.reconnectDelay))
          )
        )
      )
      .subscribe({
        next: (msg) => {
          // Handle incoming messages as needed.
          this.handleMessage(msg);
        },
        error: (err) => {
          console.error('WebSocket connection error', err);
        },
        complete: () => {
          console.warn('WebSocket connection closed');
          this.connectionStatusSubject.next(false);
        },
      });

    // Signal that the connection is (assumed) active.
    this.connectionStatusSubject.next(true);

    // Optionally, start a heartbeat if your server expects one:
    this.startHeartbeat();
  }

  private handleMessage(message: any): void {
    // Example: Handle heartbeat responses or other messages.
    console.log('Received message from WebSocket:', message);
  }

  /**
   * Optional heartbeat mechanism to keep the connection alive.
   */
  private startHeartbeat(): void {
    // You can adjust the interval (e.g., every 30 seconds)
    timer(0, 30000).subscribe(() => {
      this.sendMessage({ type: 'PING' });
    });
  }

  public sendMessage(message: any): void {
    if (this.socket$) {
      this.socket$.next(message);
    }
  }

  public close(): void {
    this.socket$?.complete();
    this.connectionStatusSubject.next(false);
  }
}
