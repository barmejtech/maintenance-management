import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../../services/notification.service';
import { AppNotification, NotificationType } from '../../models';

type FilterStatus = 'all' | 'unread' | 'read';
type FilterType   = 'all' | 'info' | 'success' | 'warning' | 'error';

@Component({
  selector: 'app-notifications-page',
  standalone: true,
  imports: [CommonModule, RouterModule, FormsModule],
  templateUrl: './notifications-page.component.html',
  styleUrls: ['./notifications-page.component.css']
})
export class NotificationsPageComponent implements OnInit {
  isLoading = signal(false);

  filterStatus = signal<FilterStatus>('all');
  filterType   = signal<FilterType>('all');
  searchQuery  = signal('');

  readonly filtered = computed(() => {
    const status = this.filterStatus();
    const type   = this.filterType();
    const query  = this.searchQuery().toLowerCase();

    return this.notifService.allNotifications().filter(n => {
      if (status === 'unread' && n.isRead)   return false;
      if (status === 'read'   && !n.isRead)  return false;

      const typeName = this.getTypeName(n.type);
      if (type !== 'all' && typeName !== type) return false;

      if (query && !n.title.toLowerCase().includes(query) && !n.message.toLowerCase().includes(query))
        return false;

      return true;
    });
  });

  readonly unreadFiltered = computed(() => this.filtered().filter(n => !n.isRead).length);

  constructor(public notifService: NotificationService) {}

  ngOnInit() {
    this.isLoading.set(true);
    // Fetch up to 30 days of notifications for the full-page view
    this.notifService.loadAll(30).subscribe({
      next: () => this.isLoading.set(false),
      error: () => this.isLoading.set(false)
    });
  }

  setFilterStatus(s: FilterStatus): void { this.filterStatus.set(s); }
  setFilterType(t: FilterType):     void { this.filterType.set(t); }
  onSearch(q: string):              void { this.searchQuery.set(q); }

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

  getTypeLabel(type: NotificationType): string {
    return ['Info', 'Success', 'Warning', 'Error'][type] ?? 'Info';
  }

  getEntityRoute(relatedEntityType?: string): string | null {
    return this.notifService.getEntityRoute(relatedEntityType);
  }

  getEntityLabel(relatedEntityType?: string): string {
    switch (relatedEntityType) {
      case 'TaskOrder':           return 'Work Order';
      case 'MaintenanceSchedule': return 'Schedule';
      case 'SparePart':           return 'Spare Part';
      case 'Technician':          return 'Technician';
      case 'Equipment':           return 'Equipment';
      default:                    return 'Details';
    }
  }

  timeAgo(dateStr: string): string {
    const diff = Date.now() - new Date(dateStr).getTime();
    const minutes = Math.floor(diff / 60000);
    if (minutes < 1)   return 'just now';
    if (minutes < 60)  return `${minutes}m ago`;
    const hours = Math.floor(minutes / 60);
    if (hours < 24)    return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    if (days < 7)      return `${days}d ago`;
    return new Date(dateStr).toLocaleDateString();
  }
}
