import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { UnitTypeService } from '../../../../services/unit-type.service';
import { AuthService } from '../../../../services/auth.service';
import { TranslationService } from '../../../../services/translate.service';
import { ToastService } from '../../../../services/toast.service';
import { TranslatePipe } from '../../../../pipes/translate.pipe';
import { UnitTypeDto } from '../../../../models';

@Component({
  selector: 'app-unit-type-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './unit-type-list.component.html',
  styleUrls: ['./unit-type-list.component.css']
})
export class UnitTypeListComponent implements OnInit {
  unitTypes = signal<UnitTypeDto[]>([]);
  search = signal('');
  currentPage = signal(1);
  readonly pageSize = 10;
  isLoading = signal(false);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);

  form = { name: '', description: '', defaultSizeSqm: null as number | null };
  private editingId = '';

  constructor(
    private service: UnitTypeService,
    public auth: AuthService,
    private translation: TranslationService,
    private toast: ToastService
  ) {}

  ngOnInit() { this.load(); }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: d => { this.unitTypes.set(d); this.isLoading.set(false); },
      error: () => { this.toast.error(); this.isLoading.set(false); }
    });
  }

  get filtered(): UnitTypeDto[] {
    const q = this.search().toLowerCase();
    return q ? this.unitTypes().filter(t => t.name.toLowerCase().includes(q)) : this.unitTypes();
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { name: '', description: '', defaultSizeSqm: null };
    this.showModal.set(true);
  }

  openEdit(t: UnitTypeDto) {
    this.isEditing.set(true);
    this.editingId = t.id;
    this.form = { name: t.name, description: t.description ?? '', defaultSizeSqm: t.defaultSizeSqm ?? null };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    if (!this.form.name.trim()) return;
    this.isSaving.set(true);
    const dto = { name: this.form.name.trim(), description: this.form.description || undefined, defaultSizeSqm: this.form.defaultSizeSqm || undefined };
    const obs = this.isEditing() ? this.service.update(this.editingId, dto) : this.service.create(dto);
    obs.subscribe({
      next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created'); },
      error: () => { this.isSaving.set(false); this.toast.error(); }
    });
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) return;
    this.service.delete(id).subscribe({
      next: () => { this.load(); this.toast.success('messages.deleted'); },
      error: () => this.toast.error()
    });
  }
}
