import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EquipmentService } from '../../services/api.service';
import { Equipment, EquipmentStatus } from '../../models';

@Component({
  selector: 'app-equipment',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h2>🔩 Equipment</h2>
        <a routerLink="/dashboard" class="btn-back">← Dashboard</a>
      </div>
      <div class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>Name</th>
              <th>Serial No.</th>
              <th>Manufacturer</th>
              <th>Location</th>
              <th>Status</th>
              <th>Next Maintenance</th>
            </tr>
          </thead>
          <tbody>
            @for (eq of equipment(); track eq.id) {
              <tr>
                <td><strong>{{ eq.name }}</strong></td>
                <td>{{ eq.serialNumber }}</td>
                <td>{{ eq.manufacturer }}</td>
                <td>{{ eq.location }}</td>
                <td><span class="badge" [class]="getStatusClass(eq.status)">{{ getStatusLabel(eq.status) }}</span></td>
                <td>{{ eq.nextMaintenanceDate ? (eq.nextMaintenanceDate | date:'mediumDate') : '—' }}</td>
              </tr>
            }
          </tbody>
        </table>
        @if (equipment().length === 0) {
          <div class="empty-state">No equipment found.</div>
        }
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .page-header h2 { font-size: 1.5rem; color: #333; }
    .btn-back { color: #0f3460; text-decoration: none; font-weight: 500; }
    .table-container { background: white; border-radius: 0.75rem; box-shadow: 0 2px 8px rgba(0,0,0,0.06); overflow: hidden; }
    .data-table { width: 100%; border-collapse: collapse; }
    .data-table th { background: #f8f9fa; padding: 0.875rem 1rem; text-align: left; font-size: 0.85rem; font-weight: 600; color: #555; border-bottom: 1px solid #e0e0e0; }
    .data-table td { padding: 0.875rem 1rem; border-bottom: 1px solid #f0f0f0; font-size: 0.9rem; color: #333; }
    .data-table tr:last-child td { border-bottom: none; }
    .badge { padding: 0.25rem 0.75rem; border-radius: 1rem; font-size: 0.75rem; font-weight: 600; }
    .s-operational { background: #d4edda; color: #155724; }
    .s-maintenance { background: #fff3cd; color: #856404; }
    .s-out-of-service { background: #f8d7da; color: #721c24; }
    .s-decommissioned { background: #f5f5f5; color: #616161; }
    .empty-state { text-align: center; padding: 3rem; color: #888; }
  `]
})
export class EquipmentComponent implements OnInit {
  equipment = signal<Equipment[]>([]);

  constructor(private service: EquipmentService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.equipment.set(d), error: () => {} });
  }

  getStatusLabel(s: EquipmentStatus): string { return ['Operational', 'Under Maintenance', 'Out of Service', 'Decommissioned'][s]; }
  getStatusClass(s: EquipmentStatus): string { return ['s-operational', 's-maintenance', 's-out-of-service', 's-decommissioned'][s]; }
}
