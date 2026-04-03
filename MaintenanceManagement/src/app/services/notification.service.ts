import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';
import { AppNotification } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private readonly base = `${environment.apiUrl}/notifications`;
  private hubConnection: signalR.HubConnection | null = null;

  private notificationsSignal = signal<AppNotification[]>([]);

  /** Notifications from the last 5 days for the header dropdown. */
  readonly notifications = computed(() => {
    const cutoff = Date.now() - 5 * 24 * 60 * 60 * 1000;
    return this.notificationsSignal().filter(n => new Date(n.createdAt).getTime() >= cutoff);
  });

  /** All cached notifications (for the full-page view). */
  readonly allNotifications = computed(() => this.notificationsSignal());

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

  /** Load notifications. Pass days > 5 to fetch a longer history (for the full-page view). */
  loadAll(days = 5): Observable<AppNotification[]> {
    return this.http.get<AppNotification[]>(`${this.base}?days=${days}`).pipe(
      tap(data => this.notificationsSignal.set(data))
    );
  }

  /** Returns only the last 5 days of notifications – kept for backward compatibility. */
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

  /** Returns the Angular route path for the related entity of a notification. */
  getEntityRoute(relatedEntityType?: string): string | null {
    switch (relatedEntityType) {
      case 'TaskOrder':           return '/tasks';
      case 'MaintenanceSchedule': return '/maintenance-schedules';
      case 'SparePart':           return '/spare-parts';
      case 'Technician':          return '/technicians';
      case 'Equipment':           return '/equipment';
      case 'MaintenanceRequest':  return '/requests';
      default:                    return null;
    }
  }
}
