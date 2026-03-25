import { Component, OnInit, OnDestroy, signal, AfterViewInit, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { TaskOrderService } from '../../services/task-order.service';
import { TechnicianService } from '../../services/technician.service';
import { EquipmentService } from '../../services/equipment.service';
import { NotificationService } from '../../services/notification.service';
import { InvoiceService } from '../../services/invoice.service';
import { ReportService } from '../../services/report.service';
import { SparePartService } from '../../services/spare-part.service';
import { MaintenanceScheduleService } from '../../services/maintenance-schedule.service';
import { ManagerService } from '../../services/manager.service';
import { TranslationService } from '../../services/translate.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TaskStatus, EquipmentStatus, TechnicianStatus, TaskPriority, InvoiceStatus, TaskOrder } from '../../models';
import {
  Chart, ArcElement, DoughnutController, BarController, BarElement,
  CategoryScale, LinearScale, Tooltip, Legend,
  LineController, LineElement, PointElement, Filler,
  PieController, RadarController, RadialLinearScale, PolarAreaController
} from 'chart.js';
import ChartDataLabels from 'chartjs-plugin-datalabels';

// Register all required components
Chart.register(
  ArcElement, DoughnutController, BarController, BarElement,
  CategoryScale, LinearScale, Tooltip, Legend,
  LineController, LineElement, PointElement, Filler,
  ChartDataLabels,
  PieController, RadarController, RadialLinearScale, PolarAreaController
);

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe],
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, AfterViewInit, OnDestroy {
  // Admin / Manager chart refs
  @ViewChild('taskChart') taskChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('equipmentChart') equipmentChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('technicianChart') technicianChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('trendChart') trendChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('performanceChart') performanceChartRef!: ElementRef<HTMLCanvasElement>;
  // Technician chart refs
  @ViewChild('myTaskChart') myTaskChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('myPriorityChart') myPriorityChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('weeklyActivityChart') weeklyActivityChartRef!: ElementRef<HTMLCanvasElement>;
  // Admin / Manager advanced chart refs
  @ViewChild('invoicePieChart') invoicePieChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('systemRadarChart') systemRadarChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('priorityPolarChart') priorityPolarChartRef!: ElementRef<HTMLCanvasElement>;
  // Technician advanced chart refs
  @ViewChild('myRadarChart') myRadarChartRef!: ElementRef<HTMLCanvasElement>;
  @ViewChild('myPriorityPolarChart') myPriorityPolarChartRef!: ElementRef<HTMLCanvasElement>;

  // Current technician's entity ID
  private currentTechnicianId = signal<string | null>(null);
  selectedPeriod = signal<'today' | '7d' | '30d' | 'all'>('all');
  lastRefreshed = signal<Date>(new Date());
  isRefreshing = signal(false);
  
  // Animation state
  animatedStats = signal<Map<string, number>>(new Map());

  // Shared signals
  taskCount = signal(0);
  equipmentCount = signal(0);
  dueMaintenance = signal(0);
  
  // Admin / Manager signals
  technicianCount = signal(0);
  managerCount = signal(0);
  invoiceCount = signal(0);
  pendingInvoices = signal(0);
  reportCount = signal(0);
  sparePartsCount = signal(0);
  lowStockCount = signal(0);
  activeSchedulesCount = signal(0);
  overdueSchedulesCount = signal(0);
  completionRate = signal(0);
  avgCompletionTime = signal(0);
  
  // Technician signals
  myTaskCount = signal(0);
  myPendingCount = signal(0);
  myInProgressCount = signal(0);
  myCompletedCount = signal(0);
  myCompletionRate = signal(0);
  // Invoice breakdown signals for Pie chart
  invoiceDraftCount = signal(0);
  invoiceSentCount = signal(0);
  invoicePaidCount = signal(0);
  invoiceOverdueCount = signal(0);
  invoiceCancelledCount = signal(0);

  private taskChart: Chart | null = null;
  private equipmentChart: Chart | null = null;
  private technicianChart: Chart | null = null;
  private trendChart: Chart | null = null;
  private performanceChart: Chart | null = null;
  private myTaskChart: Chart | null = null;
  private myPriorityChart: Chart | null = null;
  private weeklyActivityChart: Chart | null = null;
  private invoicePieChart: Chart | null = null;
  private systemRadarChart: Chart | null = null;
  private priorityPolarChart: Chart | null = null;
  private myRadarChart: Chart | null = null;
  private myPriorityPolarChart: Chart | null = null;
  private refreshInterval: ReturnType<typeof setInterval> | null = null;

  private taskStats = { pending: 0, inProgress: 0, completed: 0, cancelled: 0, onHold: 0 };
  private equipmentStats = { operational: 0, maintenance: 0, outOfService: 0, decommissioned: 0 };
  private technicianStats = { available: 0, busy: 0, onLeave: 0, inactive: 0 };
  private myTaskStats = { pending: 0, inProgress: 0, completed: 0, cancelled: 0, onHold: 0 };
  private myPriorityStats = { low: 0, medium: 0, high: 0, critical: 0 };
  private taskPriorityStats = { low: 0, medium: 0, high: 0, critical: 0 };
  private weeklyActivity: number[] = [];
  trendLabels: string[] = [];
  private trendCounts: number[] = [];

  // Raw task list for period-filtering
  private allTasks: TaskOrder[] = [];

  constructor(
    public auth: AuthService,
    public translation: TranslationService,
    private router: Router,
    private taskService: TaskOrderService,
    private techService: TechnicianService,
    private eqService: EquipmentService,
    private notifService: NotificationService,
    private invoiceService: InvoiceService,
    private reportService: ReportService,
    private sparePartService: SparePartService,
    private scheduleService: MaintenanceScheduleService,
    private managerService: ManagerService
  ) {}

  ngOnInit() {
    this.trendLabels = this.buildTrendLabels();
    
    if (this.isTechnicianRole()) {
      this.techService.getMe().subscribe({
        next: tech => {
          this.currentTechnicianId.set(tech.id);
          this.loadData();
        },
        error: () => {
          console.warn('Could not load technician profile; falling back to all tasks.');
          this.loadData();
        }
      });
    } else {
      this.loadData();
    }
    this.refreshInterval = setInterval(() => this.loadData(), 30000);
    this.initializeAnimatedStats();
  }

  ngAfterViewInit() {
    setTimeout(() => {
      this.initCharts();
    }, 100);
  }

  ngOnDestroy() {
    if (this.refreshInterval) clearInterval(this.refreshInterval);
    this.taskChart?.destroy();
    this.equipmentChart?.destroy();
    this.technicianChart?.destroy();
    this.trendChart?.destroy();
    this.performanceChart?.destroy();
    this.myTaskChart?.destroy();
    this.myPriorityChart?.destroy();
    this.weeklyActivityChart?.destroy();
    this.invoicePieChart?.destroy();
    this.systemRadarChart?.destroy();
    this.priorityPolarChart?.destroy();
    this.myRadarChart?.destroy();
    this.myPriorityPolarChart?.destroy();
  }

  // Navigation method
  navigateTo(route: string): void {
    this.router.navigate([`/${route}`]);
  }

  // Refresh charts method
  refreshCharts(): void {
    this.loadData();
    this.notifService.showSuccess('Dashboard refreshed successfully');
  }

  // Get active technicians count
  getActiveTechnicians(): number {
    return this.technicianStats.available + this.technicianStats.busy;
  }

  private initializeAnimatedStats() {
    const statsMap = new Map<string, number>();
    statsMap.set('taskCount', 0);
    statsMap.set('technicianCount', 0);
    statsMap.set('equipmentCount', 0);
    statsMap.set('completionRate', 0);
    this.animatedStats.set(statsMap);
  }

  private animateValue(key: string, start: number, end: number, duration: number = 1000) {
    const startTime = performance.now();
    const updateValue = (currentTime: number) => {
      const elapsed = currentTime - startTime;
      const progress = Math.min(elapsed / duration, 1);
      const easeOutCubic = 1 - Math.pow(1 - progress, 3);
      const currentValue = start + (end - start) * easeOutCubic;
      
      const currentMap = this.animatedStats();
      currentMap.set(key, Math.floor(currentValue));
      this.animatedStats.set(new Map(currentMap));
      
      if (progress < 1) {
        requestAnimationFrame(updateValue);
      }
    };
    requestAnimationFrame(updateValue);
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
    this.animateValue('taskCount', 0, active.length, 800);
    
    this.taskStats = {
      pending: filtered.filter(t => t.status === TaskStatus.Pending).length,
      inProgress: filtered.filter(t => t.status === TaskStatus.InProgress).length,
      completed: filtered.filter(t => t.status === TaskStatus.Completed).length,
      cancelled: filtered.filter(t => t.status === TaskStatus.Cancelled).length,
      onHold: filtered.filter(t => t.status === TaskStatus.OnHold).length
    };
    
    this.taskPriorityStats = {
      low: filtered.filter(t => t.priority === TaskPriority.Low).length,
      medium: filtered.filter(t => t.priority === TaskPriority.Medium).length,
      high: filtered.filter(t => t.priority === TaskPriority.High).length,
      critical: filtered.filter(t => t.priority === TaskPriority.Critical).length
    };
    
    const completedCount = this.taskStats.completed;
    const totalNonCancelled = filtered.filter(t => t.status !== TaskStatus.Cancelled).length;
    const completionRate = totalNonCancelled > 0 ? (completedCount / totalNonCancelled) * 100 : 0;
    this.completionRate.set(Math.round(completionRate));
    this.animateValue('completionRate', 0, completionRate, 800);
    
    // Calculate average completion time (in days)
    const completedTasks = filtered.filter(t => t.status === TaskStatus.Completed && t.completedDate);
    if (completedTasks.length > 0) {
      const avgDays = completedTasks.reduce((sum, task) => {
        const created = new Date(task.createdAt);
        const completed = new Date(task.completedDate!);
        const days = (completed.getTime() - created.getTime()) / (1000 * 60 * 60 * 24);
        return sum + days;
      }, 0) / completedTasks.length;
      this.avgCompletionTime.set(Math.round(avgDays * 10) / 10);
    }
    
    this.trendCounts = this.buildTrendCounts(filtered);
    this.weeklyActivity = this.buildWeeklyActivity(filtered);
    this.updateTaskChart();
    this.updateTrendChart();
    this.updatePerformanceChart();
    this.updatePriorityPolarChart();
    this.updateSystemRadarChart();
    
    if (this.isTechnicianRole()) {
      this.computeMyTaskStats(filtered);
    }
  }

  private loadData() {
    this.isRefreshing.set(true);

    const techId = this.currentTechnicianId();
    const tasks$ = this.isTechnicianRole() && techId
      ? this.taskService.getByTechnician(techId)
      : this.taskService.getAll();

    tasks$.subscribe({
      next: tasks => {
        this.allTasks = tasks;
        this.applyTaskFilter();
        this.isRefreshing.set(false);
        this.lastRefreshed.set(new Date());
      },
      error: () => { 
        this.isRefreshing.set(false);
      }
    });

    if (this.isManagerOrAdmin()) {
      this.techService.getAll().subscribe({
        next: techs => {
          this.technicianCount.set(techs.length);
          this.animateValue('technicianCount', 0, techs.length, 800);
          this.technicianStats = {
            available: techs.filter(t => t.status === TechnicianStatus.Available).length,
            busy: techs.filter(t => t.status === TechnicianStatus.Busy).length,
            onLeave: techs.filter(t => t.status === TechnicianStatus.OnLeave).length,
            inactive: techs.filter(t => t.status === TechnicianStatus.Inactive).length
          };
          this.updateTechnicianChart();
          this.updateSystemRadarChart();
        },
        error: () => {}
      });
      
      this.invoiceService.getAll().subscribe({
        next: invoices => {
          this.invoiceCount.set(invoices.length);
          this.pendingInvoices.set(
            invoices.filter(i => i.status === InvoiceStatus.Sent || i.status === InvoiceStatus.Overdue).length
          );
          this.invoiceDraftCount.set(invoices.filter(i => i.status === InvoiceStatus.Draft).length);
          this.invoiceSentCount.set(invoices.filter(i => i.status === InvoiceStatus.Sent).length);
          this.invoicePaidCount.set(invoices.filter(i => i.status === InvoiceStatus.Paid).length);
          this.invoiceOverdueCount.set(invoices.filter(i => i.status === InvoiceStatus.Overdue).length);
          this.invoiceCancelledCount.set(invoices.filter(i => i.status === InvoiceStatus.Cancelled).length);
          this.updateInvoicePieChart();
          this.updateSystemRadarChart();
        },
        error: () => {}
      });
      
      this.reportService.getAll().subscribe({
        next: reports => this.reportCount.set(reports.length),
        error: () => {}
      });
      
      this.sparePartService.getAll().subscribe({
        next: parts => {
          this.sparePartsCount.set(parts.length);
          this.lowStockCount.set(parts.filter(p => p.isLowStock).length);
        },
        error: () => {}
      });
      
      this.scheduleService.getAll().subscribe({
        next: schedules => {
          const now = new Date();
          this.activeSchedulesCount.set(schedules.filter(s => s.isActive).length);
          this.overdueSchedulesCount.set(
            schedules.filter(s => s.isActive && s.nextDueAt && new Date(s.nextDueAt) < now).length
          );
        },
        error: () => {}
      });
      
      if (this.isAdmin()) {
        this.managerService.getAll().subscribe({
          next: managers => this.managerCount.set(managers.length),
          error: () => {}
        });
      }
    }

    this.eqService.getAll().subscribe({
      next: eqs => {
        this.equipmentCount.set(eqs.length);
        this.animateValue('equipmentCount', 0, eqs.length, 800);
        this.equipmentStats = {
          operational: eqs.filter(e => e.status === EquipmentStatus.Operational).length,
          maintenance: eqs.filter(e => e.status === EquipmentStatus.UnderMaintenance).length,
          outOfService: eqs.filter(e => e.status === EquipmentStatus.OutOfService).length,
          decommissioned: eqs.filter(e => e.status === EquipmentStatus.Decommissioned).length
        };
        this.updateEquipmentChart();
        this.updateSystemRadarChart();
      },
      error: () => {}
    });
    
    this.eqService.getDueMaintenance().subscribe({ 
      next: e => this.dueMaintenance.set(e.length), 
      error: () => {} 
    });
  }

  private computeMyTaskStats(tasks: TaskOrder[]) {
    this.myTaskCount.set(tasks.length);
    this.myPendingCount.set(tasks.filter(t => t.status === TaskStatus.Pending).length);
    this.myInProgressCount.set(tasks.filter(t => t.status === TaskStatus.InProgress).length);
    this.myCompletedCount.set(tasks.filter(t => t.status === TaskStatus.Completed).length);
    
    const completedCount = tasks.filter(t => t.status === TaskStatus.Completed).length;
    const totalNonCancelled = tasks.filter(t => t.status !== TaskStatus.Cancelled).length;
    const completionRate = totalNonCancelled > 0 ? (completedCount / totalNonCancelled) * 100 : 0;
    this.myCompletionRate.set(Math.round(completionRate));
    
    this.myTaskStats = {
      pending: tasks.filter(t => t.status === TaskStatus.Pending).length,
      inProgress: tasks.filter(t => t.status === TaskStatus.InProgress).length,
      completed: tasks.filter(t => t.status === TaskStatus.Completed).length,
      cancelled: tasks.filter(t => t.status === TaskStatus.Cancelled).length,
      onHold: tasks.filter(t => t.status === TaskStatus.OnHold).length
    };
    
    this.myPriorityStats = {
      low: tasks.filter(t => t.priority === TaskPriority.Low).length,
      medium: tasks.filter(t => t.priority === TaskPriority.Medium).length,
      high: tasks.filter(t => t.priority === TaskPriority.High).length,
      critical: tasks.filter(t => t.priority === TaskPriority.Critical).length
    };
    
    this.updateMyTaskChart();
    this.updateMyPriorityChart();
    this.updateWeeklyActivityChart();
    this.updateMyRadarChart();
    this.updateMyPriorityPolarChart();
  }

  private buildTrendLabels(): string[] {
    const labels: string[] = [];
    for (let i = 6; i >= 0; i--) {
      const d = new Date();
      d.setDate(d.getDate() - i);
      labels.push(d.toLocaleDateString('en-US', { month: 'short', day: 'numeric' }));
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

  private buildWeeklyActivity(tasks: TaskOrder[]): number[] {
    const activity = new Array(7).fill(0);
    tasks.forEach(task => {
      const created = new Date(task.createdAt);
      const dayOfWeek = created.getDay();
      activity[dayOfWeek]++;
    });
    return activity;
  }

  private initCharts() {
    const baseOpts = { 
      responsive: true, 
      maintainAspectRatio: false,
      animation: {
        duration: 1000,
        easing: 'easeInOutQuart' as const
      }
    };
    
    const legendBottom = { 
      position: 'bottom' as const, 
      labels: { 
        padding: 16, 
        usePointStyle: true,
        font: { size: 11, weight: '500' as const },
        boxWidth: 10
      } 
    };

    // Simpler legend for chart types that require numeric or standard font weights
    const legendSimple = {
      position: 'bottom' as const,
      labels: {
        padding: 16,
        usePointStyle: true,
        font: { size: 11 },
        boxWidth: 10
      }
    };

    // Task Status Doughnut Chart
    if (this.taskChartRef) {
      this.taskChart = new Chart(this.taskChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'],
          datasets: [{
            data: [0, 0, 0, 0, 0],
            backgroundColor: ['#f59e0b', '#3b82f6', '#10b981', '#ef4444', '#8b5cf6'],
            borderWidth: 0,
            hoverOffset: 15,
           
            borderRadius: 8,
            spacing: 2
          }]
        },
        options: { 
          ...baseOpts, 
          cutout: '70%', 
          plugins: { 
           
            tooltip: {
              callbacks: {
                label: (context) => {
                  const label = context.label || '';
                  const value = context.raw as number;
                  const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
                  const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                  return `${label}: ${value} (${percentage}%)`;
                }
              }
            }
          } 
        }
      });
    }

    // Equipment Status Doughnut Chart
    if (this.equipmentChartRef) {
      this.equipmentChart = new Chart(this.equipmentChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Operational', 'Under Maintenance', 'Out of Service', 'Decommissioned'],
          datasets: [{
            data: [0, 0, 0, 0],
            backgroundColor: ['#10b981', '#f59e0b', '#ef4444', '#6b7280'],
            borderWidth: 0,
            hoverOffset: 15,
           
            borderRadius: 8,
            spacing: 2
          }]
        },
        options: { 
          ...baseOpts, 
          cutout: '70%', 
          plugins: { 
           
            tooltip: {
              callbacks: {
                label: (context) => {
                  const label = context.label || '';
                  const value = context.raw as number;
                  const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
                  const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                  return `${label}: ${value} (${percentage}%)`;
                }
              }
            }
          } 
        }
      });
    }

    // Technician Status Bar Chart
    if (this.technicianChartRef) {
      this.technicianChart = new Chart(this.technicianChartRef.nativeElement, {
        type: 'bar',
        data: {
          labels: ['Available', 'Busy', 'On Leave', 'Inactive'],
          datasets: [{
            label: 'Technicians',
            data: [0, 0, 0, 0],
            backgroundColor: [
              'rgba(16,185,129,0.85)',
              'rgba(59,130,246,0.85)',
              'rgba(245,158,11,0.85)',
              'rgba(107,114,128,0.85)'
            ],
            borderRadius: 12,
            barPercentage: 0.65,
            categoryPercentage: 0.8,
            borderSkipped: false
          }]
        },
        options: {
          ...baseOpts,
          plugins: { 
            legend: { display: false },
            tooltip: {
              callbacks: {
                label: (context) => {
                  const value = context.raw as number;
                  return `Count: ${value}`;
                }
              }
            }
          },
          scales: {
            y: { 
              beginAtZero: true, 
              ticks: { stepSize: 1, font: { size: 11 } }, 
              grid: { color: 'rgba(0,0,0,0.05)' },
              title: { display: true, text: 'Number of Technicians', font: { size: 11 } }
            },
            x: { 
              grid: { display: false },
              ticks: { font: { size: 11, weight: 'normal' as const } }
            }
          }
        }
      });
    }

    // Trend Line Chart
    if (this.trendChartRef) {
      const ctx = this.trendChartRef.nativeElement.getContext('2d');
      const gradient = ctx?.createLinearGradient(0, 0, 0, 200);
      gradient?.addColorStop(0, 'rgba(59,130,246,0.3)');
      gradient?.addColorStop(1, 'rgba(59,130,246,0.02)');
      
      this.trendChart = new Chart(this.trendChartRef.nativeElement, {
        type: 'line',
        data: {
          labels: this.trendLabels,
          datasets: [{
            label: 'Tasks Created',
            data: this.trendCounts,
            borderColor: '#3b82f6',
            backgroundColor: gradient,
            tension: 0.4,
            fill: true,
            pointBackgroundColor: '#3b82f6',
            pointBorderColor: '#ffffff',
            pointBorderWidth: 2,
            pointRadius: 5,
            pointHoverRadius: 8,
            pointHoverBackgroundColor: '#1e40af',
            borderWidth: 3
          }]
        },
        options: {
          ...baseOpts,
          plugins: { 
            legend: { display: false },
            tooltip: {
              callbacks: {
                label: (context) => `Tasks: ${context.raw}`
              }
            }
          },
          scales: {
            y: { 
              beginAtZero: true, 
              ticks: { stepSize: 1, font: { size: 11 } }, 
              grid: { color: 'rgba(0,0,0,0.05)' },
              title: { display: true, text: 'Number of Tasks', font: { size: 11 } }
            },
            x: { 
              grid: { display: false },
              ticks: { font: { size: 11 } }
            }
          }
        }
      });
    }

    // Performance Gauge Chart
    if (this.performanceChartRef) {
      this.performanceChart = new Chart(this.performanceChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Completion Rate', 'Remaining'],
          datasets: [{
            data: [0, 100],
            backgroundColor: ['#10b981', '#e5e7eb'],
            borderWidth: 0,
           
            borderRadius: 10
          }]
        },
        options: {
          ...baseOpts,
          cutout: '75%',
          plugins: {
            legend: { display: false },
            tooltip: { enabled: false }
          }
        }
      });
    }

    // Technician Task Chart
    if (this.myTaskChartRef) {
      this.myTaskChart = new Chart(this.myTaskChartRef.nativeElement, {
        type: 'doughnut',
        data: {
          labels: ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'],
          datasets: [{
            data: [0, 0, 0, 0, 0],
            backgroundColor: ['#f59e0b', '#3b82f6', '#10b981', '#ef4444', '#8b5cf6'],
            borderWidth: 0,
            hoverOffset: 15,
         
            borderRadius: 8,
            spacing: 2
          }]
        },
        options: { 
          ...baseOpts, 
          cutout: '70%', 
          plugins: { 
           
            tooltip: {
              callbacks: {
                label: (context) => {
                  const label = context.label || '';
                  const value = context.raw as number;
                  const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
                  const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                  return `${label}: ${value} (${percentage}%)`;
                }
              }
            }
          } 
        }
      });
    }

    // Priority Chart
    if (this.myPriorityChartRef) {
      this.myPriorityChart = new Chart(this.myPriorityChartRef.nativeElement, {
        type: 'bar',
        data: {
          labels: ['Low', 'Medium', 'High', 'Critical'],
          datasets: [{
            label: 'Tasks by Priority',
            data: [0, 0, 0, 0],
            backgroundColor: [
              'rgba(16,185,129,0.85)',
              'rgba(59,130,246,0.85)',
              'rgba(245,158,11,0.85)',
              'rgba(239,68,68,0.85)'
            ],
            borderRadius: 12,
            barPercentage: 0.7,
            categoryPercentage: 0.85,
            borderSkipped: false
          }]
        },
        options: {
          ...baseOpts,
          plugins: { 
            legend: { display: false },
            tooltip: {
              callbacks: {
                label: (context) => {
                  const value = context.raw as number;
                  return `Tasks: ${value}`;
                }
              }
            }
          },
          scales: {
            y: { 
              beginAtZero: true, 
              ticks: { stepSize: 1, font: { size: 11 } }, 
              grid: { color: 'rgba(0,0,0,0.05)' },
              title: { display: true, text: 'Number of Tasks', font: { size: 11 } }
            },
            x: { 
              grid: { display: false },
              ticks: { font: { size: 11, weight: 'normal' as const } }
            }
          }
        }
      });
    }

    // Weekly Activity Chart
    if (this.weeklyActivityChartRef) {
      this.weeklyActivityChart = new Chart(this.weeklyActivityChartRef.nativeElement, {
        type: 'bar',
        data: {
          labels: ['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'],
          datasets: [{
            label: 'Task Activity',
            data: [0, 0, 0, 0, 0, 0, 0],
            backgroundColor: 'rgba(139,92,246,0.7)',
            borderRadius: 8,
            barPercentage: 0.6,
            categoryPercentage: 0.8
          }]
        },
        options: {
          ...baseOpts,
          plugins: {
            legend: { display: false },
            tooltip: {
              callbacks: {
                label: (context) => `Tasks: ${context.raw}`
              }
            }
          },
          scales: {
            y: { 
              beginAtZero: true, 
              ticks: { stepSize: 1, font: { size: 11 } }, 
              grid: { color: 'rgba(0,0,0,0.05)' }
            },
            x: { 
              grid: { display: false },
              ticks: { font: { size: 11 } }
            }
          }
        }
      });
    }

    // Invoice Status Pie Chart
    if (this.invoicePieChartRef) {
      this.invoicePieChart = new Chart(this.invoicePieChartRef.nativeElement, {
        type: 'pie',
        data: {
          labels: ['Draft', 'Sent', 'Paid', 'Overdue', 'Cancelled'],
          datasets: [{
            data: [0, 0, 0, 0, 0],
            backgroundColor: ['#94a3b8', '#3b82f6', '#10b981', '#ef4444', '#6b7280'],
            borderWidth: 2,
            borderColor: '#ffffff',
            hoverOffset: 12
          }]
        },
        options: {
          ...baseOpts,
          plugins: {
            legend: { ...legendSimple },
            tooltip: {
              callbacks: {
                label: (context) => {
                  const label = context.label || '';
                  const value = context.raw as number;
                  const total = context.dataset.data.reduce((a: number, b: number) => a + b, 0);
                  const percentage = total > 0 ? ((value / total) * 100).toFixed(1) : 0;
                  return `${label}: ${value} (${percentage}%)`;
                }
              }
            }
          }
        }
      });
    }

    // System Health Radar Chart
    if (this.systemRadarChartRef) {
      this.systemRadarChart = new Chart(this.systemRadarChartRef.nativeElement, {
        type: 'radar',
        data: {
          labels: ['Task Completion', 'Tech Availability', 'Equipment Health', 'Schedule Adherence', 'Invoice Payment'],
          datasets: [{
            label: 'Current',
            data: [0, 0, 0, 0, 0],
            backgroundColor: 'rgba(99,102,241,0.2)',
            borderColor: '#6366f1',
            pointBackgroundColor: '#6366f1',
            pointBorderColor: '#ffffff',
            pointBorderWidth: 2,
            pointRadius: 5,
            borderWidth: 2
          }, {
            label: 'Target',
            data: [80, 80, 80, 80, 80],
            backgroundColor: 'rgba(16,185,129,0.08)',
            borderColor: 'rgba(16,185,129,0.4)',
            pointBackgroundColor: 'rgba(16,185,129,0.6)',
            pointBorderColor: '#ffffff',
            pointBorderWidth: 1,
            pointRadius: 3,
            borderWidth: 1,
            borderDash: [5, 5]
          }]
        },
        options: {
          ...baseOpts,
          scales: {
            r: {
              min: 0,
              max: 100,
              ticks: { stepSize: 20, font: { size: 10 }, backdropColor: 'transparent' },
              grid: { color: 'rgba(0,0,0,0.08)' },
              angleLines: { color: 'rgba(0,0,0,0.08)' },
              pointLabels: { font: { size: 10, weight: 'bold' as const } }
            }
          },
          plugins: {
            legend: { ...legendSimple },
            tooltip: {
              callbacks: {
                label: (context) => `${context.dataset.label}: ${context.raw}%`
              }
            }
          }
        }
      });
    }

    // Task Priority Polar Area Chart
    if (this.priorityPolarChartRef) {
      this.priorityPolarChart = new Chart(this.priorityPolarChartRef.nativeElement, {
        type: 'polarArea',
        data: {
          labels: ['Low', 'Medium', 'High', 'Critical'],
          datasets: [{
            data: [0, 0, 0, 0],
            backgroundColor: [
              'rgba(16,185,129,0.75)',
              'rgba(59,130,246,0.75)',
              'rgba(245,158,11,0.75)',
              'rgba(239,68,68,0.75)'
            ],
            borderColor: ['#10b981', '#3b82f6', '#f59e0b', '#ef4444'],
            borderWidth: 2
          }]
        },
        options: {
          ...baseOpts,
          scales: {
            r: {
              ticks: { stepSize: 1, font: { size: 10 }, backdropColor: 'transparent' },
              grid: { color: 'rgba(0,0,0,0.08)' }
            }
          },
          plugins: {
            legend: { ...legendSimple },
            tooltip: {
              callbacks: {
                label: (context) => `${context.label}: ${context.raw} tasks`
              }
            }
          }
        }
      });
    }

    // Technician My Performance Radar Chart
    if (this.myRadarChartRef) {
      this.myRadarChart = new Chart(this.myRadarChartRef.nativeElement, {
        type: 'radar',
        data: {
          labels: ['Pending', 'In Progress', 'Completed', 'On Hold', 'Cancelled'],
          datasets: [{
            label: 'My Tasks',
            data: [0, 0, 0, 0, 0],
            backgroundColor: 'rgba(59,130,246,0.2)',
            borderColor: '#3b82f6',
            pointBackgroundColor: '#3b82f6',
            pointBorderColor: '#ffffff',
            pointBorderWidth: 2,
            pointRadius: 5,
            borderWidth: 2
          }]
        },
        options: {
          ...baseOpts,
          scales: {
            r: {
              ticks: { stepSize: 1, font: { size: 10 }, backdropColor: 'transparent' },
              grid: { color: 'rgba(0,0,0,0.08)' },
              angleLines: { color: 'rgba(0,0,0,0.08)' },
              pointLabels: { font: { size: 10, weight: 'bold' as const } }
            }
          },
          plugins: {
            legend: { display: false },
            tooltip: {
              callbacks: {
                label: (context) => `${context.label}: ${context.raw} tasks`
              }
            }
          }
        }
      });
    }

    // Technician My Priority Polar Area Chart
    if (this.myPriorityPolarChartRef) {
      this.myPriorityPolarChart = new Chart(this.myPriorityPolarChartRef.nativeElement, {
        type: 'polarArea',
        data: {
          labels: ['Low', 'Medium', 'High', 'Critical'],
          datasets: [{
            data: [0, 0, 0, 0],
            backgroundColor: [
              'rgba(16,185,129,0.75)',
              'rgba(59,130,246,0.75)',
              'rgba(245,158,11,0.75)',
              'rgba(239,68,68,0.75)'
            ],
            borderColor: ['#10b981', '#3b82f6', '#f59e0b', '#ef4444'],
            borderWidth: 2
          }]
        },
        options: {
          ...baseOpts,
          scales: {
            r: {
              ticks: { stepSize: 1, font: { size: 10 }, backdropColor: 'transparent' },
              grid: { color: 'rgba(0,0,0,0.08)' }
            }
          },
          plugins: {
            legend: { ...legendSimple },
            tooltip: {
              callbacks: {
                label: (context) => `${context.label}: ${context.raw} tasks`
              }
            }
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

  private updatePerformanceChart() {
    if (!this.performanceChart) return;
    const completionRate = this.completionRate();
    this.performanceChart.data.datasets[0].data = [completionRate, 100 - completionRate];
    this.performanceChart.update();
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

  private updateWeeklyActivityChart() {
    if (!this.weeklyActivityChart) return;
    this.weeklyActivityChart.data.datasets[0].data = this.weeklyActivity;
    this.weeklyActivityChart.update();
  }

  private updateInvoicePieChart() {
    if (!this.invoicePieChart) return;
    this.invoicePieChart.data.datasets[0].data = [
      this.invoiceDraftCount(),
      this.invoiceSentCount(),
      this.invoicePaidCount(),
      this.invoiceOverdueCount(),
      this.invoiceCancelledCount()
    ];
    this.invoicePieChart.update();
  }

  private updateSystemRadarChart() {
    if (!this.systemRadarChart) return;
    const techAvail = this.technicianCount() > 0
      ? Math.round((this.technicianStats.available / this.technicianCount()) * 100) : 0;
    const eqHealth = this.equipmentCount() > 0
      ? Math.round((this.equipmentStats.operational / this.equipmentCount()) * 100) : 0;
    const activeSchedules = this.activeSchedulesCount();
    const schedAdh = activeSchedules > 0
      ? Math.round(((activeSchedules - this.overdueSchedulesCount()) / activeSchedules) * 100) : 100;
    const invPayment = this.invoiceCount() > 0
      ? Math.round((this.invoicePaidCount() / this.invoiceCount()) * 100) : 0;
    this.systemRadarChart.data.datasets[0].data = [
      this.completionRate(), techAvail, eqHealth, schedAdh, invPayment
    ];
    this.systemRadarChart.update();
  }

  private updatePriorityPolarChart() {
    if (!this.priorityPolarChart) return;
    const { low, medium, high, critical } = this.taskPriorityStats;
    this.priorityPolarChart.data.datasets[0].data = [low, medium, high, critical];
    this.priorityPolarChart.update();
  }

  private updateMyRadarChart() {
    if (!this.myRadarChart) return;
    const { pending, inProgress, completed, onHold, cancelled } = this.myTaskStats;
    this.myRadarChart.data.datasets[0].data = [pending, inProgress, completed, onHold, cancelled];
    this.myRadarChart.update();
  }

  private updateMyPriorityPolarChart() {
    if (!this.myPriorityPolarChart) return;
    const { low, medium, high, critical } = this.myPriorityStats;
    this.myPriorityPolarChart.data.datasets[0].data = [low, medium, high, critical];
    this.myPriorityPolarChart.update();
  }

  // Role check methods
  isAdmin(): boolean { 
    return this.auth.isAdmin(); 
  }
  
  isManagerOrAdmin(): boolean { 
    return this.auth.isManager() || this.auth.isAdmin(); 
  }
  
  isTechnicianRole(): boolean { 
    return !this.auth.isManager() && !this.auth.isAdmin() && this.auth.isAuthenticated(); 
  }

  getLastRefreshedLabel(): string {
    const d = this.lastRefreshed();
    return d.toLocaleTimeString([], { hour: '2-digit', minute: '2-digit', second: '2-digit' });
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

  getAnimatedValue(key: string): number {
    return this.animatedStats().get(key) || 0;
  }
}