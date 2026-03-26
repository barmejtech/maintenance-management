import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { EquipmentService } from '../../services/equipment.service';
import { FileUploadService } from '../../services/file-upload.service';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translate.service';
import { CsvExportService } from '../../services/csv-export.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { Equipment, EquipmentStatus } from '../../models';

@Component({
  selector: 'app-equipment',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './equipment.component.html',
  styleUrls: ['./equipment.component.css']
})
export class EquipmentComponent implements OnInit {
  equipment = signal<Equipment[]>([]);
  currentPage = signal(1);
  readonly pageSize = 9;
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  isUploadingBefore = signal(false);
  isUploadingAfter = signal(false);
  isUploadingPhoto3 = signal(false);
  isUploadingPhoto4 = signal(false);
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
    afterMaintenancePhotoUrl: '',
    photo3Url: '',
    photo4Url: ''
  };
  private editingId = '';

  constructor(private service: EquipmentService, private fileService: FileUploadService, private translation: TranslationService, private csvExport: CsvExportService, public auth: AuthService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({ next: d => this.equipment.set(d), error: () => {} });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { name: '', serialNumber: '', model: '', manufacturer: '', location: '', installationDate: '', lastMaintenanceDate: '', nextMaintenanceDate: '', status: EquipmentStatus.Operational, notes: '', beforeMaintenancePhotoUrl: '', afterMaintenancePhotoUrl: '', photo3Url: '', photo4Url: '' };
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
      afterMaintenancePhotoUrl: eq.afterMaintenancePhotoUrl ?? '',
      photo3Url: eq.photo3Url ?? '',
      photo4Url: eq.photo4Url ?? ''
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

  uploadPhoto3(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    this.isUploadingPhoto3.set(true);
    this.fileService.uploadPhoto(Array.from(input.files)).subscribe({
      next: (results) => {
        if (results.length > 0) this.form.photo3Url = this.fileService.getPhotoUrl(results[0].url);
        this.isUploadingPhoto3.set(false);
      },
      error: () => this.isUploadingPhoto3.set(false)
    });
  }

  uploadPhoto4(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    this.isUploadingPhoto4.set(true);
    this.fileService.uploadPhoto(Array.from(input.files)).subscribe({
      next: (results) => {
        if (results.length > 0) this.form.photo4Url = this.fileService.getPhotoUrl(results[0].url);
        this.isUploadingPhoto4.set(false);
      },
      error: () => this.isUploadingPhoto4.set(false)
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
      afterMaintenancePhotoUrl: this.form.afterMaintenancePhotoUrl || null,
      photo3Url: this.form.photo3Url || null,
      photo4Url: this.form.photo4Url || null
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

    this.csvExport.download(headers, rows, 'equipment.csv');
  }
}
