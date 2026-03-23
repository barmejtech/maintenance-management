import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EquipmentService } from '../../services/equipment.service';
import { Equipment, EquipmentStatus } from '../../models';

@Component({
  selector: 'app-equipment',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './equipment.component.html',
  styleUrls: ['./equipment.component.css']
})
export class EquipmentComponent implements OnInit {
  equipment = signal<Equipment[]>([]);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  EquipmentStatus = EquipmentStatus;

  form = {
    name: '',
    serialNumber: '',
    model: '',
    manufacturer: '',
    location: '',
    installationDate: '',
    lastMaintenanceDate: '',
    nextMaintenanceDate: '',
    status: EquipmentStatus.Operational,
    notes: ''
  };
  private editingId = '';

  constructor(private service: EquipmentService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({ next: d => this.equipment.set(d), error: () => {} });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { name: '', serialNumber: '', model: '', manufacturer: '', location: '', installationDate: '', lastMaintenanceDate: '', nextMaintenanceDate: '', status: EquipmentStatus.Operational, notes: '' };
    this.showModal.set(true);
  }

  openEdit(eq: Equipment) {
    this.isEditing.set(true);
    this.editingId = eq.id;
    this.form = {
      name: eq.name,
      serialNumber: eq.serialNumber,
      model: eq.model,
      manufacturer: eq.manufacturer,
      location: eq.location,
      installationDate: eq.installationDate ? eq.installationDate.substring(0, 10) : '',
      lastMaintenanceDate: eq.lastMaintenanceDate ? eq.lastMaintenanceDate.substring(0, 10) : '',
      nextMaintenanceDate: eq.nextMaintenanceDate ? eq.nextMaintenanceDate.substring(0, 10) : '',
      status: eq.status,
      notes: eq.notes ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    const dto = {
      ...this.form,
      installationDate: this.form.installationDate || null,
      lastMaintenanceDate: this.form.lastMaintenanceDate || null,
      nextMaintenanceDate: this.form.nextMaintenanceDate || null,
      status: Number(this.form.status)
    };
    const obs = this.isEditing()
      ? this.service.update(this.editingId, dto)
      : this.service.create(dto);
    obs.subscribe({
      next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
      error: () => this.isSaving.set(false)
    });
  }

  delete(id: string) {
    if (!confirm('Delete this equipment?')) return;
    this.service.delete(id).subscribe({ next: () => this.load(), error: () => {} });
  }

  getStatusLabel(s: EquipmentStatus): string { return ['Operational', 'Under Maintenance', 'Out of Service', 'Decommissioned'][s]; }
  getStatusClass(s: EquipmentStatus): string { return ['bg-success', 'bg-warning text-dark', 'bg-danger', 'bg-secondary'][s]; }
}
