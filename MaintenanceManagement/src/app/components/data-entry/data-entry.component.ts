import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { DataEntryService } from '../../services/data-entry.service';
import { FileUploadService } from '../../services/file-upload.service';
import { DataEntry, CreateDataEntryRequest, UpdateDataEntryRequest } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-data-entry',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './data-entry.component.html',
  styleUrls: ['./data-entry.component.css']
})
export class DataEntryComponent implements OnInit {
  entries = signal<DataEntry[]>([]);
  currentPage = signal(1);
  readonly pageSize = 12;
  isLoading = signal(true);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  isUploadingPhoto = signal(false);

  form: {
    firstName: string; lastName: string; email: string;
    phone: string; section: string; password: string;
    profilePhotoUrl: string;
  } = {
    firstName: '', lastName: '', email: '', phone: '',
    section: '', password: '', profilePhotoUrl: ''
  };
  private editingId = '';

  constructor(
    private service: DataEntryService,
    private fileService: FileUploadService,
    private translation: TranslationService,
    private toast: ToastService,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: (data) => { this.entries.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { firstName: '', lastName: '', email: '', phone: '', section: '', password: '', profilePhotoUrl: '' };
    this.showModal.set(true);
  }

  openEdit(entry: DataEntry) {
    this.isEditing.set(true);
    this.editingId = entry.id;
    this.form = {
      firstName: entry.firstName, lastName: entry.lastName, email: entry.email,
      phone: entry.phone, section: entry.section, password: '',
      profilePhotoUrl: entry.profilePhotoUrl ?? ''
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
      const dto: UpdateDataEntryRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        phone: this.form.phone,
        section: this.form.section,
        profilePhotoUrl: this.form.profilePhotoUrl || undefined
      };
      this.service.update(this.editingId, dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); this.toast.success('messages.updated'); },
        error: () => { this.isSaving.set(false); this.toast.error(); }
      });
    } else {
      const dto: CreateDataEntryRequest = {
        firstName: this.form.firstName,
        lastName: this.form.lastName,
        email: this.form.email,
        phone: this.form.phone,
        section: this.form.section,
        password: this.form.password
      };
      this.service.create(dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); this.toast.success('messages.created'); },
        error: () => { this.isSaving.set(false); this.toast.error(); }
      });
    }
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('dataEntry.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => { this.load(); this.toast.success('messages.deleted'); }, error: () => this.toast.error() });
  }

  getInitials(entry: DataEntry): string {
    return `${entry.firstName.length > 0 ? entry.firstName[0] : ''}${entry.lastName.length > 0 ? entry.lastName[0] : ''}`.toUpperCase();
  }

  getPhotoUrl(url: string | undefined): string {
    return this.fileService.getPhotoUrl(url ?? '');
  }
}
