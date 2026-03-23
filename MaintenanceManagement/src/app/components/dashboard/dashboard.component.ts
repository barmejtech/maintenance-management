import { Component, OnInit, OnDestroy, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TaskOrderService } from '../../services/task-order.service';
import { TechnicianService } from '../../services/technician.service';
import { EquipmentService } from '../../services/equipment.service';
import { NotificationService } from '../../services/notification.service';
import { NotificationsComponent } from '../notifications/notifications.component';
import { TaskStatus, EquipmentStatus, TechnicianStatus } from '../../models';
import { Chart, ArcElement, DoughnutController, BarController, BarElement, CategoryScale, LinearScale, Tooltip, Legend } from 'chart.js';

Chart.register(ArcElement, DoughnutController, BarController, BarElement, CategoryScale, LinearScale, Tooltip, Legend);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, NotificationsComponent],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('taskChart') taskChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('equipmentChart') equipmentChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('technicianChart') technicianChartRef!: ElementRef<HTMLCanvasElement>;

  taskCount = signal(0);
  technicianCount = signal(0);
  equipmentCount = signal(0);
  dueMaintenance = signal(0);

  private taskChart: Chart | null = null;
  private equipmentChart: Chart | null = null;
  private technicianChart: Chart | null = null;
  private refreshInterval: ReturnType<typeof setInterval> | null = null;

  // Raw data for charts
  private taskStats = { pending: 0, inProgress: 0, completed: 0, cancelled: 0, onHold: 0 };
  private equipmentStats = { operational: 0, maintenance: 0, outOfService: 0, decommissioned: 0 };
  private technicianStats = { available: 0, busy: 0, onLeave: 0, inactive: 0 };

  constructor(
    public auth: AuthService,
    private taskService: TaskOrderService,
    private techService: TechnicianService,
    private eqService: EquipmentService,
    private notifService: NotificationService
  ) {}

  ngOnInit() {
    this.loadData();
    // Refresh every 30 seconds for real-time feel
    this.refreshInterval = setInterval(() => this.loadData(), 30000);
  }

  ngAfterViewInit() {
    this.initCharts();
  }

  ngOnDestroy() {
    if (this.refreshInterval) clearInterval(this.refreshInterval);
    this.taskChart?.destroy();
    this.equipmentChart?.destroy();
    this.technicianChart?.destroy();
  }

  private loadData() {
    this.taskService.getAll().subscribe({
      next: tasks => {
        const active = tasks.filter(x => x.status < 2);
        this.taskCount.set(active.length);
        this.taskStats = {
          pending: tasks.filter(t => t.status === TaskStatus.Pending).length,
          inProgress: tasks.filter(t => t.status === TaskStatus.InProgress).length,
          completed: tasks.filter(t => t.status === TaskStatus.Completed).length,
          cancelled: tasks.filter(t => t.status === TaskStatus.Cancelled).length,
          onHold: tasks.filter(t => t.status === TaskStatus.OnHold).length
        };
        this.updateTaskChart();
      }, error: () => {}
    });
    this.techService.getAll().subscribe({
      next: techs => {
        this.technicianCount.set(techs.length);
        this.technicianStats = {
          available: techs.filter(t => t.status === TechnicianStatus.Available).length,
          busy: techs.filter(t => t.status === TechnicianStatus.Busy).length,
          onLeave: techs.filter(t => t.status === TechnicianStatus.OnLeave).length,
          inactive: techs.filter(t => t.status === TechnicianStatus.Inactive).length
        };
        this.updateTechnicianChart();
      }, error: () => {}
    });
    this.eqService.getAll().subscribe({
      next: eqs => {
        this.equipmentCount.set(eqs.length);
        this.equipmentStats = {
          operational: eqs.filter(e => e.status === EquipmentStatus.Operational).length,
          maintenance: eqs.filter(e => e.status === EquipmentStatus.UnderMaintenance).length,
          outOfService: eqs.filter(e => e.status === EquipmentStatus.OutOfService).length,
          decommissioned: eqs.filter(e => e.status === EquipmentStatus.Decommissioned).length
        };
        this.updateEquipmentChart();
      }, error: () => {}
    });
    this.eqService.getDueMaintenance().subscribe({ next: e => this.dueMaintenance.set(e.length), error: () => {} });
  }

  private initCharts() {
    if (this.taskChartRef) {
      this.taskChart = new Chart(this.taskChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'],
          datasets: [{
            data: [0, 0, 0, 0, 0],
            backgroundColor: ['#f39c12', '#3498db', '#27ae60', '#e74c3c', '#95a5a6'],
            borderWidth: 2,
            borderColor: '#fff'
          }]
        },
        options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
      });
    }
    if (this.equipmentChartRef) {
      this.equipmentChart = new Chart(this.equipmentChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Operational', 'Under Maintenance', 'Out of Service', 'Decommissioned'],
          datasets: [{
            data: [0, 0, 0, 0],
            backgroundColor: ['#27ae60', '#f39c12', '#e74c3c', '#95a5a6'],
            borderWidth: 2,
            borderColor: '#fff'
          }]
        },
        options: { responsive: true, maintainAspectRatio: false, plugins: { legend: { position: 'bottom' } } }
      });
    }
    if (this.technicianChartRef) {
      this.technicianChart = new Chart(this.technicianChartRef.nativeElement, {
        type: 'bar',
        data: {
          labels: ['Available', 'Busy', 'On Leave', 'Inactive'],
          datasets: [{
            label: 'Technicians',
            data: [0, 0, 0, 0],
            backgroundColor: ['#27ae60', '#3498db', '#f39c12', '#95a5a6'],
            borderRadius: 6
          }]
        },
        options: {
          responsive: true,
          maintainAspectRatio: false,
          plugins: { legend: { display: false } },
          scales: { y: { beginAtZero: true, ticks: { stepSize: 1 } } }
        }
      });
    }
  }

  private updateTaskChart() {
    if (!this.taskChart) return;
    const { pending, inProgress, completed, cancelled, onHold } = this.taskStats;
    this.taskChart.data.datasets[0].data = [pending, inProgress, completed, cancelled, onHold];
    this.taskChart.update();
  }

  private updateEquipmentChart() {
    if (!this.equipmentChart) return;
    const { operational, maintenance, outOfService, decommissioned } = this.equipmentStats;
    this.equipmentChart.data.datasets[0].data = [operational, maintenance, outOfService, decommissioned];
    this.equipmentChart.update();
  }

  private updateTechnicianChart() {
    if (!this.technicianChart) return;
    const { available, busy, onLeave, inactive } = this.technicianStats;
    this.technicianChart.data.datasets[0].data = [available, busy, onLeave, inactive];
    this.technicianChart.update();
  }

  isAdmin(): boolean {
    return this.auth.isAdmin();
  }

  isManagerOrAdmin(): boolean {
    return this.auth.isManager();
  }
}
