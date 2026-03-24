import { Component, OnInit, OnDestroy, signal, AfterViewInit, ElementRef, ViewChild, HostListener } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TaskOrderService } from '../../services/task-order.service';
import { TechnicianService } from '../../services/technician.service';
import { EquipmentService } from '../../services/equipment.service';
import { NotificationService } from '../../services/notification.service';
import { InvoiceService } from '../../services/invoice.service';
import { ReportService } from '../../services/report.service';
import { SparePartService } from '../../services/spare-part.service';
import { MaintenanceScheduleService } from '../../services/maintenance-schedule.service';
import { NotificationsComponent } from '../notifications/notifications.component';
import { TranslationService } from '../../services/translate.service';
import { ThemeService } from '../../services/theme.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TaskStatus, EquipmentStatus, TechnicianStatus, TaskPriority, InvoiceStatus, TaskOrder } from '../../models';
import {
  Chart, ArcElement, DoughnutController, BarController, BarElement,
  CategoryScale, LinearScale, Tooltip, Legend,
  LineController, LineElement, PointElement, Filler
} from 'chart.js';

Chart.register(
  ArcElement, DoughnutController, BarController, BarElement,
  CategoryScale, LinearScale, Tooltip, Legend,
  LineController, LineElement, PointElement, Filler
);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, NotificationsComponent, TranslatePipe],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  // Admin / Manager chart refs
  @ViewChild('taskChart') taskChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('equipmentChart') equipmentChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('technicianChart') technicianChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('trendChart') trendChartRef!: ElementRef<HTMLCanvasElement>;
  // Technician chart refs
  @ViewChild('myTaskChart') myTaskChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('myPriorityChart') myPriorityChartRef!: ElementRef<HTMLCanvasElement>;

  // Sidebar visibility: open by default on desktop, closed on mobile
  sidebarOpen = signal(typeof window !== 'undefined' ? window.innerWidth > 768 : true);

  toggleSidebar(): void {
    this.sidebarOpen.update(v => !v);
  }

  @HostListener('window:resize')
  onResize(): void {
    const isDesktop = window.innerWidth > 768;
    // Auto-open on desktop, auto-close on mobile when crossing the breakpoint
    if (isDesktop && !this.sidebarOpen()) {
      this.sidebarOpen.set(true);
    } else if (!isDesktop && this.sidebarOpen()) {
      this.sidebarOpen.set(false);
    }
  }

  // Period filter: 'today' | '7d' | '30d' | 'all'
  selectedPeriod = signal<'today' | '7d' | '30d' | 'all'>('all');
  lastRefreshed = signal<Date>(new Date());
  isRefreshing = signal(false);

  // Shared signals
  taskCount = signal(0);
  equipmentCount = signal(0);
  dueMaintenance = signal(0);
  // Admin / Manager signals
  technicianCount = signal(0);
  invoiceCount = signal(0);
  pendingInvoices = signal(0);
  reportCount = signal(0);
  sparePartsCount = signal(0);
  lowStockCount = signal(0);
  activeSchedulesCount = signal(0);
  overdueSchedulesCount = signal(0);
  // Technician signals
  myTaskCount = signal(0);
  myPendingCount = signal(0);
  myInProgressCount = signal(0);
  myCompletedCount = signal(0);

  private taskChart: Chart | null = null;
  private equipmentChart: Chart | null = null;
  private technicianChart: Chart | null = null;
  private trendChart: Chart | null = null;
  private myTaskChart: Chart | null = null;
  private myPriorityChart: Chart | null = null;
  private refreshInterval: ReturnType<typeof setInterval> | null = null;

  private taskStats = { pending: 0, inProgress: 0, completed: 0, cancelled: 0, onHold: 0 };
  private equipmentStats = { operational: 0, maintenance: 0, outOfService: 0, decommissioned: 0 };
  private technicianStats = { available: 0, busy: 0, onLeave: 0, inactive: 0 };
  private myTaskStats = { pending: 0, inProgress: 0, completed: 0, cancelled: 0, onHold: 0 };
  private myPriorityStats = { low: 0, medium: 0, high: 0, critical: 0 };
  trendLabels: string[] = [];
  private trendCounts: number[] = [];

  // Raw task list for period-filtering
  private allTasks: TaskOrder[] = [];

  constructor(
    public auth: AuthService,
    public translation: TranslationService,
    public theme: ThemeService,
    private taskService: TaskOrderService,
    private techService: TechnicianService,
    private eqService: EquipmentService,
    private notifService: NotificationService,
    private invoiceService: InvoiceService,
    private reportService: ReportService,
    private sparePartService: SparePartService,
    private scheduleService: MaintenanceScheduleService
  ) {}

  ngOnInit() {
    this.trendLabels = this.buildTrendLabels();
    this.loadData();
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
    this.trendChart?.destroy();
    this.myTaskChart?.destroy();
    this.myPriorityChart?.destroy();
  }

  setPeriod(period: 'today' | '7d' | '30d' | 'all') {
    this.selectedPeriod.set(period);
    this.applyTaskFilter();
  }

  private filterTasksByPeriod(tasks: TaskOrder[]): TaskOrder[] {
    const period = this.selectedPeriod();
    if (period === 'all') return tasks;
    const now = new Date();
    const cutoff = new Date(now);
    if (period === 'today') {
      cutoff.setHours(0, 0, 0, 0);
    } else if (period === '7d') {
      cutoff.setDate(now.getDate() - 7);
    } else if (period === '30d') {
      cutoff.setDate(now.getDate() - 30);
    }
    return tasks.filter(t => new Date(t.createdAt) >= cutoff);
  }

  private applyTaskFilter() {
    const filtered = this.filterTasksByPeriod(this.allTasks);
    const active = filtered.filter(x => x.status < 2);
    this.taskCount.set(active.length);
    this.taskStats = {
      pending: filtered.filter(t => t.status === TaskStatus.Pending).length,
      inProgress: filtered.filter(t => t.status === TaskStatus.InProgress).length,
      completed: filtered.filter(t => t.status === TaskStatus.Completed).length,
      cancelled: filtered.filter(t => t.status === TaskStatus.Cancelled).length,
      onHold: filtered.filter(t => t.status === TaskStatus.OnHold).length
    };
    this.trendCounts = this.buildTrendCounts(filtered);
    this.updateTaskChart();
    this.updateTrendChart();
    if (this.isTechnicianRole()) {
      this.computeMyTaskStats(filtered);
    }
  }

  private loadData() {
    this.isRefreshing.set(true);
    this.taskService.getAll().subscribe({
      next: tasks => {
        this.allTasks = tasks;
        this.applyTaskFilter();
        this.isRefreshing.set(false);
        this.lastRefreshed.set(new Date());
      }, error: () => { this.isRefreshing.set(false); }
    });

    if (this.isManagerOrAdmin()) {
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
      this.invoiceService.getAll().subscribe({
        next: invoices => {
          this.invoiceCount.set(invoices.length);
          this.pendingInvoices.set(
            invoices.filter(i => i.status === InvoiceStatus.Sent || i.status === InvoiceStatus.Overdue).length
          );
        }, error: () => {}
      });
      this.reportService.getAll().subscribe({
        next: reports => this.reportCount.set(reports.length),
        error: () => {}
      });
      this.sparePartService.getAll().subscribe({
        next: parts => {
          this.sparePartsCount.set(parts.length);
          this.lowStockCount.set(parts.filter(p => p.isLowStock).length);
        }, error: () => {}
      });
      this.scheduleService.getAll().subscribe({
        next: schedules => {
          const now = new Date();
          this.activeSchedulesCount.set(schedules.filter(s => s.isActive).length);
          this.overdueSchedulesCount.set(
            schedules.filter(s => s.isActive && s.nextDueAt && new Date(s.nextDueAt) < now).length
          );
        }, error: () => {}
      });
    }

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

  private computeMyTaskStats(tasks: TaskOrder[]) {
    const userId = this.auth.currentUser()?.userId;
    const myTasks = userId ? tasks.filter(t => t.createdByUserId === userId) : tasks;
    this.myTaskCount.set(myTasks.length);
    this.myPendingCount.set(myTasks.filter(t => t.status === TaskStatus.Pending).length);
    this.myInProgressCount.set(myTasks.filter(t => t.status === TaskStatus.InProgress).length);
    this.myCompletedCount.set(myTasks.filter(t => t.status === TaskStatus.Completed).length);
    this.myTaskStats = {
      pending: myTasks.filter(t => t.status === TaskStatus.Pending).length,
      inProgress: myTasks.filter(t => t.status === TaskStatus.InProgress).length,
      completed: myTasks.filter(t => t.status === TaskStatus.Completed).length,
      cancelled: myTasks.filter(t => t.status === TaskStatus.Cancelled).length,
      onHold: myTasks.filter(t => t.status === TaskStatus.OnHold).length
    };
    this.myPriorityStats = {
      low: myTasks.filter(t => t.priority === TaskPriority.Low).length,
      medium: myTasks.filter(t => t.priority === TaskPriority.Medium).length,
      high: myTasks.filter(t => t.priority === TaskPriority.High).length,
      critical: myTasks.filter(t => t.priority === TaskPriority.Critical).length
    };
    this.updateMyTaskChart();
    this.updateMyPriorityChart();
  }

  private buildTrendLabels(): string[] {
    const labels: string[] = [];
    for (let i = 6; i >= 0; i--) {
      const d = new Date();
      d.setDate(d.getDate() - i);
      labels.push(d.toLocaleDateString('en-US', { weekday: 'short' }));
    }
    return labels;
  }

  private buildTrendCounts(tasks: TaskOrder[]): number[] {
    const counts = new Array(7).fill(0);
    const now = new Date();
    tasks.forEach(task => {
      const created = new Date(task.createdAt);
      const diff = Math.floor((now.getTime() - created.getTime()) / (1000 * 60 * 60 * 24));
      if (diff >= 0 && diff < 7) {
        counts[6 - diff]++;
      }
    });
    return counts;
  }

  private initCharts() {
    const baseOpts = { responsive: true, maintainAspectRatio: false };
    const legendBottom = { position: 'bottom' as const, labels: { padding: 16, usePointStyle: true } };

    if (this.taskChartRef) {
      this.taskChart = new Chart(this.taskChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'],
          datasets: [{
            data: [0, 0, 0, 0, 0],
            backgroundColor: ['#f59e0b', '#3b82f6', '#10b981', '#ef4444', '#8b5cf6'],
            borderWidth: 3, borderColor: '#ffffff', hoverOffset: 8
          }]
        },
        options: { ...baseOpts, cutout: '65%', plugins: { legend: legendBottom } }
      });
    }
    if (this.equipmentChartRef) {
      this.equipmentChart = new Chart(this.equipmentChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Operational', 'Under Maintenance', 'Out of Service', 'Decommissioned'],
          datasets: [{
            data: [0, 0, 0, 0],
            backgroundColor: ['#10b981', '#f59e0b', '#ef4444', '#6b7280'],
            borderWidth: 3, borderColor: '#ffffff', hoverOffset: 8
          }]
        },
        options: { ...baseOpts, cutout: '65%', plugins: { legend: legendBottom } }
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
            backgroundColor: [
              'rgba(16,185,129,0.85)', 'rgba(59,130,246,0.85)',
              'rgba(245,158,11,0.85)', 'rgba(107,114,128,0.85)'
            ],
            borderRadius: 8, borderSkipped: false
          }]
        },
        options: {
          ...baseOpts,
          plugins: { legend: { display: false } },
          scales: {
            y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: 'rgba(0,0,0,0.05)' } },
            x: { grid: { display: false } }
          }
        }
      });
    }
    if (this.trendChartRef) {
      this.trendChart = new Chart(this.trendChartRef.nativeElement, {
        type: 'line',
        data: {
          labels: this.trendLabels,
          datasets: [{
            label: 'Tasks Created',
            data: this.trendCounts,
            borderColor: '#3b82f6',
            backgroundColor: 'rgba(59,130,246,0.12)',
            tension: 0.4,
            fill: true,
            pointBackgroundColor: '#3b82f6',
            pointBorderColor: '#ffffff',
            pointBorderWidth: 2,
            pointRadius: 5
          }]
        },
        options: {
          ...baseOpts,
          plugins: { legend: { display: false } },
          scales: {
            y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: 'rgba(0,0,0,0.05)' } },
            x: { grid: { display: false } }
          }
        }
      });
    }
    if (this.myTaskChartRef) {
      this.myTaskChart = new Chart(this.myTaskChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'],
          datasets: [{
            data: [0, 0, 0, 0, 0],
            backgroundColor: ['#f59e0b', '#3b82f6', '#10b981', '#ef4444', '#8b5cf6'],
            borderWidth: 3, borderColor: '#ffffff', hoverOffset: 8
          }]
        },
        options: { ...baseOpts, cutout: '65%', plugins: { legend: legendBottom } }
      });
    }
    if (this.myPriorityChartRef) {
      this.myPriorityChart = new Chart(this.myPriorityChartRef.nativeElement, {
        type: 'bar',
        data: {
          labels: ['Low', 'Medium', 'High', 'Critical'],
          datasets: [{
            label: 'Tasks by Priority',
            data: [0, 0, 0, 0],
            backgroundColor: [
              'rgba(16,185,129,0.85)', 'rgba(59,130,246,0.85)',
              'rgba(245,158,11,0.85)', 'rgba(239,68,68,0.85)'
            ],
            borderRadius: 8, borderSkipped: false
          }]
        },
        options: {
          ...baseOpts,
          plugins: { legend: { display: false } },
          scales: {
            y: { beginAtZero: true, ticks: { stepSize: 1 }, grid: { color: 'rgba(0,0,0,0.05)' } },
            x: { grid: { display: false } }
          }
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

  private updateTrendChart() {
    if (!this.trendChart) return;
    this.trendChart.data.datasets[0].data = this.trendCounts;
    this.trendChart.update();
  }

  private updateMyTaskChart() {
    if (!this.myTaskChart) return;
    const { pending, inProgress, completed, cancelled, onHold } = this.myTaskStats;
    this.myTaskChart.data.datasets[0].data = [pending, inProgress, completed, cancelled, onHold];
    this.myTaskChart.update();
  }

  private updateMyPriorityChart() {
    if (!this.myPriorityChart) return;
    const { low, medium, high, critical } = this.myPriorityStats;
    this.myPriorityChart.data.datasets[0].data = [low, medium, high, critical];
    this.myPriorityChart.update();
  }

  isAdmin(): boolean { return this.auth.isAdmin(); }
  isManagerOrAdmin(): boolean { return this.auth.isManager(); }
  isTechnicianRole(): boolean { return !this.auth.isManager() && this.auth.isAuthenticated(); }

  getLastRefreshedLabel(): string {
    const d = this.lastRefreshed();
    return d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' });
  }

  getRoleName(): string {
    if (this.auth.isAdmin()) return this.translation.translate('dashboard.roles.administrator');
    if (this.auth.isManager()) return this.translation.translate('dashboard.roles.manager');
    return this.translation.translate('dashboard.roles.technician');
  }

  getDashboardTitle(): string {
    if (this.auth.isAdmin()) return this.translation.translate('dashboard.titles.admin');
    if (this.auth.isManager()) return this.translation.translate('dashboard.titles.manager');
    return this.translation.translate('dashboard.titles.technician');
  }

  getDashboardSubtitle(): string {
    if (this.auth.isAdmin()) return this.translation.translate('dashboard.subtitles.admin');
    if (this.auth.isManager()) return this.translation.translate('dashboard.subtitles.manager');
    return this.translation.translate('dashboard.subtitles.technician');
  }
}
