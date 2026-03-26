import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { MaintenanceScheduleService } from '../../services/maintenance-schedule.service';
import { EquipmentService } from '../../services/equipment.service';
import { TechnicianService } from '../../services/technician.service';
import { GroupService } from '../../services/group.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { AuthService } from '../../services/auth.service';
import {
  MaintenanceSchedule, CreateMaintenanceScheduleRequest, UpdateMaintenanceScheduleRequest,
  MaintenanceType, ScheduleFrequency, Equipment, Technician, TechnicianGroup
} from '../../models';

@Component({
  selector: 'app-maintenance-schedules',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './maintenance-schedules.component.html',
  styleUrls: ['./maintenance-schedules.component.css']
})
export class MaintenanceSchedulesComponent implements OnInit {
  schedules = signal<MaintenanceSchedule[]>([]);
  currentPage = signal(1);
  readonly pageSize = 9;
  equipment = signal<Equipment[]>([]);
  technicians = signal<Technician[]>([]);
  groups = signal<TechnicianGroup[]>([]);
  isLoading = signal(true);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  activeTab = signal<'all' | 'active' | 'overdue' | 'soon'>('all');

  MaintenanceType = MaintenanceType;
  ScheduleFrequency = ScheduleFrequency;

  form: {
    name: string; description: string; maintenanceType: MaintenanceType;
    frequency: ScheduleFrequency; frequencyValue: number;
    lastExecutedAt: string; nextDueAt: string; isActive: boolean; notes: string;
    equipmentId: string; assignedTechnicianId: string; assignedGroupId: string;
  } = {
    name: '', description: '', maintenanceType: MaintenanceType.Preventive,
    frequency: ScheduleFrequency.Monthly, frequencyValue: 1,
    lastExecutedAt: '', nextDueAt: '', isActive: true, notes: '',
    equipmentId: '', assignedTechnicianId: '', assignedGroupId: ''
  };
  private editingId = '';

  constructor(
    private service: MaintenanceScheduleService,
    private equipmentService: EquipmentService,
    private technicianService: TechnicianService,
    private groupService: GroupService,
    private translation: TranslationService,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.load();
    this.equipmentService.getAll().subscribe({ next: d => this.equipment.set(d) });
    this.technicianService.getAll().subscribe({ next: d => this.technicians.set(d) });
    this.groupService.getAll().subscribe({ next: d => this.groups.set(d) });
  }

