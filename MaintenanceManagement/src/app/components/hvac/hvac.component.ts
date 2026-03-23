import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { HVACService } from '../../services/hvac.service';
import { HVACSystem } from '../../models';

@Component({
  selector: 'app-hvac',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './hvac.component.html',
  styleUrls: ['./hvac.component.css']
})
export class HVACComponent implements OnInit {
  systems = signal<HVACSystem[]>([]);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);

  form = {
    name: '', systemType: '', brand: '', model: '', capacity: 0, capacityUnit: 'kW',
    refrigerantType: '', location: '', installationDate: '', lastInspectionDate: '',
    nextInspectionDate: '', notes: ''
  };
  private editingId = '';

  constructor(private service: HVACService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({ next: d => this.systems.set(d), error: () => {} });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { name: '', systemType: '', brand: '', model: '', capacity: 0, capacityUnit: 'kW', refrigerantType: '', location: '', installationDate: '', lastInspectionDate: '', nextInspectionDate: '', notes: '' };
    this.showModal.set(true);
  }

  openEdit(hvac: HVACSystem) {
    this.isEditing.set(true);
    this.editingId = hvac.id;
    this.form = {
      name: hvac.name, systemType: hvac.systemType, brand: hvac.brand, model: hvac.model,
      capacity: hvac.capacity, capacityUnit: hvac.capacityUnit, refrigerantType: hvac.refrigerantType,
      location: hvac.location,
      installationDate: hvac.installationDate ? hvac.installationDate.substring(0, 10) : '',
      lastInspectionDate: hvac.lastInspectionDate ? hvac.lastInspectionDate.substring(0, 10) : '',
      nextInspectionDate: hvac.nextInspectionDate ? hvac.nextInspectionDate.substring(0, 10) : '',
      notes: hvac.notes ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    const dto = {
      ...this.form,
      capacity: Number(this.form.capacity),
      installationDate: this.form.installationDate || null,
      lastInspectionDate: this.form.lastInspectionDate || null,
      nextInspectionDate: this.form.nextInspectionDate || null
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
    if (!confirm('Delete this HVAC system?')) return;
    this.service.delete(id).subscribe({ next: () => this.load(), error: () => {} });
  }
}
