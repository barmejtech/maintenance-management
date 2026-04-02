import { Component, HostListener, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translate.service';
import { ThemeService } from '../../services/theme.service';
import { NotificationsComponent } from '../notifications/notifications.component';
import { NotificationService } from '../../services/notification.service';
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
    public theme: ThemeService,
    public notifService: NotificationService
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
  isDataEntryRole(): boolean { return this.auth.isDataEntry() && !this.auth.isAdmin() && !this.auth.hasRole('Manager'); }
  isTechnicianRole(): boolean { return this.auth.isTechnician(); }
  isClientRole(): boolean { return this.auth.isClient(); }
  isSupportRole(): boolean { return this.auth.isSupport(); }

  getRoleName(): string {
    if (this.auth.isAdmin()) return this.translation.translate('dashboard.roles.administrator');
    if (this.auth.hasRole('Manager')) return this.translation.translate('dashboard.roles.manager');
    if (this.auth.isDataEntry()) return this.translation.translate('dashboard.roles.dataEntry');
    if (this.auth.isSupport()) return 'Support';
    if (this.auth.isClient()) return 'Client';
    return this.translation.translate('dashboard.roles.technician');
  }
}
