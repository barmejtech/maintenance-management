import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EquipmentDigitalTwinService } from '../../services/equipment-digital-twin.service';
import { EquipmentService } from '../../services/equipment.service';
import { EquipmentDigitalTwin, Equipment, EquipmentStatus, UpsertDigitalTwinRequest } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';

@Component({
  selector: 'app-equipment-digital-twin',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe],
  templateUrl: './equipment-digital-twin.component.html',
  styleUrls: ['./equipment-digital-twin.component.css']
})
export class EquipmentDigitalTwinComponent implements OnInit {
  twins = signal<EquipmentDigitalTwin[]>([]);
  equipment = signal<Equipment[]>([]);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  isSyncing = signal<string | null>(null);
  isLoading = signal(true);
  EquipmentStatus = EquipmentStatus;

  form: UpsertDigitalTwinRequest = {
    equipmentId: '',
    currentStatus: EquipmentStatus.Operational,
    wearPercentage: 0,
    performanceScore: 100,
    temperatureCelsius: undefined,
    usageHours: undefined,
    lastKnownIssue: '',
    simulationNotes: ''
  };

  constructor(
    private service: EquipmentDigitalTwinService,
    private equipmentService: EquipmentService,
    private translation: TranslationService
  ) {}

  ngOnInit() {
    this.load();
    this.equipmentService.getAll().subscribe({ next: e => this.equipment.set(e), error: () => {} });
  }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: d => { this.twins.set(d); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.form = {
      equipmentId: '',
      currentStatus: EquipmentStatus.Operational,
      wearPercentage: 0,
      performanceScore: 100,
      temperatureCelsius: undefined,
      usageHours: undefined,
      lastKnownIssue: '',
      simulationNotes: ''
    };
    this.showModal.set(true);
  }

  openEdit(twin: EquipmentDigitalTwin) {
    this.isEditing.set(true);
    this.form = {
      equipmentId: twin.equipmentId,
      currentStatus: twin.currentStatus,
      wearPercentage: twin.wearPercentage,
      performanceScore: twin.performanceScore,
      temperatureCelsius: twin.temperatureCelsius,
      usageHours: twin.usageHours,
      lastKnownIssue: twin.lastKnownIssue ?? '',
      simulationNotes: twin.simulationNotes ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    const dto: UpsertDigitalTwinRequest = {
      ...this.form,
      currentStatus: Number(this.form.currentStatus) as EquipmentStatus,
      temperatureCelsius: this.form.temperatureCelsius != null && this.form.temperatureCelsius !== undefined && String(this.form.temperatureCelsius) !== '' ? Number(this.form.temperatureCelsius) : undefined,
      usageHours: this.form.usageHours != null && this.form.usageHours !== undefined && String(this.form.usageHours) !== '' ? Number(this.form.usageHours) : undefined
    };
    this.service.upsert(dto).subscribe({
      next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
      error: () => this.isSaving.set(false)
    });
  }

  syncFromEquipment(equipmentId: string) {
    this.isSyncing.set(equipmentId);
    this.service.syncFromEquipment(equipmentId).subscribe({
      next: () => { this.isSyncing.set(null); this.load(); },
      error: () => this.isSyncing.set(null)
    });
  }

  syncAll() {
    const eqs = this.equipment();
    if (!eqs.length) return;
    let pending = eqs.length;
    eqs.forEach(eq => {
      this.service.syncFromEquipment(eq.id).subscribe({
        next: () => { pending--; if (pending === 0) this.load(); },
        error: () => { pending--; if (pending === 0) this.load(); }
      });
    });
  }

  getWearClass(wear: number): string {
    if (wear >= 75) return 'bg-danger';
    if (wear >= 50) return 'bg-warning';
    if (wear >= 25) return 'bg-info';
    return 'bg-success';
  }

  getPerformanceClass(score: number): string {
    if (score >= 75) return 'text-success';
    if (score >= 50) return 'text-warning';
    if (score >= 25) return 'text-info';
    return 'text-danger';
  }

  getStatusLabel(s: EquipmentStatus): string {
    const keys = ['operational', 'underMaintenance', 'outOfService', 'decommissioned'];
    return this.translation.translate(`digitalTwin.status.${keys[s]}`);
  }
  getStatusClass(s: EquipmentStatus): string { return ['bg-success', 'bg-warning text-dark', 'bg-danger', 'bg-secondary'][s]; }

  getEquipmentName(id: string): string {
    return this.equipment().find(e => e.id === id)?.name ?? id;
  }
}
