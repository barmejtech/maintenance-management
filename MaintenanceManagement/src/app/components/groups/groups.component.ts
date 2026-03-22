import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { GroupService } from '../../services/api.service';
import { TechnicianGroup } from '../../models';

@Component({
  selector: 'app-groups',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h2>👥 Technician Groups</h2>
        <a routerLink="/dashboard" class="btn-back">← Dashboard</a>
      </div>
      <div class="card-grid">
        @for (group of groups(); track group.id) {
          <div class="group-card">
            <div class="group-icon">👥</div>
            <h3>{{ group.name }}</h3>
            <p>{{ group.description }}</p>
            <div class="member-count">{{ group.memberCount }} members</div>
          </div>
        }
        @empty {
          <div class="empty-state">No groups found.</div>
        }
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .page-header h2 { font-size: 1.5rem; color: #333; }
    .btn-back { color: #0f3460; text-decoration: none; font-weight: 500; }
    .card-grid { display: grid; grid-template-columns: repeat(auto-fill, minmax(250px, 1fr)); gap: 1.5rem; }
    .group-card { background: white; border-radius: 0.75rem; padding: 1.5rem; box-shadow: 0 2px 8px rgba(0,0,0,0.06); text-align: center; }
    .group-icon { font-size: 2.5rem; margin-bottom: 0.75rem; }
    .group-card h3 { color: #333; margin-bottom: 0.5rem; }
    .group-card p { color: #666; font-size: 0.9rem; margin-bottom: 1rem; }
    .member-count { background: #e3f2fd; color: #1565c0; padding: 0.3rem 0.8rem; border-radius: 1rem; font-size: 0.8rem; font-weight: 600; display: inline-block; }
    .empty-state { text-align: center; padding: 3rem; color: #888; background: white; border-radius: 0.75rem; }
  `]
})
export class GroupsComponent implements OnInit {
  groups = signal<TechnicianGroup[]>([]);

  constructor(private service: GroupService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.groups.set(d), error: () => {} });
  }
}
