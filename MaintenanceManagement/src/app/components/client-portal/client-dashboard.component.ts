import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { NotificationService } from '../../services/notification.service';
import { environment } from '../../../environments/environment';
import { MaintenanceRequestStatus, AppNotification, NotificationType } from '../../models';

interface ClientDashboardStats {
  totalRequests: number;
  pendingRequests: number;
  underReviewRequests: number;
  approvedRequests: number;
  inProgressRequests: number;
  completedRequests: number;
  rejectedRequests: number;
  recentRequests: Array<{
    id: string;
    title: string;
    status: MaintenanceRequestStatus;
    requestDate: string;
    createdAt: string;
  }>;
  clientName: string;
  companyName?: string;
}

@Component({
  selector: 'app-client-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="client-dashboard">
      <!-- Welcome Section -->
      <div class="welcome-section">
        <div class="welcome-text">
          <h1>Welcome back, {{ auth.currentUser()?.firstName }}! 👋</h1>
          @if (auth.currentUser()?.companyName) {
            <p class="company-name"><i class="bi bi-building"></i> {{ auth.currentUser()?.companyName }}</p>
          }
          <p class="subtitle">Track your maintenance requests and stay updated</p>
        </div>
        <a routerLink="/client-portal/new-request" class="btn-new-request">
          <i class="bi bi-plus-circle"></i> New Request
        </a>
      </div>

      @if (isLoading()) {
        <div class="loading-state">
          <div class="spinner"></div>
          <p>Loading your dashboard...</p>
        </div>
      } @else {
        <!-- Stats Cards -->
        <div class="stats-grid">
          <div class="stat-card total">
            <div class="stat-icon"><i class="bi bi-clipboard-list"></i></div>
            <div class="stat-info">
              <span class="stat-value">{{ stats()?.totalRequests ?? 0 }}</span>
              <span class="stat-label">Total Requests</span>
            </div>
          </div>
          <div class="stat-card pending">
            <div class="stat-icon"><i class="bi bi-clock"></i></div>
            <div class="stat-info">
              <span class="stat-value">{{ stats()?.pendingRequests ?? 0 }}</span>
              <span class="stat-label">Pending</span>
            </div>
          </div>
          <div class="stat-card in-progress">
            <div class="stat-icon"><i class="bi bi-gear-wide-connected"></i></div>
            <div class="stat-info">
              <span class="stat-value">{{ (stats()?.inProgressRequests ?? 0) + (stats()?.approvedRequests ?? 0) + (stats()?.underReviewRequests ?? 0) }}</span>
              <span class="stat-label">In Progress</span>
            </div>
          </div>
          <div class="stat-card completed">
            <div class="stat-icon"><i class="bi bi-check-circle"></i></div>
            <div class="stat-info">
              <span class="stat-value">{{ stats()?.completedRequests ?? 0 }}</span>
              <span class="stat-label">Completed</span>
            </div>
          </div>
        </div>

        <!-- Recent Requests -->
        <div class="section-card">
          <div class="section-header">
            <h2>Recent Requests</h2>
            <a routerLink="/client-portal/my-requests" class="view-all-link">View All</a>
          </div>
          @if (stats()?.recentRequests?.length) {
            <div class="requests-table">
              <table>
                <thead>
                  <tr>
                    <th>Title</th>
                    <th>Date</th>
                    <th>Status</th>
                  </tr>
                </thead>
                <tbody>
                  @for (req of stats()?.recentRequests; track req.id) {
                    <tr>
                      <td class="req-title">{{ req.title }}</td>
                      <td class="req-date">{{ req.requestDate | date:'MMM d, y' }}</td>
                      <td><span class="status-badge" [class]="getStatusClass(req.status)">{{ getStatusLabel(req.status) }}</span></td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          } @else {
            <div class="empty-state">
              <i class="bi bi-inbox"></i>
              <p>No maintenance requests yet</p>
              <a routerLink="/client-portal/new-request" class="btn-create-first">Create your first request</a>
            </div>
          }
        </div>

        <!-- Recent Notifications -->
        <div class="section-card">
          <div class="section-header">
            <h2>Recent Notifications</h2>
            <a routerLink="/client-portal/notifications" class="view-all-link">View All</a>
          </div>
          @if (recentNotifications().length) {
            <div class="notif-list">
              @for (n of recentNotifications(); track n.id) {
                <div class="notif-item" [class.unread]="!n.isRead">
                  <span class="notif-icon">{{ getNotifIcon(n.type) }}</span>
                  <div class="notif-body">
                    <span class="notif-title">{{ n.title }}</span>
                    <span class="notif-msg">{{ n.message }}</span>
                  </div>
                  <span class="notif-time">{{ timeAgo(n.createdAt) }}</span>
                </div>
              }
            </div>
          } @else {
            <div class="empty-state">
              <i class="bi bi-bell-slash"></i>
              <p>No recent notifications</p>
            </div>
          }
        </div>

        <!-- Quick Actions -->
        <div class="quick-actions">
          <h2>Quick Actions</h2>
          <div class="actions-grid">
            <a routerLink="/client-portal/new-request" class="action-card">
              <i class="bi bi-plus-circle-fill"></i>
              <span>Submit New Request</span>
            </a>
            <a routerLink="/client-portal/my-requests" class="action-card">
              <i class="bi bi-list-check"></i>
              <span>View All Requests</span>
            </a>
            <a routerLink="/client-portal/chat" class="action-card">
              <i class="bi bi-chat-dots-fill"></i>
              <span>Chat with Support</span>
            </a>
            <a routerLink="/client-portal/profile" class="action-card">
              <i class="bi bi-person-gear"></i>
              <span>Update Profile</span>
            </a>
          </div>
        </div>
      }
    </div>
  `,
  styles: [`
    .client-dashboard { max-width: 1100px; margin: 0 auto; }

    .welcome-section {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 28px;
      flex-wrap: wrap;
      gap: 16px;
    }
    .welcome-text h1 { font-size: 1.6rem; font-weight: 700; color: #1a237e; margin: 0 0 4px; }
    .company-name { color: #1565c0; font-weight: 500; display: flex; align-items: center; gap: 6px; margin: 0 0 4px; }
    .subtitle { color: #666; margin: 0; }
    .btn-new-request {
      display: flex;
      align-items: center;
      gap: 8px;
      background: linear-gradient(135deg, #1565c0, #1a237e);
      color: white;
      padding: 12px 24px;
      border-radius: 10px;
      text-decoration: none;
      font-weight: 600;
      transition: opacity 0.2s;
    }
    .btn-new-request:hover { opacity: 0.9; }

    .loading-state { display: flex; flex-direction: column; align-items: center; padding: 60px; color: #666; gap: 16px; }
    .spinner { width: 40px; height: 40px; border: 3px solid #e3f2fd; border-top-color: #1565c0; border-radius: 50%; animation: spin 0.8s linear infinite; }
    @keyframes spin { to { transform: rotate(360deg); } }

    .stats-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; margin-bottom: 28px; }
    .stat-card {
      background: white;
      border-radius: 14px;
      padding: 20px;
      display: flex;
      align-items: center;
      gap: 16px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.05);
      border-left: 4px solid transparent;
    }
    .stat-card.total { border-left-color: #1565c0; }
    .stat-card.pending { border-left-color: #f57f17; }
    .stat-card.in-progress { border-left-color: #00838f; }
    .stat-card.completed { border-left-color: #2e7d32; }
    .stat-icon { font-size: 28px; }
    .stat-card.total .stat-icon { color: #1565c0; }
    .stat-card.pending .stat-icon { color: #f57f17; }
    .stat-card.in-progress .stat-icon { color: #00838f; }
    .stat-card.completed .stat-icon { color: #2e7d32; }
    .stat-value { display: block; font-size: 2rem; font-weight: 700; color: #1a237e; line-height: 1; }
    .stat-label { font-size: 0.85rem; color: #666; margin-top: 4px; display: block; }

    .section-card {
      background: white;
      border-radius: 14px;
      padding: 24px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.05);
      margin-bottom: 28px;
    }
    .section-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 20px; }
    .section-header h2 { font-size: 1.1rem; font-weight: 600; color: #1a237e; margin: 0; }
    .view-all-link { color: #1565c0; text-decoration: none; font-size: 0.9rem; font-weight: 500; }

    .requests-table table { width: 100%; border-collapse: collapse; }
    .requests-table th { text-align: left; padding: 10px 14px; font-size: 0.8rem; font-weight: 600; color: #666; text-transform: uppercase; border-bottom: 1px solid #f0f0f0; }
    .requests-table td { padding: 12px 14px; border-bottom: 1px solid #f9f9f9; }
    .req-title { font-weight: 500; color: #333; }
    .req-date { color: #888; font-size: 0.9rem; }

    .status-badge { padding: 4px 10px; border-radius: 20px; font-size: 0.78rem; font-weight: 600; }
    .status-pending { background: #fff3e0; color: #e65100; }
    .status-review { background: #e3f2fd; color: #1565c0; }
    .status-approved { background: #e8f5e9; color: #2e7d32; }
    .status-progress { background: #e0f7fa; color: #00695c; }
    .status-completed { background: #e8f5e9; color: #1b5e20; }
    .status-rejected { background: #fce4ec; color: #c62828; }

    .empty-state { display: flex; flex-direction: column; align-items: center; padding: 40px; color: #999; gap: 12px; }
    .empty-state i { font-size: 48px; }
    .btn-create-first { background: #e3f2fd; color: #1565c0; padding: 10px 20px; border-radius: 8px; text-decoration: none; font-weight: 500; }

    .notif-list { display: flex; flex-direction: column; gap: 2px; }
    .notif-item {
      display: flex;
      align-items: flex-start;
      gap: 12px;
      padding: 12px;
      border-radius: 8px;
      border-bottom: 1px solid #f5f5f5;
    }
    .notif-item.unread { background: #f0f7ff; }
    .notif-icon { font-size: 18px; flex-shrink: 0; margin-top: 2px; }
    .notif-body { flex: 1; display: flex; flex-direction: column; gap: 2px; overflow: hidden; }
    .notif-title { font-weight: 600; color: #1a237e; font-size: 0.9rem; }
    .notif-msg { color: #555; font-size: 0.85rem; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }
    .notif-time { color: #aaa; font-size: 0.78rem; white-space: nowrap; flex-shrink: 0; }

    .quick-actions h2 { font-size: 1.1rem; font-weight: 600; color: #1a237e; margin: 0 0 16px; }
    .actions-grid { display: grid; grid-template-columns: repeat(4, 1fr); gap: 16px; }
    .action-card {
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 10px;
      background: white;
      border-radius: 12px;
      padding: 24px 16px;
      text-decoration: none;
      color: #1a237e;
      box-shadow: 0 2px 10px rgba(0,0,0,0.05);
      transition: transform 0.2s, box-shadow 0.2s;
      font-weight: 500;
      text-align: center;
    }
    .action-card i { font-size: 28px; color: #1565c0; }
    .action-card:hover { transform: translateY(-2px); box-shadow: 0 6px 20px rgba(0,0,0,0.1); }

    @media (max-width: 900px) { .stats-grid { grid-template-columns: repeat(2, 1fr); } .actions-grid { grid-template-columns: repeat(2, 1fr); } }
    @media (max-width: 600px) { .stats-grid { grid-template-columns: 1fr; } .actions-grid { grid-template-columns: 1fr 1fr; } }
  `]
})
export class ClientDashboardComponent implements OnInit {
  stats = signal<ClientDashboardStats | null>(null);
  isLoading = signal(true);

  constructor(
    private http: HttpClient,
    public auth: AuthService,
    public notifService: NotificationService
  ) {}

  ngOnInit(): void {
    this.http.get<ClientDashboardStats>(`${environment.apiUrl}/dashboard`).subscribe({
      next: (data) => { this.stats.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
    // Load recent notifications if not already loaded (fetch 5 days to match the preview count)
    if (this.notifService.allNotifications().length === 0) {
      this.notifService.loadAll(5).subscribe();
    }
  }

  /** Returns the 5 most recent notifications for the dashboard preview. */
  recentNotifications(): AppNotification[] {
    return this.notifService.allNotifications().slice(0, 5);
  }

  getNotifIcon(type: NotificationType): string {
    const icons: Record<number, string> = {
      [NotificationType.Info]:    'ℹ️',
      [NotificationType.Success]: '✅',
      [NotificationType.Warning]: '⚠️',
      [NotificationType.Error]:   '❌'
    };
    return icons[type] ?? 'ℹ️';
  }

  timeAgo(dateStr: string): string {
    const diff = Date.now() - new Date(dateStr).getTime();
    const minutes = Math.floor(diff / 60000);
    if (minutes < 1)  return 'just now';
    if (minutes < 60) return `${minutes}m ago`;
    const hours = Math.floor(minutes / 60);
    if (hours < 24)   return `${hours}h ago`;
    const days = Math.floor(hours / 24);
    return days < 7 ? `${days}d ago` : new Date(dateStr).toLocaleDateString();
  }

  getStatusClass(status: MaintenanceRequestStatus): string {
    const map: Record<number, string> = {
      0: 'status-pending',
      1: 'status-review',
      2: 'status-approved',
      3: 'status-progress',
      4: 'status-completed',
      5: 'status-rejected'
    };
    return map[status] ?? 'status-pending';
  }

  getStatusLabel(status: MaintenanceRequestStatus): string {
    const map: Record<number, string> = {
      0: 'Pending',
      1: 'Under Review',
      2: 'Approved',
      3: 'In Progress',
      4: 'Completed',
      5: 'Rejected',
      6: 'Cancelled'
    };
    return map[status] ?? 'Unknown';
  }
}
