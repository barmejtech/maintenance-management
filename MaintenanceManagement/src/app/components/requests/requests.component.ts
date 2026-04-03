import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { MaintenanceRequestService } from '../../services/maintenance-request.service';
import { TechnicianService } from '../../services/technician.service';
import { ToastService } from '../../services/toast.service';
import { TravelEstimationService, TravelEstimationResult } from '../../services/travel-estimation.service';
import {
  MaintenanceRequest,
  MaintenanceRequestStatus,
  AuditLogEntry,
  Technician
} from '../../models';

@Component({
  selector: 'app-requests',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, NgxPaginationModule],
  templateUrl: './requests.component.html',
  styleUrls: ['./requests.component.css']
})
export class RequestsComponent implements OnInit {
  requests = signal<MaintenanceRequest[]>([]);
  availableTechnicians = signal<Technician[]>([]);
  auditLog = signal<AuditLogEntry[]>([]);
  isLoading = signal(true);
  isProcessing = signal(false);

  // Filters
  filterStatus = '';
  filterSearch = '';
  filterFrom = '';
  filterTo = '';

  // Selected request for detail view
  selectedRequest = signal<MaintenanceRequest | null>(null);
  showDetail = signal(false);

  // Approve / Reject modals
  showApproveModal = signal(false);
  showRejectModal = signal(false);
  reviewNotes = '';
  actionRequestId = '';

  // Assign technicians modal
  showAssignModal = signal(false);
  selectedTechnicianIds = signal<Set<string>>(new Set());

  // Audit log modal
  showAuditModal = signal(false);

  // Travel estimation
  travelEstimates = signal<Record<string, TravelEstimationResult | null>>({});
  travelEstimateLoading = signal<Record<string, boolean>>({});
  travelEstimateError = signal<Record<string, string>>({});

  currentPage = signal(1);
  readonly pageSize = 12;
  MaintenanceRequestStatus = MaintenanceRequestStatus;

  constructor(
    private requestService: MaintenanceRequestService,
    private technicianService: TechnicianService,
    private toast: ToastService,
    private travelService: TravelEstimationService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);
    const filters: any = {};
    if (this.filterStatus) filters.status = +this.filterStatus;
    if (this.filterSearch) filters.search = this.filterSearch;
    if (this.filterFrom) filters.from = this.filterFrom;
    if (this.filterTo) filters.to = this.filterTo;