  load() {
    this.isLoading.set(true);
    const obs = this.activeTab() === 'active'
      ? this.service.getActive()
      : this.activeTab() === 'overdue'
      ? this.service.getOverdue()
      : this.activeTab() === 'soon'
      ? this.service.getDueSoon(7)
      : this.service.getAll();

    obs.subscribe({
      next: (data) => { this.schedules.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  setTab(tab: 'all' | 'active' | 'overdue' | 'soon') {
    this.activeTab.set(tab);
    this.currentPage.set(1);
    this.load();
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = {
      name: '', description: '', maintenanceType: MaintenanceType.Preventive,
      frequency: ScheduleFrequency.Monthly, frequencyValue: 1,
      lastExecutedAt: '', nextDueAt: '', isActive: true, notes: '',
      equipmentId: '', assignedTechnicianId: '', assignedGroupId: ''
    };
    this.showModal.set(true);
  }

  openEdit(s: MaintenanceSchedule) {
    this.isEditing.set(true);
    this.editingId = s.id;
    this.form = {
      name: s.name, description: s.description, maintenanceType: s.maintenanceType,
      frequency: s.frequency, frequencyValue: s.frequencyValue,
      lastExecutedAt: s.lastExecutedAt ? s.lastExecutedAt.substring(0, 10) : '',
      nextDueAt: s.nextDueAt ? s.nextDueAt.substring(0, 10) : '',
      isActive: s.isActive, notes: s.notes ?? '',
      equipmentId: s.equipmentId ?? '',
      assignedTechnicianId: s.assignedTechnicianId ?? '',
      assignedGroupId: s.assignedGroupId ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    if (this.isEditing()) {
      const dto: UpdateMaintenanceScheduleRequest = {
        name: this.form.name, description: this.form.description,
        maintenanceType: this.form.maintenanceType, frequency: this.form.frequency,
        frequencyValue: this.form.frequencyValue,
        lastExecutedAt: this.form.lastExecutedAt || undefined,
        nextDueAt: this.form.nextDueAt || undefined,
        isActive: this.form.isActive, notes: this.form.notes || undefined,
        equipmentId: this.form.equipmentId || undefined,
        assignedTechnicianId: this.form.assignedTechnicianId || undefined,
        assignedGroupId: this.form.assignedGroupId || undefined
      };
      this.service.update(this.editingId, dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    } else {
      const dto: CreateMaintenanceScheduleRequest = {
        name: this.form.name, description: this.form.description,
        maintenanceType: this.form.maintenanceType, frequency: this.form.frequency,
        frequencyValue: this.form.frequencyValue,
        nextDueAt: this.form.nextDueAt || undefined,
        isActive: this.form.isActive, notes: this.form.notes || undefined,
        equipmentId: this.form.equipmentId || undefined,
        assignedTechnicianId: this.form.assignedTechnicianId || undefined,
        assignedGroupId: this.form.assignedGroupId || undefined
      };
      this.service.create(dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    }
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('maintenanceSchedules.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => this.load() });
  }

  getFrequencyLabel(f: ScheduleFrequency): string {
    const map: Record<number, string> = {
      [ScheduleFrequency.Daily]: this.translation.translate('maintenanceSchedules.frequency.daily'),
      [ScheduleFrequency.Weekly]: this.translation.translate('maintenanceSchedules.frequency.weekly'),
      [ScheduleFrequency.BiWeekly]: this.translation.translate('maintenanceSchedules.frequency.biWeekly'),
      [ScheduleFrequency.Monthly]: this.translation.translate('maintenanceSchedules.frequency.monthly'),
      [ScheduleFrequency.Quarterly]: this.translation.translate('maintenanceSchedules.frequency.quarterly'),
      [ScheduleFrequency.SemiAnnual]: this.translation.translate('maintenanceSchedules.frequency.semiAnnual'),
      [ScheduleFrequency.Annual]: this.translation.translate('maintenanceSchedules.frequency.annual'),
    };
    return map[f] ?? String(f);
  }

  getTypeLabel(t: MaintenanceType): string {
    const map: Record<number, string> = {
      [MaintenanceType.Preventive]: this.translation.translate('maintenanceSchedules.type.preventive'),
      [MaintenanceType.Corrective]: this.translation.translate('maintenanceSchedules.type.corrective'),
      [MaintenanceType.Inspection]: this.translation.translate('maintenanceSchedules.type.inspection'),
      [MaintenanceType.Emergency]: this.translation.translate('maintenanceSchedules.type.emergency'),
    };
    return map[t] ?? String(t);
  }

  isOverdue(s: MaintenanceSchedule): boolean {
    return !!s.nextDueAt && new Date(s.nextDueAt) < new Date();
  }

  isDueSoon(s: MaintenanceSchedule): boolean {
    if (!s.nextDueAt) return false;
    const due = new Date(s.nextDueAt);
    const now = new Date();
    const diff = (due.getTime() - now.getTime()) / (1000 * 60 * 60 * 24);
    return diff >= 0 && diff <= 7;
  }

  getTypeIcon(t: MaintenanceType): string {
    const map: Record<number, string> = {
      [MaintenanceType.Preventive]: 'bi-shield-check',
      [MaintenanceType.Corrective]: 'bi-wrench-adjustable',
      [MaintenanceType.Inspection]: 'bi-search',
      [MaintenanceType.Emergency]: 'bi-lightning-charge',
    };
    return map[t] ?? 'bi-tools';
  }

  getTypeColor(t: MaintenanceType): string {
    const map: Record<number, string> = {
      [MaintenanceType.Preventive]: 'text-primary bg-primary-subtle',
      [MaintenanceType.Corrective]: 'text-danger bg-danger-subtle',
      [MaintenanceType.Inspection]: 'text-warning bg-warning-subtle',
      [MaintenanceType.Emergency]: 'text-danger bg-danger-subtle',
    };
    return map[t] ?? 'text-secondary bg-secondary-subtle';
  }
}
