import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';
import { AppNotification } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  showSuccess(arg0: string) {
    throw new Error('Method not implemented.');
  }
  private readonly base = `${environment.apiUrl}/notifications`;
  private hubConnection: signalR.HubConnection | null = null;

  private notificationsSignal = signal<AppNotification[]>([]);
  readonly notifications = computed(() => {
    const cutoff = Date.now() - 5 * 24 * 60 * 60 * 1000;
    return this.notificationsSignal().filter(n => new Date(n.createdAt).getTime() >= cutoff);
  });
  readonly unreadCount = computed(() => {
    const cutoff = Date.now() - 5 * 24 * 60 * 60 * 1000;
    return this.notificationsSignal().filter(n => !n.isRead && new Date(n.createdAt).getTime() >= cutoff).length;
  });

  constructor(private http: HttpClient) {}

  startConnection(token: string): void {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubsUrl}/hubs/notifications`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveNotification', (notification: AppNotification) => {
      this.notificationsSignal.update(list => [notification, ...list]);
    });

    this.hubConnection.start().catch(err => console.error('Notification hub error:', err));
  }

  stopConnection(): void {
    this.hubConnection?.stop();
  }

  loadAll(): Observable<AppNotification[]> {
    return this.http.get<AppNotification[]>(this.base).pipe(
      tap(data => this.notificationsSignal.set(data))
    );
  }

  /** Returns only the last 5 days of notifications - same as notifications computed, kept for backward compatibility */
  get recentNotifications(): AppNotification[] {
    return this.notifications();
  }

  markAsRead(id: string): Observable<void> {
    return this.http.patch<void>(`${this.base}/${id}/read`, {}).pipe(
      tap(() => {
        this.notificationsSignal.update(list =>
          list.map(n => n.id === id ? { ...n, isRead: true } : n)
        );
      })
    );
  }

  markAllAsRead(): Observable<void> {
    return this.http.patch<void>(`${this.base}/read-all`, {}).pipe(
      tap(() => {
        this.notificationsSignal.update(list => list.map(n => ({ ...n, isRead: true })));
      })
    );
  }

  delete(id: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${id}`).pipe(
      tap(() => {
        this.notificationsSignal.update(list => list.filter(n => n.id !== id));
      })
    );
  }

  addLocal(notification: AppNotification): void {
    this.notificationsSignal.update(list => [notification, ...list]);
  }
}
