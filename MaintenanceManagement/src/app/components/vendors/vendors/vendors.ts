import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { VendorService } from '../../../services/vendor.service';
import { ToastService } from '../../../services/toast.service';
import { TranslationService } from '../../../services/translate.service';
import { CreateVendorRequest, UpdateVendorRequest, Vendor } from '../../../models';

@Component({
  selector: 'app-vendors',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './vendors.html',
  styleUrls: ['./vendors.css']
})
export class VendorsComponent implements OnInit {
  vendors = signal<Vendor[]>([]);
  isLoading = signal(false);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  form: UpdateVendorRequest = this.emptyForm();
  private editingId = '';

  constructor(
    private service: VendorService,
    private toast: ToastService,
    private translation: TranslationService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: vendors => {
        this.vendors.set(vendors);
        this.isLoading.set(false);
      },
      error: () => {
        this.toast.error();
        this.isLoading.set(false);
      }
    });
  }

  openAdd(): void {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = this.emptyForm();
    this.showModal.set(true);
  }

  openEdit(vendor: Vendor): void {
    this.isEditing.set(true);
    this.editingId = vendor.id;
    this.form = {
      name: vendor.name,
      companyName: vendor.companyName || '',
      email: vendor.email,
      phone: vendor.phone || '',
      address: vendor.address || '',
      taxNumber: vendor.taxNumber || '',
      bankName: vendor.bankName || '',
      bankAccountNumber: vendor.bankAccountNumber || '',
      notes: vendor.notes || '',
      isActive: vendor.isActive
    };
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  save(): void {
    const payload = {
      name: this.form.name.trim(),
      companyName: this.form.companyName?.trim() || undefined,
      email: this.form.email.trim(),
      phone: this.form.phone?.trim() || undefined,
      address: this.form.address?.trim() || undefined,
      taxNumber: this.form.taxNumber?.trim() || undefined,
      bankName: this.form.bankName?.trim() || undefined,
      bankAccountNumber: this.form.bankAccountNumber?.trim() || undefined,
      notes: this.form.notes?.trim() || undefined,
      isActive: this.form.isActive
    };

    this.isSaving.set(true);
    const request = this.isEditing()
      ? this.service.update(this.editingId, payload satisfies UpdateVendorRequest)
      : this.service.create(payload satisfies CreateVendorRequest);

    request.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showModal.set(false);
        this.load();
        this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created');
      },
      error: () => {
        this.isSaving.set(false);
        this.toast.error();
      }
    });
  }

  delete(vendor: Vendor): void {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) {
      return;
    }
    this.service.delete(vendor.id).subscribe({
      next: () => {
        this.load();
        this.toast.success('messages.deleted');
      },
      error: () => this.toast.error()
    });
  }

  private emptyForm(): UpdateVendorRequest {
    return {
      name: '',
      companyName: '',
      email: '',
      phone: '',
      address: '',
      taxNumber: '',
      bankName: '',
      bankAccountNumber: '',
      notes: '',
      isActive: true
    };
  }
}

