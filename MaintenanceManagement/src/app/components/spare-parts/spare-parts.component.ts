import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { QRCodeComponent } from 'angularx-qrcode';
import { SparePartService } from '../../services/spare-part.service';
import { FileUploadService } from '../../services/file-upload.service';
import { AuthService } from '../../services/auth.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { SparePart, CreateSparePartRequest, UpdateSparePartRequest } from '../../models';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-spare-parts',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule, QRCodeComponent],
  templateUrl: './spare-parts.component.html',
  styleUrls: ['./spare-parts.component.css']
})
export class SparePartsComponent implements OnInit {
  parts = signal<SparePart[]>([]);
  currentPage = signal(1);
  readonly pageSize = 9;
  isLoading = signal(true);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  isUploadingPhoto1 = signal(false);
  isUploadingPhoto2 = signal(false);
  isUploadingPhoto3 = signal(false);
  isUploadingPhoto4 = signal(false);

  form: {
    name: string; partNumber: string; description: string; unit: string;
    quantityInStock: number; minimumStockLevel: number; unitPrice: number;
    supplier: string; storageLocation: string; notes: string;
    photo1Url: string; photo2Url: string; photo3Url: string; photo4Url: string;
  } = {
    name: '', partNumber: '', description: '', unit: 'pcs',
    quantityInStock: 0, minimumStockLevel: 0, unitPrice: 0,
    supplier: '', storageLocation: '', notes: '',
    photo1Url: '', photo2Url: '', photo3Url: '', photo4Url: ''
  };
  private editingId = '';

  constructor(
    private service: SparePartService,
    private fileService: FileUploadService,
    private translation: TranslationService,
    private toast: ToastService,
    public auth: AuthService
  ) {}

  ngOnInit() { this.load(); }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: (data) => { this.parts.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { name: '', partNumber: '', description: '', unit: 'pcs', quantityInStock: 0, minimumStockLevel: 0, unitPrice: 0, supplier: '', storageLocation: '', notes: '', photo1Url: '', photo2Url: '', photo3Url: '', photo4Url: '' };
    this.showModal.set(true);
  }

  openEdit(part: SparePart) {
    this.isEditing.set(true);
    this.editingId = part.id;
    this.form = {
      name: part.name, partNumber: part.partNumber, description: part.description,
      unit: part.unit, quantityInStock: part.quantityInStock, minimumStockLevel: part.minimumStockLevel,
      unitPrice: part.unitPrice, supplier: part.supplier ?? '', storageLocation: part.storageLocation ?? '',
      notes: part.notes ?? '',
      photo1Url: part.photo1Url ?? '', photo2Url: part.photo2Url ?? '',
      photo3Url: part.photo3Url ?? '', photo4Url: part.photo4Url ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  uploadPhoto(slot: 1 | 2 | 3 | 4, event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    const uploading = [this.isUploadingPhoto1, this.isUploadingPhoto2, this.isUploadingPhoto3, this.isUploadingPhoto4][slot - 1];
    uploading.set(true);
    this.fileService.uploadPhoto(Array.from(input.files)).subscribe({
      next: (results) => {
        if (results.length > 0) {
          const url = this.fileService.getPhotoUrl(results[0].url);
          if (slot === 1) this.form.photo1Url = url;
          else if (slot === 2) this.form.photo2Url = url;
          else if (slot === 3) this.form.photo3Url = url;
          else this.form.photo4Url = url;
        }
        uploading.set(false);
      },
      error: () => uploading.set(false)
    });
  }

  save() {
    this.isSaving.set(true);
    const dto = {
      ...this.form,
      photo1Url: this.form.photo1Url || undefined,
      photo2Url: this.form.photo2Url || undefined,
      photo3Url: this.form.photo3Url || undefined,
      photo4Url: this.form.photo4Url || undefined
    };
    if (this.isEditing()) {
      this.service.update(this.editingId, dto as UpdateSparePartRequest).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created'); },
        error: () => { this.isSaving.set(false); this.toast.error(); }
      });
    } else {
      this.service.create(dto as CreateSparePartRequest).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created'); },
        error: () => { this.isSaving.set(false); this.toast.error(); }
      });
    }
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('spareParts.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => { this.load(); this.toast.success('messages.deleted'); }, error: () => this.toast.error() });
  }

  isUploading(): boolean {
    return this.isUploadingPhoto1() || this.isUploadingPhoto2() || this.isUploadingPhoto3() || this.isUploadingPhoto4();
  }

  getStockPercent(qty: number, min: number): number {
    if (min <= 0) return qty > 0 ? 100 : 0;
    // Progress bar fills to 100% when stock reaches twice the minimum level
    const targetStock = min * 2;
    return Math.min(100, Math.round((qty / targetStock) * 100));
  }
}
