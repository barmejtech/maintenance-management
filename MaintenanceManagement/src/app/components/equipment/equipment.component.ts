import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EquipmentService } from '../../services/equipment.service';
import { FileUploadService } from '../../services/file-upload.service';
import { TranslationService } from '../../services/translate.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { Equipment, EquipmentStatus } from '../../models';

@Component({
  selector: 'app-equipment',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe],
  templateUrl: './equipment.component.html',
  styleUrls: ['./equipment.component.css']
})
export class EquipmentComponent implements OnInit {
  equipment = signal<Equipment[]>([]);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  isUploadingBefore = signal(false);
  isUploadingAfter = signal(false);
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
    notes: '',
    beforeMaintenancePhotoUrl: '',
    afterMaintenancePhotoUrl: ''
  };
  private editingId = '';

  constructor(private service: EquipmentService, private fileService: FileUploadService, private translation: TranslationService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({ next: d => this.equipment.set(d), error: () => {} });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { name: '', serialNumber: '', model: '', manufacturer: '', location: '', installationDate: '', lastMaintenanceDate: '', nextMaintenanceDate: '', status: EquipmentStatus.Operational, notes: '', beforeMaintenancePhotoUrl: '', afterMaintenancePhotoUrl: '' };
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
      notes: eq.notes ?? '',
      beforeMaintenancePhotoUrl: eq.beforeMaintenancePhotoUrl ?? '',
      afterMaintenancePhotoUrl: eq.afterMaintenancePhotoUrl ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  uploadBeforePhoto(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    this.isUploadingBefore.set(true);
    this.fileService.uploadPhoto(Array.from(input.files)).subscribe({
      next: (results) => {
        if (results.length > 0) this.form.beforeMaintenancePhotoUrl = this.fileService.getPhotoUrl(results[0].url);
        this.isUploadingBefore.set(false);
      },
      error: () => this.isUploadingBefore.set(false)
    });
  }

  uploadAfterPhoto(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    this.isUploadingAfter.set(true);
    this.fileService.uploadPhoto(Array.from(input.files)).subscribe({
      next: (results) => {
        if (results.length > 0) this.form.afterMaintenancePhotoUrl = this.fileService.getPhotoUrl(results[0].url);
        this.isUploadingAfter.set(false);
      },
      error: () => this.isUploadingAfter.set(false)
    });
  }

  save() {
    this.isSaving.set(true);
    const dto = {
      ...this.form,
      installationDate: this.form.installationDate || null,
      lastMaintenanceDate: this.form.lastMaintenanceDate || null,
      nextMaintenanceDate: this.form.nextMaintenanceDate || null,
      status: Number(this.form.status),
      beforeMaintenancePhotoUrl: this.form.beforeMaintenancePhotoUrl || null,
      afterMaintenancePhotoUrl: this.form.afterMaintenancePhotoUrl || null
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
    if (!confirm(this.translation.translate('asset.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => this.load(), error: () => {} });
  }

  getStatusLabel(s: EquipmentStatus): string {
    const keys = ['asset.status.operational', 'asset.status.underMaintenance', 'asset.status.outOfService', 'asset.status.decommissioned'];
    return this.translation.translate(keys[s] ?? keys[0]);
  }
  getStatusClass(s: EquipmentStatus): string { return ['bg-success', 'bg-warning text-dark', 'bg-danger', 'bg-secondary'][s]; }

  // ===== CSV EXPORT =====
  exportToCsv(): void {
    const headers = [
      this.translation.translate('asset.csv.id'),
      this.translation.translate('asset.csv.name'),
      this.translation.translate('asset.csv.serialNumber'),
      this.translation.translate('asset.csv.model'),
      this.translation.translate('asset.csv.manufacturer'),
      this.translation.translate('asset.csv.location'),
      this.translation.translate('asset.csv.status'),
      this.translation.translate('asset.csv.installationDate'),
      this.translation.translate('asset.csv.lastMaintenance'),
      this.translation.translate('asset.csv.nextMaintenance')
    ];

    const rows = this.equipment().map(e => [
      e.id,
      `"${(e.name ?? '').replace(/"/g, '""')}"`,
      `"${(e.serialNumber ?? '').replace(/"/g, '""')}"`,
      `"${(e.model ?? '').replace(/"/g, '""')}"`,
      `"${(e.manufacturer ?? '').replace(/"/g, '""')}"`,
      `"${(e.location ?? '').replace(/"/g, '""')}"`,
      this.getStatusLabel(e.status),
      e.installationDate ? new Date(e.installationDate).toLocaleDateString() : '',
      e.lastMaintenanceDate ? new Date(e.lastMaintenanceDate).toLocaleDateString() : '',
      e.nextMaintenanceDate ? new Date(e.nextMaintenanceDate).toLocaleDateString() : ''
    ]);

    const csv = [headers.join(','), ...rows.map(r => r.join(','))].join('\n');
    this.downloadCsv(csv, 'equipment.csv');
  }

  private downloadCsv(content: string, filename: string): void {
    const blob = new Blob(['\uFEFF' + content], { type: 'text/csv;charset=utf-8;' });
    const url = URL.createObjectURL(blob);
    const link = document.createElement('a');
    link.href = url;
    link.download = filename;
    link.click();
    URL.revokeObjectURL(url);
  }
}
