import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-committee-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="container-fluid py-3">
      <div class="d-flex justify-content-between align-items-center mb-3 flex-wrap gap-2">
        <div>
          <h4 class="mb-1 fw-bold"><i class="bi bi-people-fill text-primary me-2"></i>Committee Member Portal</h4>
          <p class="text-muted mb-0">Role-aware dashboard for committee-level oversight and approvals.</p>
        </div>
        <a routerLink="/dashboard" class="btn btn-outline-secondary btn-sm"><i class="bi bi-arrow-left me-1"></i>Back to Dashboard</a>
      </div>

      <div class="row g-3 mb-3">
        <div class="col-12 col-md-6 col-xl-3" *ngFor="let item of quickCards">
          <div class="card h-100 shadow-sm border-0">
            <div class="card-body">
              <div class="text-muted small">{{ item.label }}</div>
              <div class="fs-5 fw-semibold">{{ item.value }}</div>
            </div>
          </div>
        </div>
      </div>

      <div class="card shadow-sm border-0">
        <div class="card-body">
          <h5 class="mb-3">Committee shortcuts</h5>
          <div class="d-flex gap-2 flex-wrap">
            <a routerLink="/property-profiles/units" class="btn btn-outline-primary btn-sm">Units</a>
            <a routerLink="/renovations" class="btn btn-outline-primary btn-sm">Renovations</a>
            <a routerLink="/financial-reports/balance-sheet" class="btn btn-outline-primary btn-sm">Balance Sheet</a>
            <a routerLink="/financial-reports/cash-flow" class="btn btn-outline-primary btn-sm">Cash Flow</a>
            <a routerLink="/bank-reconciliation" class="btn btn-outline-primary btn-sm" *ngIf="auth.isManager()">Bank Reconciliation</a>
            <a routerLink="/reports" class="btn btn-outline-primary btn-sm" *ngIf="auth.isAdmin() || auth.isSupport()">Operations Reports</a>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [``]
})
export class CommitteeDashboardComponent {
  quickCards = [
    { label: 'Open Renovations', value: 'Review in Renovations module' },
    { label: 'Pending Expenses', value: 'Review in Expenses module' },
    { label: 'Aging Items', value: 'Review in Aging Report' },
    { label: 'Meter Trends', value: 'Review in Meter Readings' }
  ];

  constructor(public auth: AuthService) {}
}
