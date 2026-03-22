import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HVACService } from '../../services/api.service';
import { HVACSystem } from '../../models';

@Component({
  selector: 'app-hvac',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h2>❄️ HVAC Systems</h2>
        <a routerLink="/dashboard" class="btn-back">← Dashboard</a>
      </div>
      <div class="card-grid">
        @for (hvac of systems(); track hvac.id) {
          <div class="hvac-card">
            <div class="hvac-icon">❄️</div>
            <h3>{{ hvac.name }}</h3>
            <div class="hvac-details">
              <div class="detail-row"><span class="label">Type:</span> {{ hvac.systemType }}</div>
              <div class="detail-row"><span class="label">Brand:</span> {{ hvac.brand }}</div>
              <div class="detail-row"><span class="label">Model:</span> {{ hvac.model }}</div>
              <div class="detail-row"><span class="label">Capacity:</span> {{ hvac.capacity }} {{ hvac.capacityUnit }}</div>
              <div class="detail-row"><span class="label">Refrigerant:</span> {{ hvac.refrigerantType }}</div>
              <div class="detail-row"><span class="label">Location:</span> {{ hvac.location }}</div>
              @if (hvac.nextInspectionDate) {
                <div class="detail-row">
                  <span class="label">Next Inspection:</span>
                  {{ hvac.nextInspectionDate | date:'mediumDate' }}
                </div>
              }
            </div>
          </div>
        }
        @empty {
          <div class="empty-state">No HVAC systems found.</div>
        }
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .page-header h2 { font-size: 1.5rem; color: #333; }
    .btn-back { color: #0f3460; text-decoration: none; font-weight: 500; }
    .card-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 1.5rem; }
    .hvac-card { background: white; border-radius: 0.75rem; padding: 1.5rem; box-shadow: 0 2px 8px rgba(0,0,0,0.06); }
    .hvac-icon { font-size: 2rem; margin-bottom: 0.75rem; }
    .hvac-card h3 { color: #333; margin-bottom: 1rem; }
    .detail-row { display: flex; gap: 0.5rem; font-size: 0.85rem; margin-bottom: 0.4rem; }
    .label { color: #888; min-width: 90px; }
    .empty-state { text-align: center; padding: 3rem; color: #888; background: white; border-radius: 0.75rem; }
  `]
})
export class HVACComponent implements OnInit {
  systems = signal<HVACSystem[]>([]);

  constructor(private service: HVACService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.systems.set(d), error: () => {} });
  }
}
