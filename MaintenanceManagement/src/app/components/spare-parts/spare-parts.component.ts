import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { SparePartService } from '../../services/spare-part.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { SparePart, CreateSparePartRequest, UpdateSparePartRequest } from '../../models';

@Component({
  selector: 'app-spare-parts',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe],
  templateUrl: './spare-parts.component.html',
  styleUrls: ['./spare-parts.component.css']
})
export class SparePartsComponent implements OnInit {
  parts = signal<SparePart[]>([]);
  isLoading = signal(true);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);

  form: {
    name: string; partNumber: string; description: string; unit: string;
    quantityInStock: number; minimumStockLevel: number; unitPrice: number;
    supplier: string; storageLocation: string; notes: string;
  } = {
    name: '', partNumber: '', description: '', unit: 'pcs',
    quantityInStock: 0, minimumStockLevel: 0, unitPrice: 0,
    supplier: '', storageLocation: '', notes: ''
  };
  private editingId = '';

  constructor(private service: SparePartService, private translation: TranslationService) {}

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
    this.form = { name: '', partNumber: '', description: '', unit: 'pcs', quantityInStock: 0, minimumStockLevel: 0, unitPrice: 0, supplier: '', storageLocation: '', notes: '' };
    this.showModal.set(true);
  }

  openEdit(part: SparePart) {
    this.isEditing.set(true);
    this.editingId = part.id;
    this.form = {
      name: part.name, partNumber: part.partNumber, description: part.description,
      unit: part.unit, quantityInStock: part.quantityInStock, minimumStockLevel: part.minimumStockLevel,
      unitPrice: part.unitPrice, supplier: part.supplier ?? '', storageLocation: part.storageLocation ?? '',
      notes: part.notes ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    if (this.isEditing()) {
      const dto: UpdateSparePartRequest = { ...this.form };
      this.service.update(this.editingId, dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    } else {
      const dto: CreateSparePartRequest = { ...this.form };
      this.service.create(dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    }
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('spareParts.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => this.load() });
  }
}
