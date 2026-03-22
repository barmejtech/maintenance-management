import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TaskOrderService } from '../../services/api.service';
import { TaskOrder, TaskStatus, TaskPriority } from '../../models';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h2>✅ Work Orders</h2>
        <a routerLink="/dashboard" class="btn-back">← Dashboard</a>
      </div>

      <div class="filter-bar">
        <button (click)="filterStatus.set(null)" [class.active]="filterStatus() === null" class="filter-btn">All</button>
        <button (click)="filterStatus.set(0)" [class.active]="filterStatus() === 0" class="filter-btn">Pending</button>
        <button (click)="filterStatus.set(1)" [class.active]="filterStatus() === 1" class="filter-btn">In Progress</button>
        <button (click)="filterStatus.set(2)" [class.active]="filterStatus() === 2" class="filter-btn">Completed</button>
      </div>

      @if (isLoading()) {
        <div class="loading">Loading work orders...</div>
      } @else {
        <div class="task-list">
          @for (task of filteredTasks(); track task.id) {
            <div class="task-card" [class]="'priority-' + task.priority">
              <div class="task-header">
                <h3>{{ task.title }}</h3>
                <div class="task-badges">
                  <span class="badge priority" [class]="getPriorityClass(task.priority)">
                    {{ getPriorityLabel(task.priority) }}
                  </span>
                  <span class="badge status" [class]="getStatusClass(task.status)">
                    {{ getStatusLabel(task.status) }}
                  </span>
                </div>
              </div>
              <p class="task-desc">{{ task.description }}</p>
              <div class="task-meta">
                @if (task.technicianName) {
                  <span>👷 {{ task.technicianName }}</span>
                }
                @if (task.groupName) {
                  <span>👥 {{ task.groupName }}</span>
                }
                @if (task.equipmentName) {
                  <span>🔩 {{ task.equipmentName }}</span>
                }
                @if (task.dueDate) {
                  <span>📅 Due: {{ task.dueDate | date:'mediumDate' }}</span>
                }
              </div>
            </div>
          }
          @empty {
            <div class="empty-state">No work orders found.</div>
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
    .filter-bar { display: flex; gap: 0.5rem; margin-bottom: 1.5rem; flex-wrap: wrap; }
    .filter-btn {
      padding: 0.4rem 1rem;
      border: 2px solid #e0e0e0;
      border-radius: 2rem;
      background: white;
      cursor: pointer;
      font-size: 0.85rem;
      font-weight: 500;
      transition: all 0.2s;
    }
    .filter-btn.active { border-color: #0f3460; background: #0f3460; color: white; }
    .loading { text-align: center; padding: 2rem; color: #888; }
    .task-list { display: flex; flex-direction: column; gap: 1rem; }
    .task-card {
      background: white;
      border-radius: 0.75rem;
      padding: 1.25rem 1.5rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.06);
      border-left: 4px solid #e0e0e0;
    }
    .task-card.priority-3 { border-left-color: #e74c3c; }
    .task-card.priority-2 { border-left-color: #e67e22; }
    .task-card.priority-1 { border-left-color: #3498db; }
    .task-card.priority-0 { border-left-color: #95a5a6; }
    .task-header { display: flex; justify-content: space-between; align-items: flex-start; margin-bottom: 0.5rem; }
    .task-header h3 { font-size: 1rem; color: #333; }
    .task-badges { display: flex; gap: 0.5rem; flex-wrap: wrap; }
    .badge {
      padding: 0.2rem 0.6rem;
      border-radius: 1rem;
      font-size: 0.75rem;
      font-weight: 600;
    }
    .priority.p-critical { background: #fde8e8; color: #c0392b; }
    .priority.p-high { background: #fef0e0; color: #e67e22; }
    .priority.p-medium { background: #eaf4fd; color: #2980b9; }
    .priority.p-low { background: #f0f0f0; color: #666; }
    .status.s-pending { background: #fff3cd; color: #856404; }
    .status.s-inprogress { background: #cce5ff; color: #004085; }
    .status.s-completed { background: #d4edda; color: #155724; }
    .status.s-cancelled { background: #f8d7da; color: #721c24; }
    .task-desc { color: #666; font-size: 0.9rem; margin-bottom: 0.75rem; }
    .task-meta { display: flex; flex-wrap: wrap; gap: 1rem; font-size: 0.8rem; color: #888; }
    .empty-state { text-align: center; padding: 3rem; color: #888; background: white; border-radius: 0.75rem; }
  `]
})
export class TasksComponent implements OnInit {
  tasks = signal<TaskOrder[]>([]);
  isLoading = signal(true);
  filterStatus = signal<number | null>(null);

  filteredTasks = () => {
    const s = this.filterStatus();
    return s === null ? this.tasks() : this.tasks().filter(t => t.status === s);
  };

  constructor(private service: TaskOrderService) {}

  ngOnInit() {
    this.service.getAll().subscribe({
      next: (data) => { this.tasks.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  getPriorityLabel(p: TaskPriority): string { return ['Low', 'Medium', 'High', 'Critical'][p]; }
  getPriorityClass(p: TaskPriority): string { return ['p-low', 'p-medium', 'p-high', 'p-critical'][p]; }
  getStatusLabel(s: TaskStatus): string { return ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'][s]; }
  getStatusClass(s: TaskStatus): string { return ['s-pending', 's-inprogress', 's-completed', 's-cancelled', 's-onhold'][s]; }
}
