import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TaskOrderService } from '../../services/task-order.service';
import { TechnicianService } from '../../services/technician.service';
import { EquipmentService } from '../../services/equipment.service';
import { GroupService } from '../../services/group.service';
import { TaskOrder, TaskStatus, TaskPriority, MaintenanceType, Technician, Equipment, TechnicianGroup, CreateTaskOrderRequest } from '../../models';

const GPS_TIMEOUT_MS = 10_000;
const GPS_MAX_AGE_MS = 60_000;

@Component({
  selector: 'app-tasks',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './tasks.component.html',
  styleUrls: ['./tasks.component.css']
})
export class TasksComponent implements OnInit {
  tasks = signal<TaskOrder[]>([]);
  technicians = signal<Technician[]>([]);
  equipmentList = signal<Equipment[]>([]);
  groups = signal<TechnicianGroup[]>([]);
  isLoading = signal(true);
  filterStatus = signal<number | null>(null);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  gpsStatus = signal<'idle' | 'capturing' | 'success' | 'denied' | 'unavailable'>('idle');
  TaskStatus = TaskStatus;
  TaskPriority = TaskPriority;
  MaintenanceType = MaintenanceType;

  form: CreateTaskOrderRequest = {
    title: '', description: '', status: TaskStatus.Pending, priority: TaskPriority.Medium,
    maintenanceType: MaintenanceType.Preventive, scheduledDate: undefined, dueDate: undefined,
    notes: undefined, technicianId: undefined, groupId: undefined, equipmentId: undefined
  };
  private editingId = '';

  filteredTasks = () => {
    const s = this.filterStatus();
    return s === null ? this.tasks() : this.tasks().filter(t => t.status === s);
  };

  constructor(
    private service: TaskOrderService,
    private techService: TechnicianService,
    private eqService: EquipmentService,
    private groupService: GroupService
  ) {}

  ngOnInit() {
    this.load();
    this.techService.getAll().subscribe({ next: d => this.technicians.set(d), error: () => {} });
    this.eqService.getAll().subscribe({ next: d => this.equipmentList.set(d), error: () => {} });
    this.groupService.getAll().subscribe({ next: d => this.groups.set(d), error: () => {} });
  }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: (data) => { this.tasks.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.gpsStatus.set('idle');
    this.form = { title: '', description: '', status: TaskStatus.Pending, priority: TaskPriority.Medium, maintenanceType: MaintenanceType.Preventive };
    this.showModal.set(true);
  }

  openEdit(task: TaskOrder) {
    this.isEditing.set(true);
    this.editingId = task.id;
    this.gpsStatus.set('idle');
    this.form = {
      title: task.title, description: task.description, status: task.status,
      priority: task.priority, maintenanceType: task.maintenanceType,
      scheduledDate: task.scheduledDate?.substring(0, 10),
      dueDate: task.dueDate?.substring(0, 10),
      notes: task.notes ?? undefined,
      technicianId: task.technicianId ?? undefined,
      groupId: task.groupId ?? undefined,
      equipmentId: task.equipmentId ?? undefined
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  /** Capture browser GPS and push it to the assigned technician's location record. */
  private captureAndSendGps(technicianId: string): void {
    if (!navigator.geolocation) {
      this.gpsStatus.set('unavailable');
      return;
    }
    this.gpsStatus.set('capturing');
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        this.techService.updateLocation(technicianId, pos.coords.latitude, pos.coords.longitude)
          .subscribe({
            next: () => this.gpsStatus.set('success'),
            error: () => this.gpsStatus.set('unavailable')
          });
      },
      () => this.gpsStatus.set('denied'),
      { timeout: GPS_TIMEOUT_MS, maximumAge: GPS_MAX_AGE_MS }
    );
  }

  save() {
    this.isSaving.set(true);
    const dto = {
      ...this.form,
      status: Number(this.form.status),
      priority: Number(this.form.priority),
      maintenanceType: Number(this.form.maintenanceType),
      technicianId: this.form.technicianId || undefined,
      groupId: this.form.groupId || undefined,
      equipmentId: this.form.equipmentId || undefined
    };
    const obs = this.isEditing()
      ? this.service.update(this.editingId, dto)
      : this.service.create(dto);
    obs.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showModal.set(false);
        this.load();
        // Capture GPS when a technician is dispatched (task set to InProgress)
        if (dto.status === TaskStatus.InProgress && dto.technicianId) {
          this.captureAndSendGps(dto.technicianId);
        }
      },
      error: () => this.isSaving.set(false)
    });
  }

  delete(id: string) {
    if (!confirm('Delete this work order?')) return;
    this.service.delete(id).subscribe({ next: () => this.load(), error: () => {} });
  }

  getTechnicianLocation(technicianId?: string): Technician | undefined {
    if (!technicianId) return undefined;
    return this.technicians().find(t => t.id === technicianId);
  }

  isInProgressWithTechnician(): boolean {
    return +this.form.status === TaskStatus.InProgress && !!this.form.technicianId;
  }

  getPriorityLabel(p: TaskPriority): string { return ['Low', 'Medium', 'High', 'Critical'][p]; }
  getPriorityClass(p: TaskPriority): string { return ['bg-secondary', 'bg-info text-dark', 'bg-warning text-dark', 'bg-danger'][p]; }
  getStatusLabel(s: TaskStatus): string { return ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'][s]; }
  getStatusClass(s: TaskStatus): string { return ['bg-warning text-dark', 'bg-info text-dark', 'bg-success', 'bg-danger', 'bg-secondary'][s]; }
}
