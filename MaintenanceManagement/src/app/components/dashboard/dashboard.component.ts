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
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
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
