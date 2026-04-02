import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { MaintenanceRequestService } from '../../services/maintenance-request.service';
import { MaintenanceRequest, MaintenanceRequestStatus } from '../../models';

@Component({
  selector: 'app-client-requests',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="client-requests">
      <div class="page-header">
        <div>
          <h1>My Maintenance Requests</h1>
          <p>Track the status of all your requests</p>
        </div>
        <a routerLink="/client-portal/new-request" class="btn-new">
          <i class="bi bi-plus-circle"></i> New Request
        </a>
      </div>

      <!-- Filters -->
      <div class="filters-bar">
        <select [(ngModel)]="statusFilter" (change)="applyFilter()" class="filter-select">
          <option value="">All Statuses</option>
          <option value="0">Pending</option>
          <option value="1">Under Review</option>
          <option value="2">Approved</option>
          <option value="3">In Progress</option>
          <option value="4">Completed</option>
          <option value="5">Rejected</option>
        </select>
        <input type="text" [(ngModel)]="searchText" (input)="applyFilter()" placeholder="Search requests..." class="search-input" />
      </div>

      @if (isLoading()) {
        <div class="loading"><div class="spinner"></div><p>Loading your requests...</p></div>
      } @else if (filteredRequests().length === 0) {
        <div class="empty-state">
          <i class="bi bi-inbox"></i>
          <h3>No requests found</h3>
          <p>You haven't submitted any maintenance requests yet.</p>
          <a routerLink="/client-portal/new-request" class="btn-create">Submit Your First Request</a>
        </div>
      } @else {
        <div class="requests-grid">
          @for (req of filteredRequests(); track req.id) {
            <div class="request-card">
              <div class="card-header">
                <h3>{{ req.title }}</h3>
                <span class="status-badge" [class]="getStatusClass(req.status)">
                  {{ getStatusLabel(req.status) }}
                </span>
              </div>
              @if (req.description) {
                <p class="card-description">{{ req.description }}</p>
              }
              @if (req.equipmentDescription) {
                <div class="card-detail">
                  <i class="bi bi-cpu"></i>
                  <span>{{ req.equipmentDescription }}</span>
                </div>
              }
              <div class="card-footer">
                <span class="req-date"><i class="bi bi-calendar3"></i> {{ req.requestDate | date:'MMM d, yyyy' }}</span>
                @if (req.taskTitle) {
                  <span class="task-ref"><i class="bi bi-link-45deg"></i> {{ req.taskTitle }}</span>
                }
              </div>
              @if (req.notes) {
                <div class="card-notes">
                  <i class="bi bi-info-circle"></i>
                  <span>{{ req.notes }}</span>
                </div>
              }
            </div>
          }
        </div>

        <div class="pagination" *ngIf="totalPages() > 1">
          <button (click)="prevPage()" [disabled]="currentPage() === 1" class="page-btn">
            <i class="bi bi-chevron-left"></i>
          </button>
          <span>Page {{ currentPage() }} of {{ totalPages() }}</span>
          <button (click)="nextPage()" [disabled]="currentPage() === totalPages()" class="page-btn">
            <i class="bi bi-chevron-right"></i>
          </button>
        </div>
      }
    </div>
  `,
  styles: [`
    .client-requests { max-width: 1000px; margin: 0 auto; }
    .page-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 24px; flex-wrap: wrap; gap: 16px; }
    .page-header h1 { font-size: 1.5rem; font-weight: 700; color: #1a237e; margin: 0 0 4px; }
    .page-header p { color: #666; margin: 0; }
    .btn-new {
      display: flex;
      align-items: center;
      gap: 8px;
      background: linear-gradient(135deg, #1565c0, #1a237e);
      color: white;
      padding: 10px 20px;
      border-radius: 8px;
      text-decoration: none;
      font-weight: 600;
    }
    .btn-new:hover { opacity: 0.9; }

    .filters-bar { display: flex; gap: 12px; margin-bottom: 20px; flex-wrap: wrap; }
    .filter-select, .search-input {
      padding: 10px 14px;
      border: 1.5px solid #e0e0e0;
      border-radius: 8px;
      font-size: 0.9rem;
      outline: none;
    }
    .filter-select:focus, .search-input:focus { border-color: #1565c0; }
    .search-input { flex: 1; min-width: 200px; }

    .loading { display: flex; flex-direction: column; align-items: center; padding: 60px; color: #666; gap: 16px; }
    .spinner { width: 36px; height: 36px; border: 3px solid #e3f2fd; border-top-color: #1565c0; border-radius: 50%; animation: spin 0.8s linear infinite; }
    @keyframes spin { to { transform: rotate(360deg); } }

    .empty-state { display: flex; flex-direction: column; align-items: center; padding: 60px; color: #999; gap: 12px; text-align: center; }
    .empty-state i { font-size: 56px; color: #bbdefb; }
    .empty-state h3 { color: #555; margin: 0; }
    .empty-state p { margin: 0; }
    .btn-create { background: #e3f2fd; color: #1565c0; padding: 10px 24px; border-radius: 8px; text-decoration: none; font-weight: 600; }

    .requests-grid { display: flex; flex-direction: column; gap: 16px; }
    .request-card {
      background: white;
      border-radius: 12px;
      padding: 20px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.06);
      border-left: 4px solid #1565c0;
    }
    .card-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 10px; gap: 12px; }
    .card-header h3 { margin: 0; font-size: 1rem; font-weight: 600; color: #1a237e; }
    .card-description { color: #555; font-size: 0.9rem; margin: 0 0 10px; }
    .card-detail { display: flex; align-items: center; gap: 6px; color: #666; font-size: 0.85rem; margin-bottom: 8px; }
    .card-footer { display: flex; gap: 16px; align-items: center; flex-wrap: wrap; margin-top: 10px; border-top: 1px solid #f5f5f5; padding-top: 10px; }
    .req-date, .task-ref { display: flex; align-items: center; gap: 5px; color: #888; font-size: 0.85rem; }
    .card-notes { display: flex; align-items: flex-start; gap: 6px; margin-top: 10px; padding: 10px; background: #f5f9ff; border-radius: 6px; color: #555; font-size: 0.85rem; }

    .status-badge { padding: 4px 12px; border-radius: 20px; font-size: 0.78rem; font-weight: 600; white-space: nowrap; }
    .status-pending { background: #fff3e0; color: #e65100; }
    .status-review { background: #e3f2fd; color: #1565c0; }
    .status-approved { background: #e8f5e9; color: #2e7d32; }
    .status-progress { background: #e0f7fa; color: #00695c; }
    .status-completed { background: #e8f5e9; color: #1b5e20; }
    .status-rejected { background: #fce4ec; color: #c62828; }

    .pagination { display: flex; align-items: center; gap: 16px; justify-content: center; margin-top: 24px; color: #555; }
    .page-btn { background: white; border: 1.5px solid #e0e0e0; border-radius: 8px; padding: 6px 12px; cursor: pointer; }
    .page-btn:disabled { opacity: 0.4; cursor: not-allowed; }
  `]
})
export class ClientRequestsComponent implements OnInit {
  allRequests = signal<MaintenanceRequest[]>([]);
  filteredRequests = signal<MaintenanceRequest[]>([]);
  isLoading = signal(true);
  currentPage = signal(1);
  readonly pageSize = 10;
  statusFilter = '';
  searchText = '';

  totalPages = () => Math.ceil(this.filteredRequests().length / this.pageSize) || 1;

  constructor(private requestService: MaintenanceRequestService) {}

  ngOnInit(): void {
    this.requestService.getMyRequests().subscribe({
      next: (data) => {
        this.allRequests.set(data);
        this.applyFilter();
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  applyFilter(): void {
    let filtered = this.allRequests();
    if (this.statusFilter !== '') {
      filtered = filtered.filter(r => r.status === +this.statusFilter);
    }
    if (this.searchText.trim()) {
      const q = this.searchText.toLowerCase();
      filtered = filtered.filter(r => r.title.toLowerCase().includes(q) || r.description?.toLowerCase().includes(q));
    }
    this.filteredRequests.set(filtered);
    this.currentPage.set(1);
  }

  prevPage(): void { if (this.currentPage() > 1) this.currentPage.update(p => p - 1); }
  nextPage(): void { if (this.currentPage() < this.totalPages()) this.currentPage.update(p => p + 1); }

  getStatusClass(status: MaintenanceRequestStatus): string {
    const map: Record<number, string> = { 0: 'status-pending', 1: 'status-review', 2: 'status-approved', 3: 'status-progress', 4: 'status-completed', 5: 'status-rejected' };
    return map[status] ?? 'status-pending';
  }

  getStatusLabel(status: MaintenanceRequestStatus): string {
    const map: Record<number, string> = { 0: 'Pending', 1: 'Under Review', 2: 'Approved', 3: 'In Progress', 4: 'Completed', 5: 'Rejected', 6: 'Cancelled' };
    return map[status] ?? 'Unknown';
  }
}
