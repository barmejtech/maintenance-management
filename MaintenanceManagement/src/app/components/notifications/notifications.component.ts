import { Component, OnInit, signal, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService } from '../../services/notification.service';
import { AppNotification, NotificationType } from '../../models';

@Component({
  selector: 'app-notifications',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notifications.component.html',
  styleUrls: ['./notifications.component.css']
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
