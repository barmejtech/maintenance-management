import { Component, OnInit, signal, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';
import { AppNotification, NotificationType } from '../../models';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="notif-wrapper">
      <button class="notif-bell" (click)="togglePanel()" [attr.aria-label]="'Notifications: ' + notifService.unreadCount() + ' unread'">
        🔔
        @if (notifService.unreadCount() > 0) {
          <span class="notif-badge">{{ notifService.unreadCount() > 99 ? '99+' : notifService.unreadCount() }}</span>
        }
      </button>

      @if (isOpen()) {
        <div class="notif-panel" role="dialog" aria-label="Notifications">
          <div class="notif-header">
            <h3>Notifications</h3>
            <div class="notif-actions">
              @if (notifService.unreadCount() > 0) {
                <button class="btn-text" (click)="markAllRead()">Mark all read</button>
              }
            </div>
          </div>

          <div class="notif-list">
            @if (isLoading()) {
              <div class="notif-empty">Loading...</div>
            } @else if (notifService.notifications().length === 0) {
              <div class="notif-empty">No notifications</div>
            } @else {
              @for (n of notifService.notifications(); track n.id) {
                <div class="notif-item" [class.unread]="!n.isRead" [class]="'type-' + getTypeName(n.type)">
                  <div class="notif-icon">{{ getTypeIcon(n.type) }}</div>
                  <div class="notif-body">
                    <div class="notif-title">{{ n.title }}</div>
                    <div class="notif-msg">{{ n.message }}</div>
                    <div class="notif-time">{{ n.createdAt | date:'short' }}</div>
                  </div>
                  <div class="notif-item-actions">
                    @if (!n.isRead) {
                      <button class="btn-icon" (click)="markRead(n)" title="Mark as read">✓</button>
                    }
                    <button class="btn-icon btn-delete" (click)="deleteNotif(n)" title="Delete">✕</button>
                  </div>
                </div>
              }
            }
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .notif-wrapper { position: relative; display: inline-block; }
    .notif-bell {
      background: none;
      border: none;
      cursor: pointer;
      font-size: 1.4rem;
      position: relative;
      padding: 0.25rem;
      line-height: 1;
    }
    .notif-badge {
      position: absolute;
      top: -4px;
      right: -6px;
      background: #e74c3c;
      color: white;
      font-size: 0.65rem;
      font-weight: 700;
      min-width: 18px;
      height: 18px;
      border-radius: 9px;
      display: flex;
      align-items: center;
      justify-content: center;
      padding: 0 4px;
      line-height: 1;
    }
    .notif-panel {
      position: absolute;
      right: 0;
      top: calc(100% + 8px);
      width: 360px;
      background: white;
      border-radius: 0.75rem;
      box-shadow: 0 8px 32px rgba(0,0,0,0.18);
      z-index: 1000;
      overflow: hidden;
    }
    .notif-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 1rem 1.25rem;
      border-bottom: 1px solid #f0f0f0;
    }
    .notif-header h3 { font-size: 1rem; font-weight: 700; color: #333; margin: 0; }
    .btn-text {
      background: none;
      border: none;
      color: #0f3460;
      cursor: pointer;
      font-size: 0.8rem;
      font-weight: 500;
      padding: 0.25rem 0.5rem;
    }
    .btn-text:hover { text-decoration: underline; }
    .notif-list { max-height: 420px; overflow-y: auto; }
    .notif-empty { text-align: center; padding: 2rem; color: #aaa; font-size: 0.9rem; }
    .notif-item {
      display: flex;
      align-items: flex-start;
      gap: 0.75rem;
      padding: 0.875rem 1.25rem;
      border-bottom: 1px solid #f7f7f7;
      transition: background 0.15s;
    }
    .notif-item:hover { background: #f9f9f9; }
    .notif-item.unread { background: #f0f4ff; }
    .notif-item.unread:hover { background: #e8eeff; }
    .notif-icon { font-size: 1.2rem; flex-shrink: 0; margin-top: 2px; }
    .notif-body { flex: 1; min-width: 0; }
    .notif-title { font-size: 0.85rem; font-weight: 600; color: #333; margin-bottom: 0.2rem; }
    .notif-msg { font-size: 0.8rem; color: #666; margin-bottom: 0.25rem; white-space: pre-wrap; }
    .notif-time { font-size: 0.72rem; color: #aaa; }
    .notif-item-actions { display: flex; gap: 0.25rem; flex-shrink: 0; }
    .btn-icon {
      background: none;
      border: 1px solid #e0e0e0;
      border-radius: 0.3rem;
      cursor: pointer;
      font-size: 0.75rem;
      width: 24px;
      height: 24px;
      display: flex;
      align-items: center;
      justify-content: center;
      color: #555;
      transition: all 0.15s;
    }
    .btn-icon:hover { background: #f0f4ff; border-color: #0f3460; color: #0f3460; }
    .btn-delete:hover { background: #fdf0f0; border-color: #e74c3c; color: #e74c3c; }
  `]
})
export class NotificationsComponent implements OnInit {
  isOpen = signal(false);
  isLoading = signal(false);

  constructor(public notifService: NotificationService) {}

  ngOnInit() {
    this.isLoading.set(true);
    this.notifService.loadAll().subscribe({
      next: () => this.isLoading.set(false),
      error: () => this.isLoading.set(false)
    });
  }

  togglePanel(): void {
    this.isOpen.update(v => !v);
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    const target = event.target as HTMLElement;
    if (!target.closest('app-notifications')) {
      this.isOpen.set(false);
    }
  }

  markRead(n: AppNotification): void {
    this.notifService.markAsRead(n.id).subscribe();
  }

  markAllRead(): void {
    this.notifService.markAllAsRead().subscribe();
  }

  deleteNotif(n: AppNotification): void {
    this.notifService.delete(n.id).subscribe();
  }

  getTypeIcon(type: NotificationType): string {
    return ['ℹ️', '✅', '⚠️', '❌'][type] ?? 'ℹ️';
  }

  getTypeName(type: NotificationType): string {
    return ['info', 'success', 'warning', 'error'][type] ?? 'info';
  }
}
