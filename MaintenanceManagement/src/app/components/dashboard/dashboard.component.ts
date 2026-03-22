import { Component, OnInit, OnDestroy, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TaskOrderService } from '../../services/task-order.service';
import { TechnicianService } from '../../services/technician.service';
import { EquipmentService } from '../../services/equipment.service';
import { NotificationService } from '../../services/notification.service';
import { NotificationsComponent } from '../notifications/notifications.component';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, NotificationsComponent],
  template: `
    <div class="dashboard">
      <header class="dash-header">
        <div class="dash-title">
          <span class="logo">🔧</span>
          <h1>Maintenance Management</h1>
        </div>
        <div class="dash-user">
          <app-notifications />
          <span>{{ auth.currentUser()?.firstName }} {{ auth.currentUser()?.lastName }}</span>
          <span class="role-badge">{{ auth.currentUser()?.roles?.[0] ?? 'User' }}</span>
          <button (click)="auth.logout()" class="btn-logout">Sign Out</button>
        </div>
      </header>

      <nav class="sidebar">
        <a routerLink="/dashboard" routerLinkActive="active" class="nav-item">
          <span>📊</span> Dashboard
        </a>
        <a routerLink="/tasks" routerLinkActive="active" class="nav-item">
          <span>✅</span> Work Orders
        </a>
        @if (isManagerOrAdmin()) {
          <a routerLink="/technicians" routerLinkActive="active" class="nav-item">
            <span>👷</span> Technicians
          </a>
          <a routerLink="/groups" routerLinkActive="active" class="nav-item">
            <span>👥</span> Groups
          </a>
        }
        <a routerLink="/equipment" routerLinkActive="active" class="nav-item">
          <span>🔩</span> Equipment
        </a>
        <a routerLink="/hvac" routerLinkActive="active" class="nav-item">
          <span>❄️</span> HVAC
        </a>
        <a routerLink="/reports" routerLinkActive="active" class="nav-item">
          <span>📄</span> Reports
        </a>
        @if (isManagerOrAdmin()) {
          <a routerLink="/invoices" routerLinkActive="active" class="nav-item">
            <span>💰</span> Invoices
          </a>
        }
        <a routerLink="/availability" routerLinkActive="active" class="nav-item">
          <span>📅</span> Availability
        </a>
        <a routerLink="/chat" routerLinkActive="active" class="nav-item">
          <span>💬</span> Team Chat
        </a>
        <div class="sidebar-divider"></div>
        @if (isAdmin()) {
          <div class="sidebar-section-label">Admin</div>
          <a routerLink="/technicians" routerLinkActive="active" class="nav-item admin-item">
            <span>⚙️</span> Manage Technicians
          </a>
        }
      </nav>

      <main class="main-content">
        <h2>Dashboard Overview</h2>
        <div class="stats-grid">
          <div class="stat-card">
            <div class="stat-icon">✅</div>
            <div class="stat-value">{{ taskCount() }}</div>
            <div class="stat-label">Active Work Orders</div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">👷</div>
            <div class="stat-value">{{ technicianCount() }}</div>
            <div class="stat-label">Technicians</div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">🔩</div>
            <div class="stat-value">{{ equipmentCount() }}</div>
            <div class="stat-label">Equipment</div>
          </div>
          <div class="stat-card">
            <div class="stat-icon">⚠️</div>
            <div class="stat-value">{{ dueMaintenance() }}</div>
            <div class="stat-label">Due Maintenance</div>
          </div>
        </div>

        <div class="quick-actions">
          <h3>Quick Actions</h3>
          <div class="actions-grid">
            <a routerLink="/tasks" class="action-btn">
              <span>➕</span> New Work Order
            </a>
            @if (isManagerOrAdmin()) {
              <a routerLink="/technicians" class="action-btn">
                <span>👷</span> Manage Technicians
              </a>
            }
            <a routerLink="/reports" class="action-btn">
              <span>📄</span> Create Report
            </a>
            @if (isManagerOrAdmin()) {
              <a routerLink="/invoices" class="action-btn">
                <span>💰</span> New Invoice
              </a>
            }
            <a routerLink="/chat" class="action-btn">
              <span>💬</span> Team Chat
            </a>
          </div>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .dashboard {
      display: grid;
      grid-template-areas:
        "header header"
        "sidebar main";
      grid-template-rows: 64px 1fr;
      grid-template-columns: 220px 1fr;
      min-height: 100vh;
      background: #f4f6f9;
    }
    .dash-header {
      grid-area: header;
      background: #0f3460;
      color: white;
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 2rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.2);
    }
    .dash-title { display: flex; align-items: center; gap: 0.75rem; }
    .dash-title h1 { font-size: 1.2rem; font-weight: 700; }
    .logo { font-size: 1.5rem; }
    .dash-user { display: flex; align-items: center; gap: 1rem; font-size: 0.9rem; }
    .role-badge {
      background: rgba(255,255,255,0.2);
      padding: 0.2rem 0.6rem;
      border-radius: 1rem;
      font-size: 0.75rem;
      font-weight: 600;
    }
    .btn-logout {
      background: rgba(255,255,255,0.15);
      color: white;
      border: 1px solid rgba(255,255,255,0.3);
      padding: 0.4rem 1rem;
      border-radius: 0.4rem;
      cursor: pointer;
      font-size: 0.85rem;
    }
    .sidebar {
      grid-area: sidebar;
      background: white;
      padding: 1.5rem 0;
      box-shadow: 2px 0 8px rgba(0,0,0,0.08);
    }
    .nav-item {
      display: flex;
      align-items: center;
      gap: 0.75rem;
      padding: 0.8rem 1.5rem;
      color: #555;
      text-decoration: none;
      font-size: 0.9rem;
      font-weight: 500;
      transition: all 0.2s;
    }
    .nav-item:hover, .nav-item.active {
      background: #f0f4ff;
      color: #0f3460;
      border-left: 3px solid #0f3460;
    }
    .sidebar-divider { height: 1px; background: #f0f0f0; margin: 0.75rem 0; }
    .sidebar-section-label {
      padding: 0.4rem 1.5rem;
      font-size: 0.7rem;
      font-weight: 700;
      text-transform: uppercase;
      letter-spacing: 0.08em;
      color: #aaa;
    }
    .admin-item { color: #7f5af0; }
    .admin-item:hover, .admin-item.active { background: #f5f0ff; color: #7f5af0; border-left-color: #7f5af0; }
    .main-content {
      grid-area: main;
      padding: 2rem;
    }
    .main-content h2 { color: #333; margin-bottom: 1.5rem; font-size: 1.5rem; }
    .stats-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(200px, 1fr));
      gap: 1.5rem;
      margin-bottom: 2rem;
    }
    .stat-card {
      background: white;
      padding: 1.5rem;
      border-radius: 0.75rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.06);
      text-align: center;
    }
    .stat-icon { font-size: 2rem; margin-bottom: 0.5rem; }
    .stat-value { font-size: 2rem; font-weight: 700; color: #0f3460; }
    .stat-label { color: #888; font-size: 0.85rem; margin-top: 0.25rem; }
    .quick-actions h3 { color: #333; margin-bottom: 1rem; }
    .actions-grid {
      display: grid;
      grid-template-columns: repeat(auto-fill, minmax(180px, 1fr));
      gap: 1rem;
    }
    .action-btn {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 1rem;
      background: white;
      border: 2px solid #e0e0e0;
      border-radius: 0.75rem;
      color: #333;
      text-decoration: none;
      font-weight: 500;
      font-size: 0.9rem;
      transition: all 0.2s;
    }
    .action-btn:hover { border-color: #0f3460; color: #0f3460; background: #f0f4ff; }
  `]
})
export class DashboardComponent implements OnInit, OnDestroy {
  taskCount = signal(0);
  technicianCount = signal(0);
  equipmentCount = signal(0);
  dueMaintenance = signal(0);

  constructor(
    public auth: AuthService,
    private taskService: TaskOrderService,
    private techService: TechnicianService,
    private eqService: EquipmentService,
    private notifService: NotificationService
  ) {}

  ngOnInit() {
    this.taskService.getAll().subscribe({ next: t => this.taskCount.set(t.filter(x => x.status < 2).length), error: () => {} });
    this.techService.getAll().subscribe({ next: t => this.technicianCount.set(t.length), error: () => {} });
    this.eqService.getAll().subscribe({ next: e => this.equipmentCount.set(e.length), error: () => {} });
    this.eqService.getDueMaintenance().subscribe({ next: e => this.dueMaintenance.set(e.length), error: () => {} });
    const token = this.auth.getAccessToken();
    if (token) {
      this.notifService.startConnection(token);
    }
  }

  ngOnDestroy() {
    this.notifService.stopConnection();
  }

  isAdmin(): boolean {
    return this.auth.isAdmin();
  }

  isManagerOrAdmin(): boolean {
    return this.auth.isManager();
  }
}
