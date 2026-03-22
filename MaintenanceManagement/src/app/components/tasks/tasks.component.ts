import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TaskOrderService } from '../../services/task-order.service';
import { TaskOrder, TaskStatus, TaskPriority } from '../../models';

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
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
