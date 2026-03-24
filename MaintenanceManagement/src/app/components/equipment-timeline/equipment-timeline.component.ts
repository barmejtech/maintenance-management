import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EquipmentService } from '../../services/equipment.service';
import { TaskOrderService } from '../../services/task-order.service';
import { Equipment, TaskOrder, TaskStatus, TaskPriority, MaintenanceType, EquipmentStatus } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';

@Component({
  selector: 'app-equipment-timeline',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './equipment-timeline.component.html',
  styleUrls: ['./equipment-timeline.component.css']
})
export class EquipmentTimelineComponent implements OnInit {
  equipmentList = signal<Equipment[]>([]);
  selectedEquipmentId = signal<string>('');
  timeline = signal<TaskOrder[]>([]);
  isLoadingEquipment = signal(true);
  isLoadingTimeline = signal(false);
  searchTerm = signal('');

  TaskStatus = TaskStatus;
  TaskPriority = TaskPriority;
  MaintenanceType = MaintenanceType;
  EquipmentStatus = EquipmentStatus;

  selectedEquipment = computed(() =>
    this.equipmentList().find(e => e.id === this.selectedEquipmentId())
  );

  filteredEquipment = computed(() => {
    const term = this.searchTerm().toLowerCase();
    if (!term) return this.equipmentList();
    return this.equipmentList().filter(e =>
      e.name.toLowerCase().includes(term) ||
      e.serialNumber.toLowerCase().includes(term) ||
      e.location.toLowerCase().includes(term)
    );
  });

  timelineStats = computed(() => {
    const tasks = this.timeline();
    return {
      total: tasks.length,
      completed: tasks.filter(t => t.status === TaskStatus.Completed).length,
      inProgress: tasks.filter(t => t.status === TaskStatus.InProgress).length,
      pending: tasks.filter(t => t.status === TaskStatus.Pending).length,
      cancelled: tasks.filter(t => t.status === TaskStatus.Cancelled).length,
    };
  });

  constructor(
    private equipmentService: EquipmentService,
    private taskService: TaskOrderService,
    public translation: TranslationService
  ) {}

  ngOnInit() {
    this.equipmentService.getAll().subscribe({
      next: (data) => { this.equipmentList.set(data); this.isLoadingEquipment.set(false); },
      error: () => this.isLoadingEquipment.set(false)
    });
  }

  selectEquipment(id: string) {
    this.selectedEquipmentId.set(id);
    this.isLoadingTimeline.set(true);
    this.timeline.set([]);
    this.taskService.getByEquipment(id).subscribe({
      next: (tasks) => {
        const sorted = [...tasks].sort((a, b) =>
          new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
        );
        this.timeline.set(sorted);
        this.isLoadingTimeline.set(false);
      },
      error: () => this.isLoadingTimeline.set(false)
    });
  }

  getStatusClass(s: TaskStatus): string {
    return ['bg-warning text-dark', 'bg-info text-dark', 'bg-success', 'bg-danger', 'bg-secondary'][s] ?? 'bg-secondary';
  }

  getStatusLabel(s: TaskStatus): string {
    return ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'][s] ?? 'Unknown';
  }

  getPriorityClass(p: TaskPriority): string {
    return ['secondary', 'info', 'warning', 'danger'][p] ?? 'secondary';
  }

  getPriorityLabel(p: TaskPriority): string {
    return ['Low', 'Medium', 'High', 'Critical'][p] ?? 'Unknown';
  }

  getMaintenanceTypeLabel(t: MaintenanceType): string {
    return ['Preventive', 'Corrective', 'Inspection', 'Emergency'][t] ?? 'Unknown';
  }

  getMaintenanceTypeIcon(t: MaintenanceType): string {
    return ['🛡️', '🔧', '🔍', '🚨'][t] ?? '🔧';
  }

  getEquipmentStatusClass(s: EquipmentStatus): string {
    return ['text-success', 'text-warning', 'text-danger', 'text-secondary'][s] ?? 'text-muted';
  }

  getEquipmentStatusLabel(s: EquipmentStatus): string {
    return ['Operational', 'Under Maintenance', 'Out of Service', 'Decommissioned'][s] ?? 'Unknown';
  }

  getTimelineDotClass(s: TaskStatus): string {
    return ['dot-warning', 'dot-info', 'dot-success', 'dot-danger', 'dot-secondary'][s] ?? 'dot-secondary';
  }
}
