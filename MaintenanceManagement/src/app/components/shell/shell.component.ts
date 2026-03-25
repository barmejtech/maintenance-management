import { Component, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translate.service';
import { ThemeService } from '../../services/theme.service';
import { NotificationsComponent } from '../notifications/notifications.component';
import { TranslatePipe } from '../../pipes/translate.pipe';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet, NotificationsComponent, TranslatePipe],
  templateUrl: './shell.component.html',
  styleUrls: ['./shell.component.css']
})
export class ShellComponent {
  sidebarOpen = signal(typeof window !== 'undefined' ? window.innerWidth > 768 : true);

  constructor(
    public auth: AuthService,
    public translation: TranslationService,
    public theme: ThemeService
  ) {}

  toggleSidebar(): void {
    this.sidebarOpen.update(v => !v);
  }

  @HostListener('window:resize')
  onResize(): void {
    const isDesktop = window.innerWidth > 768;
    if (isDesktop && !this.sidebarOpen()) {
      this.sidebarOpen.set(true);
    } else if (!isDesktop && this.sidebarOpen()) {
      this.sidebarOpen.set(false);
    }
  }

  isAdmin(): boolean { return this.auth.isAdmin(); }
  isManagerOrAdmin(): boolean { return this.auth.isManager(); }
  isTechnicianRole(): boolean { return !this.auth.isManager() && this.auth.isAuthenticated(); }

  getRoleName(): string {
    if (this.auth.isAdmin()) return this.translation.translate('dashboard.roles.administrator');
    if (this.auth.isManager()) return this.translation.translate('dashboard.roles.manager');
    return this.translation.translate('dashboard.roles.technician');
  }
}
