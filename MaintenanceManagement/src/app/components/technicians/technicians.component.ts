import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TechnicianService } from '../../services/technician.service';
import { Technician, TechnicianStatus } from '../../models';

@Component({
  selector: 'app-technicians',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h2>👷 Technicians</h2>
        <a routerLink="/dashboard" class="btn-back">← Dashboard</a>
      </div>

      @if (isLoading()) {
        <div class="loading">Loading technicians...</div>
      } @else {
        <div class="card-grid">
          @for (tech of technicians(); track tech.id) {
            <div class="tech-card">
              <div class="tech-avatar">{{ tech.firstName[0] }}{{ tech.lastName[0] }}</div>
              <div class="tech-info">
                <h3>{{ tech.fullName }}</h3>
                <p class="spec">{{ tech.specialization }}</p>
                <p class="email">{{ tech.email }}</p>
                <p class="phone">{{ tech.phone }}</p>
                <span class="status-badge" [class]="getStatusClass(tech.status)">
                  {{ getStatusLabel(tech.status) }}
                </span>
              </div>
              @if (tech.latitude && tech.longitude) {
                <div class="location-info">
                  📍 {{ tech.latitude.toFixed(4) }}, {{ tech.longitude.toFixed(4) }}
                </div>
              }
            </div>
          }
          @empty {
            <div class="empty-state">No technicians found.</div>
          }
        </div>
      }
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .page-header h2 { font-size: 1.5rem; color: #333; }
    .btn-back { color: #0f3460; text-decoration: none; font-weight: 500; }
    .loading { text-align: center; padding: 2rem; color: #888; }
    .card-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(280px, 1fr)); gap: 1.5rem; }
    .tech-card {
      background: white;
      border-radius: 0.75rem;
      padding: 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.06);
    }
    .tech-avatar {
      width: 60px;
      height: 60px;
      background: #0f3460;
      color: white;
      border-radius: 50%;
      display: flex;
      align-items: center;
      justify-content: center;
      font-size: 1.2rem;
      font-weight: 700;
      margin-bottom: 1rem;
    }
    .tech-info h3 { font-size: 1.1rem; margin-bottom: 0.25rem; color: #333; }
    .spec { color: #666; font-size: 0.85rem; margin-bottom: 0.25rem; }
    .email, .phone { color: #888; font-size: 0.8rem; margin-bottom: 0.25rem; }
    .status-badge {
      display: inline-block;
      padding: 0.25rem 0.75rem;
      border-radius: 1rem;
      font-size: 0.75rem;
      font-weight: 600;
      margin-top: 0.5rem;
    }
    .status-available { background: #e8f5e9; color: #2e7d32; }
    .status-busy { background: #fff3e0; color: #e65100; }
    .status-on-leave { background: #e3f2fd; color: #1565c0; }
    .status-inactive { background: #f5f5f5; color: #616161; }
    .location-info { font-size: 0.75rem; color: #888; margin-top: 0.75rem; border-top: 1px solid #f0f0f0; padding-top: 0.5rem; }
    .empty-state { text-align: center; padding: 3rem; color: #888; background: white; border-radius: 0.75rem; }
  `]
})
export class TechniciansComponent implements OnInit {
  technicians = signal<Technician[]>([]);
  isLoading = signal(true);

  constructor(private service: TechnicianService) {}

  ngOnInit() {
    this.service.getAll().subscribe({
      next: (data) => { this.technicians.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  getStatusLabel(status: TechnicianStatus): string {
    return ['Available', 'Busy', 'On Leave', 'Inactive'][status];
  }

  getStatusClass(status: TechnicianStatus): string {
    return ['status-available', 'status-busy', 'status-on-leave', 'status-inactive'][status];
  }
}
