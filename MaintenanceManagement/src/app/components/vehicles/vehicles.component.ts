import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { QRCodeComponent } from 'angularx-qrcode';
import { VehicleService } from '../../services/vehicle.service';
import { FileUploadService } from '../../services/file-upload.service';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translate.service';
import { CsvExportService } from '../../services/csv-export.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { Vehicle, VehicleStatus, FuelType, TransmissionType } from '../../models';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-vehicles',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule, QRCodeComponent],
  templateUrl: './vehicles.component.html',
  styleUrls: ['./vehicles.component.css']
})
export class VehiclesComponent implements OnInit {
  vehicles = signal<Vehicle[]>([]);
  currentPage = signal(1);
  readonly pageSize = 9;
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  isUploadingPhoto1 = signal(false);
  isUploadingPhoto2 = signal(false);
  isUploadingPhoto3 = signal(false);
  isUploadingPhoto4 = signal(false);
  VehicleStatus = VehicleStatus;
  FuelType = FuelType;
  TransmissionType = TransmissionType;
  readonly currentYear = new Date().getFullYear();

  form = {
    vin: '',
    make: '',
    model: '',
    year: new Date().getFullYear(),
    licensePlate: '',
    color: '',
    mileage: null as number | null,
    engineType: '',
    transmissionType: null as TransmissionType | null,
    fuelType: FuelType.Gasoline,
    ownerName: '',
    ownerPhone: '',
    ownerEmail: '',
    purchaseDate: '',
    lastServiceDate: '',
    nextServiceDate: '',
    lastServiceMileage: null as number | null,
    nextServiceMileage: null as number | null,
    status: VehicleStatus.Active,
    notes: '',
    vehiclePhotoUrl: '',
    photo2Url: '',
    photo3Url: '',
    photo4Url: ''
  };
  private editingId = '';

