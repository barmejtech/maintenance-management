import { Component, OnInit, OnDestroy, effect } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { AuthService } from './services/auth.service';
import { NotificationService } from './services/notification.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet],
  template: '<router-outlet />',
  styles: []
})
export class App implements OnInit, OnDestroy {
  constructor(
    private auth: AuthService,
    private notifService: NotificationService
  ) {
    // React to auth state changes to start/stop the SignalR connection
    effect(() => {
      const user = this.auth.currentUser();
      if (user) {
        const token = this.auth.getAccessToken();
        if (token) {
          this.notifService.startConnection(token);
          this.notifService.loadAll().subscribe();
        }
      } else {
        this.notifService.stopConnection();
      }
    });
  }

  ngOnInit(): void {}

  ngOnDestroy(): void {
    this.notifService.stopConnection();
  }
}
