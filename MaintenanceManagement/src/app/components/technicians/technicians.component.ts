import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { TechnicianService } from '../../services/technician.service';
import { FileUploadService } from '../../services/file-upload.service';
import { Technician, TechnicianStatus, CreateTechnicianRequest, UpdateTechnicianRequest } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-technicians',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './technicians.component.html',
  styleUrls: ['./technicians.component.css']
})
export class TechniciansComponent implements OnInit {
  technicians = signal<Technician[]>([]);
  currentPage = signal(1);
  readonly pageSize = 12;
  isLoading = signal(true);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  isUploadingPhoto = signal(false);
  TechnicianStatus = TechnicianStatus;

  form: {
    firstName: string; lastName: string; email: string;
    phone: string; specialization: string; password: string;
    status: TechnicianStatus; profilePhotoUrl: string;
  } = {
    firstName: '', lastName: '', email: '', phone: '',
    specialization: '', password: '', status: TechnicianStatus.Available,
    profilePhotoUrl: ''
  };
  private editingId = '';

  constructor(
    private service: TechnicianService,
    private fileService: FileUploadService,
    private translation: TranslationService,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: (data) => { this.technicians.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { firstName: '', lastName: '', email: '', phone: '', specialization: '', password: '', status: TechnicianStatus.Available, profilePhotoUrl: '' };
    this.showModal.set(true);
  }

  openEdit(tech: Technician) {
    this.isEditing.set(true);
    this.editingId = tech.id;
    this.form = {
      firstName: tech.firstName, lastName: tech.lastName, email: tech.email,
      phone: tech.phone, specialization: tech.specialization, password: '',
      status: tech.status, profilePhotoUrl: tech.profilePhotoUrl ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  uploadProfilePhoto(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    this.isUploadingPhoto.set(true);
    this.fileService.uploadPhoto(Array.from(input.files)).subscribe({
      next: (results) => {
        if (results.length > 0) this.form.profilePhotoUrl = this.fileService.getPhotoUrl(results[0].url);
        this.isUploadingPhoto.set(false);
      },
      error: () => this.isUploadingPhoto.set(false)
    });
  }

  save() {
    this.isSaving.set(true);
    if (this.isEditing()) {
      const dto: UpdateTechnicianRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        phone: this.form.phone,
        specialization: this.form.specialization,
        status: this.form.status,
        profilePhotoUrl: this.form.profilePhotoUrl || undefined
      };
      this.service.update(this.editingId, dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    } else {
      const dto: CreateTechnicianRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        email: this.form.email,
        phone: this.form.phone,
        specialization: this.form.specialization,
        password: this.form.password
      };
      this.service.create(dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    }
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('technicians.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => this.load(), error: () => {} });
  }

  getStatusLabel(status: TechnicianStatus): string {
    const keys = ['technicians.statuses.available', 'technicians.statuses.busy', 'technicians.statuses.onLeave', 'technicians.statuses.inactive'];
    return this.translation.translate(keys[status] ?? keys[0]);
  }

  getStatusClass(status: TechnicianStatus): string {
    return ['bg-success', 'bg-warning text-dark', 'bg-info text-dark', 'bg-secondary'][status];
  }

  getInitials(tech: Technician): string {
    return `${tech.firstName[0] ?? ''}${tech.lastName[0] ?? ''}`.toUpperCase();
  }

  getPhotoUrl(url: string | undefined): string {
    return this.fileService.getPhotoUrl(url ?? '');
  }
}