    this.requestService.getAll(filters).subscribe({
      next: (data) => { this.requests.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  applyFilters() {
    this.currentPage.set(1);
    this.load();
  }

  clearFilters() {
    this.filterStatus = '';
    this.filterSearch = '';
    this.filterFrom = '';
    this.filterTo = '';
    this.load();
  }

  viewDetail(req: MaintenanceRequest) {
    this.selectedRequest.set(req);
    this.showDetail.set(true);
  }

  closeDetail() {
    this.showDetail.set(false);
    this.selectedRequest.set(null);
  }

  // ─── Approve ───────────────────────────────────────────────────────────────
  openApprove(req: MaintenanceRequest) {
    this.actionRequestId = req.id;
    this.reviewNotes = '';
    this.showApproveModal.set(true);
  }

  confirmApprove() {
    if (!this.actionRequestId) return;
    this.isProcessing.set(true);
    this.requestService.approve(this.actionRequestId, this.reviewNotes).subscribe({
      next: (updated) => {
        this.toast.show('Request approved successfully.', 'success');
        this.showApproveModal.set(false);
        this.isProcessing.set(false);
        this.updateRequest(updated);
        if (this.selectedRequest()?.id === updated.id) this.selectedRequest.set(updated);
      },
      error: () => {
        this.toast.show('Failed to approve request.', 'error');
        this.isProcessing.set(false);
      }
    });
  }

  // ─── Reject ────────────────────────────────────────────────────────────────
  openReject(req: MaintenanceRequest) {
    this.actionRequestId = req.id;
    this.reviewNotes = '';
    this.showRejectModal.set(true);
  }

  confirmReject() {
    if (!this.actionRequestId) return;
    this.isProcessing.set(true);
    this.requestService.reject(this.actionRequestId, this.reviewNotes).subscribe({
      next: (updated) => {
        this.toast.show('Request rejected.', 'success');
        this.showRejectModal.set(false);
        this.isProcessing.set(false);
        this.updateRequest(updated);
        if (this.selectedRequest()?.id === updated.id) this.selectedRequest.set(updated);
      },
      error: () => {
        this.toast.show('Failed to reject request.', 'error');
        this.isProcessing.set(false);
      }
    });
  }

  // ─── Assign Technicians ────────────────────────────────────────────────────
  openAssign(req: MaintenanceRequest) {
    this.actionRequestId = req.id;
    const preSelected = new Set<string>(req.assignedTechnicians?.map(t => t.technicianId) ?? []);
    this.selectedTechnicianIds.set(preSelected);
    this.technicianService.getAvailable().subscribe({
      next: (techs) => this.availableTechnicians.set(techs),
      error: () => this.availableTechnicians.set([])
    });
    this.showAssignModal.set(true);
  }

  toggleTechnicianSelection(id: string) {
    const current = new Set(this.selectedTechnicianIds());
    if (current.has(id)) current.delete(id);
    else current.add(id);
    this.selectedTechnicianIds.set(current);
  }

  isTechnicianSelected(id: string): boolean {
    return this.selectedTechnicianIds().has(id);
  }

  confirmAssign() {
    const ids = Array.from(this.selectedTechnicianIds());
    if (ids.length === 0) {
      this.toast.show('Please select at least one technician.', 'warning');
      return;
    }
    this.isProcessing.set(true);
    this.requestService.assignTechnicians(this.actionRequestId, ids).subscribe({
      next: (updated) => {
        this.toast.show('Technicians assigned successfully.', 'success');
        this.showAssignModal.set(false);
        this.isProcessing.set(false);
        this.updateRequest(updated);
        if (this.selectedRequest()?.id === updated.id) this.selectedRequest.set(updated);
      },
      error: () => {
        this.toast.show('Failed to assign technicians.', 'error');
        this.isProcessing.set(false);
      }
    });
  }

  // ─── Travel Estimation ─────────────────────────────────────────────────────
  estimateTravel(technicianId: string, clientId: string) {
    const key = `${technicianId}:${clientId}`;
    this.travelEstimateLoading.update(s => ({ ...s, [key]: true }));
    this.travelEstimateError.update(s => ({ ...s, [key]: '' }));
    this.travelEstimates.update(s => ({ ...s, [key]: null }));

    this.travelService.estimate(technicianId, clientId).subscribe({
      next: (res) => {
        this.travelEstimateLoading.update(s => ({ ...s, [key]: false }));
        if (res.data) {
          this.travelEstimates.update(s => ({ ...s, [key]: res.data }));
          if (!res.success) {
            this.travelEstimateError.update(s => ({ ...s, [key]: res.message ?? 'Unable to calculate route.' }));
          }
        } else {
          this.travelEstimateError.update(s => ({ ...s, [key]: res.message ?? 'Unable to calculate route.' }));
        }
      },
      error: () => {
        this.travelEstimateLoading.update(s => ({ ...s, [key]: false }));
        this.travelEstimateError.update(s => ({ ...s, [key]: 'Failed to contact routing service.' }));
      }
    });
  }

  getTravelKey(technicianId: string, clientId: string): string {
    return `${technicianId}:${clientId}`;
  }

  getTravelEstimate(key: string): TravelEstimationResult | null {
    return this.travelEstimates()[key] ?? null;
  }

  isTravelLoading(key: string): boolean {
    return this.travelEstimateLoading()[key] ?? false;
  }

  getTravelError(key: string): string {
    return this.travelEstimateError()[key] ?? '';
  }

  // ─── Audit Log ─────────────────────────────────────────────────────────────
  openAuditLog(req: MaintenanceRequest) {
    this.requestService.getAuditLog(req.id).subscribe({
      next: (logs) => {
        this.auditLog.set(logs);
        this.showAuditModal.set(true);
      },
      error: () => this.toast.show('Failed to load audit log.', 'error')
    });
  }

  // ─── Helpers ───────────────────────────────────────────────────────────────
  private updateRequest(updated: MaintenanceRequest) {
    this.requests.update(list => list.map(r => r.id === updated.id ? updated : r));
  }

  getStatusClass(status: MaintenanceRequestStatus): string {
    switch (status) {
      case MaintenanceRequestStatus.Pending:     return 'bg-warning-subtle text-warning border border-warning';
      case MaintenanceRequestStatus.UnderReview: return 'bg-info-subtle text-info border border-info';
      case MaintenanceRequestStatus.Approved:    return 'bg-success-subtle text-success border border-success';
      case MaintenanceRequestStatus.InProgress:  return 'bg-primary-subtle text-primary border border-primary';
      case MaintenanceRequestStatus.Completed:   return 'bg-secondary-subtle text-secondary border border-secondary';
      case MaintenanceRequestStatus.Rejected:    return 'bg-danger-subtle text-danger border border-danger';
      case MaintenanceRequestStatus.Cancelled:   return 'bg-dark text-white';
      default:                                   return 'bg-light text-dark';
    }
  }

  getStatusLabel(status: MaintenanceRequestStatus): string {
    switch (status) {
      case MaintenanceRequestStatus.Pending:     return 'Pending';
      case MaintenanceRequestStatus.UnderReview: return 'Under Review';
      case MaintenanceRequestStatus.Approved:    return 'Approved';
      case MaintenanceRequestStatus.InProgress:  return 'In Progress';
      case MaintenanceRequestStatus.Completed:   return 'Completed';
      case MaintenanceRequestStatus.Rejected:    return 'Rejected';
      case MaintenanceRequestStatus.Cancelled:   return 'Cancelled';
      default:                                   return 'Unknown';
    }
  }

  canApproveOrReject(req: MaintenanceRequest): boolean {
    return req.status === MaintenanceRequestStatus.Pending || req.status === MaintenanceRequestStatus.UnderReview;
  }

  canAssign(req: MaintenanceRequest): boolean {
    return req.status === MaintenanceRequestStatus.Approved || req.status === MaintenanceRequestStatus.InProgress;
  }

  getActionLabel(action: string): string {
    switch (action) {
      case 'Approved': return 'Approved';
      case 'Rejected': return 'Rejected';
      case 'TechniciansAssigned': return 'Technicians Assigned';
      default: return action;
    }
  }

  getActionBadge(action: string): string {
    switch (action) {
      case 'Approved': return 'badge bg-success';
      case 'Rejected': return 'badge bg-danger';
      case 'TechniciansAssigned': return 'badge bg-primary';
      default: return 'badge bg-secondary';
    }
  }
}
