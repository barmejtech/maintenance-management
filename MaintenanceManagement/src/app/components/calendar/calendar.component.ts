import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TaskOrderService } from '../../services/task-order.service';
import { TaskOrder, TaskStatus, TaskPriority } from '../../models';

interface CalendarDay {
  date: Date;
  isCurrentMonth: boolean;
  isToday: boolean;
  tasks: TaskOrder[];
}

@Component({
  selector: 'app-calendar',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent implements OnInit {
  currentDate = signal(new Date());
  tasks = signal<TaskOrder[]>([]);
  isLoading = signal(true);
  selectedTask = signal<TaskOrder | null>(null);

  TaskStatus = TaskStatus;
  TaskPriority = TaskPriority;

  monthLabel = computed(() => {
    const d = this.currentDate();
    return d.toLocaleDateString('en-US', { month: 'long', year: 'numeric' });
  });

  calendarDays = computed<CalendarDay[]>(() => {
    const d = this.currentDate();
    const year = d.getFullYear();
    const month = d.getMonth();

    const firstDay = new Date(year, month, 1);
    const lastDay = new Date(year, month + 1, 0);

    const days: CalendarDay[] = [];
    const today = new Date();

    // Leading days from previous month
    const startDow = firstDay.getDay(); // 0 = Sunday
    for (let i = startDow - 1; i >= 0; i--) {
      const date = new Date(year, month, -i);
      days.push({ date, isCurrentMonth: false, isToday: false, tasks: [] });
    }

    // Current month days
    for (let day = 1; day <= lastDay.getDate(); day++) {
      const date = new Date(year, month, day);
      const isToday =
        date.getDate() === today.getDate() &&
        date.getMonth() === today.getMonth() &&
        date.getFullYear() === today.getFullYear();

      const dayTasks = this.tasks().filter(t => {
        if (!t.scheduledDate) return false;
        const scheduled = new Date(t.scheduledDate);
        return (
          scheduled.getDate() === date.getDate() &&
          scheduled.getMonth() === date.getMonth() &&
          scheduled.getFullYear() === date.getFullYear()
        );
      });

      days.push({ date, isCurrentMonth: true, isToday, tasks: dayTasks });
    }

    // Trailing days to complete the last row
    const remaining = 42 - days.length;
    for (let i = 1; i <= remaining; i++) {
      const date = new Date(year, month + 1, i);
      days.push({ date, isCurrentMonth: false, isToday: false, tasks: [] });
    }

    return days;
  });

  constructor(private taskService: TaskOrderService) {}

  ngOnInit() {
    this.loadMonth();
  }

  loadMonth() {
    const d = this.currentDate();
    const from = new Date(d.getFullYear(), d.getMonth(), 1).toISOString();
    const to = new Date(d.getFullYear(), d.getMonth() + 1, 0, 23, 59, 59).toISOString();
    this.isLoading.set(true);
    this.taskService.getCalendar(from, to).subscribe({
      next: data => { this.tasks.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  prevMonth() {
    const d = this.currentDate();
    this.currentDate.set(new Date(d.getFullYear(), d.getMonth() - 1, 1));
    this.loadMonth();
  }

  nextMonth() {
    const d = this.currentDate();
    this.currentDate.set(new Date(d.getFullYear(), d.getMonth() + 1, 1));
    this.loadMonth();
  }

  goToToday() {
    this.currentDate.set(new Date());
    this.loadMonth();
  }

  openTask(task: TaskOrder) {
    this.selectedTask.set(task);
  }

  closeTask() {
    this.selectedTask.set(null);
  }

  getStatusLabel(s: TaskStatus): string {
    return ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'][s] ?? 'Unknown';
  }

  getStatusClass(s: TaskStatus): string {
    return ['bg-warning text-dark', 'bg-info text-dark', 'bg-success', 'bg-danger', 'bg-secondary'][s] ?? '';
  }

  getPriorityLabel(p: TaskPriority): string {
    return ['Low', 'Medium', 'High', 'Critical'][p] ?? '';
  }

  getPriorityClass(p: TaskPriority): string {
    return ['task-low', 'task-medium', 'task-high', 'task-critical'][p] ?? '';
  }
}
