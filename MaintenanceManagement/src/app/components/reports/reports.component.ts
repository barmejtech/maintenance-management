import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReportService } from '../../services/api.service';
import { MaintenanceReport } from '../../models';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h2>📄 Maintenance Reports</h2>
        <a routerLink="/dashboard" class="btn-back">← Dashboard</a>
      </div>
      <div class="report-list">
        @for (report of reports(); track report.id) {
          <div class="report-card">
            <div class="report-header">
              <h3>{{ report.title }}</h3>
              <span class="report-date">{{ report.reportDate | date:'mediumDate' }}</span>
            </div>
            <p class="tech-name">👷 {{ report.technicianName }}</p>
            <p class="content">{{ report.content | slice:0:200 }}{{ report.content.length > 200 ? '...' : '' }}</p>
            @if (report.taskTitle) {
              <p class="task-ref">🔗 Task: {{ report.taskTitle }}</p>
            }
            <div class="report-meta">
              @if (report.laborHours) {
                <span>⏱ {{ report.laborHours }}h</span>
              }
              @if (report.materialCost) {
                <span>💰 \${{ report.materialCost }}</span>
              }
            </div>
            @if (report.beforePhotoUrl || report.afterPhotoUrl) {
              <div class="photos">
                @if (report.beforePhotoUrl) {
                  <div class="photo-label">Before: <a [href]="report.beforePhotoUrl" target="_blank">📷 View</a></div>
                }
                @if (report.afterPhotoUrl) {
                  <div class="photo-label">After: <a [href]="report.afterPhotoUrl" target="_blank">📷 View</a></div>
                }
              </div>
            }
          </div>
        }
        @empty {
          <div class="empty-state">No reports found.</div>
        }
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .page-header h2 { font-size: 1.5rem; color: #333; }
    .btn-back { color: #0f3460; text-decoration: none; font-weight: 500; }
    .report-list { display: flex; flex-direction: column; gap: 1rem; }
    .report-card { background: white; border-radius: 0.75rem; padding: 1.5rem; box-shadow: 0 2px 8px rgba(0,0,0,0.06); }
    .report-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.5rem; }
    .report-header h3 { color: #333; font-size: 1rem; }
    .report-date { color: #888; font-size: 0.85rem; }
    .tech-name { color: #555; font-size: 0.9rem; margin-bottom: 0.5rem; }
    .content { color: #666; font-size: 0.9rem; margin-bottom: 0.75rem; }
    .task-ref { color: #0f3460; font-size: 0.85rem; margin-bottom: 0.5rem; }
    .report-meta { display: flex; gap: 1rem; font-size: 0.8rem; color: #888; margin-bottom: 0.5rem; }
    .photos { display: flex; gap: 1rem; font-size: 0.85rem; }
    .photo-label a { color: #0f3460; text-decoration: none; }
    .empty-state { text-align: center; padding: 3rem; color: #888; background: white; border-radius: 0.75rem; }
  `]
})
export class ReportsComponent implements OnInit {
  reports = signal<MaintenanceReport[]>([]);

  constructor(private service: ReportService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.reports.set(d), error: () => {} });
  }
}
