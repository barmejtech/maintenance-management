import { Component, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { NotificationsComponent } from '../notifications/notifications.component';
import { NotificationService } from '../../services/notification.service';
import { ClientType } from '../../models';

@Component({
  selector: 'app-client-portal',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet, NotificationsComponent],
  template: `
    <div class="client-layout" [class.sidebar-collapsed]="!sidebarOpen()">
      <!-- Header -->
      <header class="client-header">
        <div class="header-left">
          <button class="btn-hamburger" (click)="toggleSidebar()">
            <i class="bi bi-list"></i>
          </button>
          <i class="bi bi-tools logo-icon"></i>
          <span class="brand-name">MaintenancePro</span>
        </div>
        <div class="header-right">
          <app-notifications />
          <span class="user-info">
            <i class="bi bi-person-circle"></i>
            {{ auth.currentUser()?.firstName }} {{ auth.currentUser()?.lastName }}
          </span>
          @if (auth.currentUser()?.companyName) {
            <span class="company-badge">
              <i class="bi bi-building"></i>
              {{ auth.currentUser()?.companyName }}
            </span>
          }
          <button class="btn-logout" (click)="auth.logout()">
            <i class="bi bi-box-arrow-right"></i> Logout
          </button>
        </div>
      </header>

      <!-- Sidebar -->
      <nav class="client-sidebar" [class.open]="sidebarOpen()">
        <div class="sidebar-brand">
          <i class="bi bi-tools"></i>
          <span>Client Portal</span>
        </div>

        <div class="nav-section">
          <span class="nav-section-label">Overview</span>
          <a routerLink="/client-portal/dashboard" routerLinkActive="active" class="nav-item">
            <i class="bi bi-speedometer2"></i>
            <span>Dashboard</span>
          </a>
        </div>

        <div class="nav-section">
          <span class="nav-section-label">Maintenance</span>
          <a routerLink="/client-portal/my-requests" routerLinkActive="active" class="nav-item">
            <i class="bi bi-clipboard-list"></i>
            <span>My Requests</span>
          </a>
          <a routerLink="/client-portal/new-request" routerLinkActive="active" class="nav-item">
            <i class="bi bi-plus-circle"></i>
            <span>New Request</span>
          </a>
        </div>

        <div class="nav-section">
          <span class="nav-section-label">Communication</span>
          <a routerLink="/client-portal/chat" routerLinkActive="active" class="nav-item">
            <i class="bi bi-chat-dots"></i>
            <span>Chat Support</span>
            @if (notifService.unreadCount() > 0) {
              <span class="nav-badge">{{ notifService.unreadCount() > 9 ? '9+' : notifService.unreadCount() }}</span>
            }
          </a>
          <a routerLink="/client-portal/notifications" routerLinkActive="active" class="nav-item">
            <i class="bi bi-bell"></i>
            <span>Notifications</span>
          </a>
        </div>

        <div class="nav-section">
          <span class="nav-section-label">Account</span>
          <a routerLink="/client-portal/profile" routerLinkActive="active" class="nav-item">
            <i class="bi bi-person"></i>
            <span>My Profile</span>
          </a>
        </div>
      </nav>

      <!-- Overlay for mobile -->
      <div class="sidebar-overlay" [class.visible]="sidebarOpen() && isMobile()" (click)="toggleSidebar()"></div>

      <!-- Main content -->
      <main class="client-main">
        <router-outlet />
      </main>
    </div>
  `,
  styles: [`
    .client-layout {
      display: flex;
      min-height: 100vh;
      background: #f5f7fa;
    }

    /* Header */
    .client-header {
      position: fixed;
      top: 0;
      left: 0;
      right: 0;
      height: 60px;
      background: white;
      box-shadow: 0 2px 8px rgba(0,0,0,0.08);
      display: flex;
      align-items: center;
      justify-content: space-between;
      padding: 0 20px;
      z-index: 100;
    }
    .header-left {
      display: flex;
      align-items: center;
      gap: 12px;
    }
    .btn-hamburger {
      background: none;
      border: none;
      font-size: 22px;
      cursor: pointer;
      color: #333;
      padding: 4px;
    }
    .logo-icon {
      font-size: 22px;
      color: #1565c0;
    }
    .brand-name {
      font-size: 1.1rem;
      font-weight: 700;
      color: #1a237e;
    }
    .header-right {
      display: flex;
      align-items: center;
      gap: 16px;
    }
    .user-info {
      display: flex;
      align-items: center;
      gap: 6px;
      font-weight: 500;
      color: #333;
    }
    .company-badge {
      display: flex;
      align-items: center;
      gap: 4px;
      background: #e3f2fd;
      color: #1565c0;
      padding: 4px 10px;
      border-radius: 20px;
      font-size: 0.85rem;
      font-weight: 500;
    }
    .btn-logout {
      display: flex;
      align-items: center;
      gap: 6px;
      background: none;
      border: 1.5px solid #ddd;
      border-radius: 8px;
      padding: 6px 14px;
      cursor: pointer;
      color: #666;
      font-size: 0.9rem;
      transition: all 0.2s;
    }
    .btn-logout:hover { border-color: #ef5350; color: #ef5350; }

    /* Sidebar */
    .client-sidebar {
      position: fixed;
      top: 60px;
      left: 0;
      width: 240px;
      height: calc(100vh - 60px);
      background: #1a237e;
      color: white;
      overflow-y: auto;
      transition: transform 0.3s;
      z-index: 90;
      padding: 20px 0;
    }
    .sidebar-brand {
      display: flex;
      align-items: center;
      gap: 10px;
      padding: 0 20px 20px;
      font-size: 1rem;
      font-weight: 600;
      color: #90caf9;
      border-bottom: 1px solid rgba(255,255,255,0.1);
    }
    .sidebar-brand i { font-size: 20px; }
    .nav-section { padding: 16px 0 4px; }
    .nav-section-label {
      display: block;
      padding: 0 20px 8px;
      font-size: 0.7rem;
      font-weight: 600;
      letter-spacing: 1px;
      text-transform: uppercase;
      color: rgba(255,255,255,0.4);
    }
    .nav-item {
      display: flex;
      align-items: center;
      gap: 12px;
      padding: 10px 20px;
      color: rgba(255,255,255,0.75);
      text-decoration: none;
      transition: all 0.2s;
      font-size: 0.9rem;
      position: relative;
    }
    .nav-item:hover { background: rgba(255,255,255,0.08); color: white; }
    .nav-item.active { background: rgba(255,255,255,0.12); color: white; }
    .nav-item.active::before {
      content: '';
      position: absolute;
      left: 0;
      top: 0;
      bottom: 0;
      width: 3px;
      background: #90caf9;
    }
    .nav-badge {
      margin-left: auto;
      background: #ef5350;
      color: white;
      font-size: 0.7rem;
      font-weight: 700;
      padding: 2px 6px;
      border-radius: 10px;
    }

    /* Main content */
    .client-main {
      flex: 1;
      margin-left: 240px;
      margin-top: 60px;
      padding: 24px;
      min-height: calc(100vh - 60px);
    }

    /* Overlay */
    .sidebar-overlay {
      display: none;
      position: fixed;
      inset: 0;
      background: rgba(0,0,0,0.4);
      z-index: 80;
    }
    .sidebar-overlay.visible { display: block; }

    /* Collapsed state */
    .client-layout.sidebar-collapsed .client-sidebar { transform: translateX(-100%); }
    .client-layout.sidebar-collapsed .client-main { margin-left: 0; }

    @media (max-width: 768px) {
      .client-sidebar { transform: translateX(-100%); }
      .client-sidebar.open { transform: translateX(0); }
      .client-main { margin-left: 0; }
      .company-badge { display: none; }
      .user-info span:not(.bi) { display: none; }
    }
  `]
})
export class ClientPortalComponent {
  sidebarOpen = signal(window.innerWidth > 768);

  constructor(
    public auth: AuthService,
    public notifService: NotificationService
  ) {}

  toggleSidebar(): void {
    this.sidebarOpen.update(v => !v);
  }

  isMobile(): boolean {
    return window.innerWidth <= 768;
  }

  @HostListener('window:resize')
  onResize(): void {
    const isDesktop = window.innerWidth > 768;
    if (isDesktop && !this.sidebarOpen()) {
      this.sidebarOpen.set(true);
    }
  }
}
