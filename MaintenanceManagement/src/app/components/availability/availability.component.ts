import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AvailabilityService } from '../../services/api.service';
import { Availability } from '../../models';

@Component({
  selector: 'app-availability',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h2>📅 Technician Availability</h2>
        <a routerLink="/dashboard" class="btn-back">← Dashboard</a>
      </div>
      <div class="avail-list">
        @for (avail of availabilities(); track avail.id) {
          <div class="avail-card" [class.available]="avail.isAvailable" [class.unavailable]="!avail.isAvailable">
            <div class="avail-indicator"></div>
            <div class="avail-info">
              <h3>{{ avail.technicianName || 'Unknown Technician' }}</h3>
              <div class="avail-time">
                <span>{{ avail.startTime | date:'short' }}</span>
                <span class="arrow">→</span>
                <span>{{ avail.endTime | date:'short' }}</span>
              </div>
              @if (avail.notes) {
                <p class="avail-notes">{{ avail.notes }}</p>
              }
            </div>
            <span class="avail-badge" [class.a-available]="avail.isAvailable" [class.a-unavailable]="!avail.isAvailable">
              {{ avail.isAvailable ? 'Available' : 'Unavailable' }}
            </span>
          </div>
        }
        @empty {
          <div class="empty-state">No availability records found.</div>
        }
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .page-header h2 { font-size: 1.5rem; color: #333; }
    .btn-back { color: #0f3460; text-decoration: none; font-weight: 500; }
    .avail-list { display: flex; flex-direction: column; gap: 0.75rem; }
    .avail-card {
      background: white;
      border-radius: 0.75rem;
      padding: 1rem 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.06);
      display: flex;
      align-items: center;
      gap: 1rem;
      border-left: 4px solid #ccc;
    }
    .avail-card.available { border-left-color: #27ae60; }
    .avail-card.unavailable { border-left-color: #e74c3c; }
    .avail-info { flex: 1; }
    .avail-info h3 { font-size: 0.95rem; color: #333; margin-bottom: 0.25rem; }
    .avail-time { display: flex; align-items: center; gap: 0.5rem; font-size: 0.85rem; color: #666; }
    .arrow { color: #888; }
    .avail-notes { color: #888; font-size: 0.8rem; margin-top: 0.25rem; }
    .avail-badge { padding: 0.25rem 0.75rem; border-radius: 1rem; font-size: 0.75rem; font-weight: 600; }
    .a-available { background: #d4edda; color: #155724; }
    .a-unavailable { background: #f8d7da; color: #721c24; }
    .empty-state { text-align: center; padding: 3rem; color: #888; background: white; border-radius: 0.75rem; }
  `]
})
export class AvailabilityComponent implements OnInit {
  availabilities = signal<Availability[]>([]);

  constructor(private service: AvailabilityService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.availabilities.set(d), error: () => {} });
  }
}
