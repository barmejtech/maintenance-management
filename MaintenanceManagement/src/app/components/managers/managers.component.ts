import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { ManagerService } from '../../services/manager.service';
import { FileUploadService } from '../../services/file-upload.service';
import { Manager, CreateManagerRequest, UpdateManagerRequest } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';

@Component({
  selector: 'app-managers',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  styleUrls: ['./managers.component.css']
})
export class ManagersComponent implements OnInit {
  managers = signal<Manager[]>([]);
  currentPage = signal(1);
  readonly pageSize = 12;
  isLoading = signal(true);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  isUploadingPhoto = signal(false);

  form: {
    firstName: string; lastName: string; email: string;
    phone: string; department: string; password: string;
    profilePhotoUrl: string;
  } = {
    firstName: '', lastName: '', email: '', phone: '',
    department: '', password: '', profilePhotoUrl: ''
  };
  private editingId = '';

  constructor(
    private service: ManagerService,
    private fileService: FileUploadService,
    private translation: TranslationService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: (data) => { this.managers.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { firstName: '', lastName: '', email: '', phone: '', department: '', password: '', profilePhotoUrl: '' };
    this.showModal.set(true);
  }

  openEdit(manager: Manager) {
    this.isEditing.set(true);
    this.editingId = manager.id;
    this.form = {
      firstName: manager.firstName, lastName: manager.lastName, email: manager.email,
      phone: manager.phone, department: manager.department, password: '',
      profilePhotoUrl: manager.profilePhotoUrl ?? ''
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
      const dto: UpdateManagerRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        phone: this.form.phone,
        department: this.form.department,
        profilePhotoUrl: this.form.profilePhotoUrl || undefined
      };
      this.service.update(this.editingId, dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    } else {
      const dto: CreateManagerRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        email: this.form.email,
        phone: this.form.phone,
        department: this.form.department,
        password: this.form.password
      };
      this.service.create(dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    }
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('managers.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => this.load(), error: () => {} });
  }

  getInitials(manager: Manager): string {
    return `${manager.firstName.length > 0 ? manager.firstName[0] : ''}${manager.lastName.length > 0 ? manager.lastName[0] : ''}`.toUpperCase();
  }

  getPhotoUrl(url: string | undefined): string {
    return this.fileService.getPhotoUrl(url ?? '');
  }
}
