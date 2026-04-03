import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminDashboardService, AdminDashboardData, AdminDashboardRow, DailyStat } from '../../services/admin-dashboard.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { MaintenanceRequestStatus } from '../../models';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './admin-dashboard.component.html',
  styleUrls: ['./admin-dashboard.component.css']
})
export class AdminDashboardComponent implements OnInit {
  data = signal<AdminDashboardData | null>(null);
  isLoading = signal(true);
  filterStatus: number | null = null;
  filterSearch = '';

  MaintenanceRequestStatus = MaintenanceRequestStatus;

  constructor(
    private service: AdminDashboardService,
    public translation: TranslationService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);
    this.service.getDashboard().subscribe({
      next: (d) => { this.data.set(d); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  get filteredRows(): AdminDashboardRow[] {
    let rows = this.data()?.rows ?? [];
    if (this.filterStatus !== null) {
      rows = rows.filter(r => r.status === this.filterStatus);
    }
    if (this.filterSearch.trim()) {
      const q = this.filterSearch.trim().toLowerCase();
      rows = rows.filter(r =>
        r.clientName.toLowerCase().includes(q) ||
        r.technicians.toLowerCase().includes(q)
      );
    }
    return rows;
  }

  get totalRevenue(): number {
    return (this.data()?.rows ?? []).reduce((sum, r) => sum + (r.price ?? 0), 0);
  }

  get paidCount(): number {
    return (this.data()?.rows ?? []).filter(r => r.isPaid).length;
  }

  get completedCount(): number {
    return (this.data()?.rows ?? []).filter(r => r.status === MaintenanceRequestStatus.Completed).length;
  }

  statusLabel(status: MaintenanceRequestStatus): string {
    switch (status) {
      case MaintenanceRequestStatus.Pending:     return this.translation.translate('adminDashboard.status.pending');
      case MaintenanceRequestStatus.UnderReview: return this.translation.translate('adminDashboard.status.underReview');
      case MaintenanceRequestStatus.Approved:    return this.translation.translate('adminDashboard.status.approved');
      case MaintenanceRequestStatus.InProgress:  return this.translation.translate('adminDashboard.status.inProgress');
      case MaintenanceRequestStatus.Completed:   return this.translation.translate('adminDashboard.status.completed');
      case MaintenanceRequestStatus.Rejected:    return this.translation.translate('adminDashboard.status.rejected');
      case MaintenanceRequestStatus.Cancelled:   return this.translation.translate('adminDashboard.status.cancelled');
      default: return String(status);
    }
  }

  statusClass(status: MaintenanceRequestStatus): string {
    switch (status) {
      case MaintenanceRequestStatus.Pending:     return 'badge-pending';
      case MaintenanceRequestStatus.UnderReview: return 'badge-review';
      case MaintenanceRequestStatus.Approved:    return 'badge-approved';
      case MaintenanceRequestStatus.InProgress:  return 'badge-progress';
      case MaintenanceRequestStatus.Completed:   return 'badge-completed';
      case MaintenanceRequestStatus.Rejected:    return 'badge-rejected';
      case MaintenanceRequestStatus.Cancelled:   return 'badge-cancelled';
      default: return '';
    }
  }
}