  constructor(
    private service: VehicleService,
    private fileService: FileUploadService,
    private translation: TranslationService,
    private csvExport: CsvExportService,
    private toast: ToastService,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({
      next: d => this.vehicles.set(d),
      error: () => this.toast.error()
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = {
      vin: '', make: '', model: '', year: new Date().getFullYear(), licensePlate: '', color: '',
      mileage: null, engineType: '', transmissionType: null, fuelType: FuelType.Gasoline,
      ownerName: '', ownerPhone: '', ownerEmail: '', purchaseDate: '', lastServiceDate: '',
      nextServiceDate: '', lastServiceMileage: null, nextServiceMileage: null,
      status: VehicleStatus.Active, notes: '', vehiclePhotoUrl: '', photo2Url: '', photo3Url: '', photo4Url: ''
    };
    this.showModal.set(true);
  }

  openEdit(v: Vehicle) {
    this.isEditing.set(true);
    this.editingId = v.id;
    this.form = {
      vin: v.vin,
      make: v.make,
      model: v.model,
      year: v.year,
      licensePlate: v.licensePlate,
      color: v.color ?? '',
      mileage: v.mileage ?? null,
      engineType: v.engineType ?? '',
      transmissionType: v.transmissionType ?? null,
      fuelType: v.fuelType,
      ownerName: v.ownerName,
      ownerPhone: v.ownerPhone ?? '',
      ownerEmail: v.ownerEmail ?? '',
      purchaseDate: v.purchaseDate ? v.purchaseDate.substring(0, 10) : '',
      lastServiceDate: v.lastServiceDate ? v.lastServiceDate.substring(0, 10) : '',
      nextServiceDate: v.nextServiceDate ? v.nextServiceDate.substring(0, 10) : '',
      lastServiceMileage: v.lastServiceMileage ?? null,
      nextServiceMileage: v.nextServiceMileage ?? null,
      status: v.status,
      notes: v.notes ?? '',
      vehiclePhotoUrl: v.vehiclePhotoUrl ?? '',
      photo2Url: v.photo2Url ?? '',
      photo3Url: v.photo3Url ?? '',
      photo4Url: v.photo4Url ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  uploadPhoto1(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    this.isUploadingPhoto1.set(true);
    this.fileService.uploadPhoto(Array.from(input.files)).subscribe({
      next: (results) => {
        if (results.length > 0) this.form.vehiclePhotoUrl = this.fileService.getPhotoUrl(results[0].url);
        this.isUploadingPhoto1.set(false);
      },
      error: () => this.isUploadingPhoto1.set(false)
    });
  }

  uploadPhoto2(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    this.isUploadingPhoto2.set(true);
    this.fileService.uploadPhoto(Array.from(input.files)).subscribe({
      next: (results) => {
        if (results.length > 0) this.form.photo2Url = this.fileService.getPhotoUrl(results[0].url);
        this.isUploadingPhoto2.set(false);
      },
      error: () => this.isUploadingPhoto2.set(false)
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
      mileage: this.form.mileage || null,
      lastServiceMileage: this.form.lastServiceMileage || null,
      nextServiceMileage: this.form.nextServiceMileage || null,
      purchaseDate: this.form.purchaseDate || null,
      lastServiceDate: this.form.lastServiceDate || null,
      nextServiceDate: this.form.nextServiceDate || null,
      transmissionType: this.form.transmissionType !== null ? Number(this.form.transmissionType) : null,
      fuelType: Number(this.form.fuelType),
      status: Number(this.form.status),
      vehiclePhotoUrl: this.form.vehiclePhotoUrl || null,
      photo2Url: this.form.photo2Url || null,
      photo3Url: this.form.photo3Url || null,
      photo4Url: this.form.photo4Url || null
    };
    const obs = this.isEditing()
      ? this.service.update(this.editingId, dto)
      : this.service.create(dto);
    obs.subscribe({
      next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created'); },
      error: () => { this.isSaving.set(false); this.toast.error(); }
    });
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('vehicles.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => { this.load(); this.toast.success('messages.deleted'); }, error: () => this.toast.error() });
  }

  getStatusLabel(s: VehicleStatus): string {
    const keys = ['vehicles.status.active', 'vehicles.status.inService', 'vehicles.status.inactive', 'vehicles.status.retired'];
    return this.translation.translate(keys[s] ?? keys[0]);
  }

  getStatusClass(s: VehicleStatus): string {
    return ['bg-success', 'bg-warning text-dark', 'bg-secondary', 'bg-dark'][s];
  }

  getFuelLabel(f: FuelType): string {
    const keys = ['vehicles.fuelType.gasoline', 'vehicles.fuelType.diesel', 'vehicles.fuelType.electric', 'vehicles.fuelType.hybrid', 'vehicles.fuelType.plugInHybrid'];
    return this.translation.translate(keys[f] ?? keys[0]);
  }

  getTransmissionLabel(t: TransmissionType | null | undefined): string {
    if (t === null || t === undefined) return '';
    const keys = ['vehicles.transmissionType.automatic', 'vehicles.transmissionType.manual', 'vehicles.transmissionType.cvt', 'vehicles.transmissionType.semiAutomatic'];
    const key = keys[t];
    return key ? this.translation.translate(key) : '';
  }

  exportToCsv(): void {
    const headers = ['ID', 'VIN', 'Make', 'Model', 'Year', 'License Plate', 'Owner', 'Status', 'Mileage', 'Next Service'];
    const rows = this.vehicles().map(v => [
      v.id,
      `"${(v.vin ?? '').replace(/"/g, '""')}"`,
      `"${(v.make ?? '').replace(/"/g, '""')}"`,
      `"${(v.model ?? '').replace(/"/g, '""')}"`,
      `${v.year}`,
      `"${(v.licensePlate ?? '').replace(/"/g, '""')}"`,
      `"${(v.ownerName ?? '').replace(/"/g, '""')}"`,
      this.getStatusLabel(v.status),
      v.mileage != null ? `${v.mileage}` : '',
      v.nextServiceDate ? new Date(v.nextServiceDate).toLocaleDateString() : ''
    ]);
    this.csvExport.download(headers, rows, 'vehicles.csv');
  }
}
