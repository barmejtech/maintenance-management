import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { UnitService } from '../../../../services/unit.service';
import { UnitTypeService } from '../../../../services/unit-type.service';
import { AuthService } from '../../../../services/auth.service';
import { TranslationService } from '../../../../services/translate.service';
import { ToastService } from '../../../../services/toast.service';
import { TranslatePipe } from '../../../../pipes/translate.pipe';
import { UnitDto, UnitTypeDto, UnitStatus, CreateUnitDto, UnitMassUpdateDto } from '../../../../models';

@Component({
  selector: 'app-unit-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './unit-list.component.html',
  styleUrls: ['./unit-list.component.css']
})
export class UnitListComponent implements OnInit {
  units = signal<UnitDto[]>([]);
  unitTypes = signal<UnitTypeDto[]>([]);
  isLoading = signal(false);
  currentPage = signal(1);
  readonly pageSize = 10;

  filterStatus = signal<string>('');
  filterUnitType = signal<string>('');
  searchQuery = signal('');

  selectedIds = signal<Set<string>>(new Set());
  showMassUpdateModal = signal(false);
  massUpdateForm = { status: '' as string, floor: null as number | null };
  isMassUpdating = signal(false);

  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  selectedUnit = signal<UnitDto | null>(null);

  form: CreateUnitDto = { unitNumber: '', floor: undefined, sizeSqm: undefined, status: UnitStatus.Vacant, shareValue: undefined, unitTypeId: '' };
  private editingId = '';

  UnitStatus = UnitStatus;

  constructor(
    private service: UnitService,
    private unitTypeService: UnitTypeService,
    public auth: AuthService,
    private translation: TranslationService,
    private toast: ToastService
  ) {}

  ngOnInit() { this.load(); this.loadUnitTypes(); }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: d => { this.units.set(d); this.isLoading.set(false); },
      error: () => { this.toast.error(); this.isLoading.set(false); }
    });
  }

  loadUnitTypes() {
    this.unitTypeService.getAll().subscribe({ next: d => this.unitTypes.set(d) });
  }

  get filtered(): UnitDto[] {
    let items = this.units();
    const q = this.searchQuery().toLowerCase();
    if (q) items = items.filter(u => u.unitNumber.toLowerCase().includes(q) || u.unitTypeName.toLowerCase().includes(q));
    if (this.filterStatus() !== '') items = items.filter(u => u.status === +this.filterStatus());
    if (this.filterUnitType()) items = items.filter(u => u.unitTypeId === this.filterUnitType());
    return items;
  }

  toggleSelect(id: string) {
    const set = new Set(this.selectedIds());
    if (set.has(id)) set.delete(id); else set.add(id);
    this.selectedIds.set(set);
  }

  isSelected(id: string): boolean { return this.selectedIds().has(id); }

  toggleAll() {
    const ids = this.filtered.map(u => u.id);
    if (ids.every(id => this.selectedIds().has(id))) {
      this.selectedIds.set(new Set());
    } else {
      this.selectedIds.set(new Set(ids));
    }
  }

  get allSelected(): boolean {
    const ids = this.filtered.map(u => u.id);
    return ids.length > 0 && ids.every(id => this.selectedIds().has(id));
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { unitNumber: '', floor: undefined, sizeSqm: undefined, status: UnitStatus.Vacant, shareValue: undefined, unitTypeId: '' };
    this.selectedUnit.set(null);
    this.showModal.set(true);
  }

  openEdit(u: UnitDto) {
    this.isEditing.set(true);
    this.editingId = u.id;
    this.form = { unitNumber: u.unitNumber, floor: u.floor, sizeSqm: u.sizeSqm, status: u.status, shareValue: u.shareValue, unitTypeId: u.unitTypeId };
    this.selectedUnit.set(u);
    this.showModal.set(true);
  }

  viewDetail(u: UnitDto) {
    this.selectedUnit.set(u);
    this.showModal.set(false);
  }

  closeModal() { this.showModal.set(false); }
  closeDetail() { this.selectedUnit.set(null); }

  save() {
    if (!this.form.unitNumber.trim() || !this.form.unitTypeId) return;
    this.isSaving.set(true);
    const dto = {
      unitNumber: this.form.unitNumber.trim(),
      floor: this.form.floor || undefined,
      sizeSqm: this.form.sizeSqm || undefined,
      status: Number(this.form.status) as UnitStatus,
      shareValue: this.form.shareValue || undefined,
      unitTypeId: this.form.unitTypeId
    };
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

  openMassUpdate() { this.massUpdateForm = { status: '', floor: null }; this.showMassUpdateModal.set(true); }
  closeMassUpdate() { this.showMassUpdateModal.set(false); }

  doMassUpdate() {
    if (!this.massUpdateForm.status && this.massUpdateForm.floor === null) return;
    this.isMassUpdating.set(true);
    const dto: UnitMassUpdateDto = {
      unitIds: Array.from(this.selectedIds()),
      status: this.massUpdateForm.status !== '' ? Number(this.massUpdateForm.status) as UnitStatus : undefined,
      floor: this.massUpdateForm.floor ?? undefined
    };
    this.service.massUpdate(dto).subscribe({
      next: () => { this.isMassUpdating.set(false); this.showMassUpdateModal.set(false); this.selectedIds.set(new Set()); this.load(); this.toast.success('messages.updated'); },
      error: () => { this.isMassUpdating.set(false); this.toast.error(); }
    });
  }

  getStatusLabel(s: UnitStatus): string {
    const labels: Record<number, string> = { 0: 'Vacant', 1: 'Occupied', 2: 'Under Maintenance', 3: 'Reserved' };
    return labels[s] ?? 'Unknown';
  }

  getStatusClass(s: UnitStatus): string {
    const classes: Record<number, string> = { 0: 'bg-secondary', 1: 'bg-success', 2: 'bg-warning text-dark', 3: 'bg-primary' };
    return classes[s] ?? 'bg-secondary';
  }

  getCurrentOwner(u: UnitDto): string {
    const active = u.ownershipHistory?.find(h => h.isActive);
    return active ? active.ownerName : '—';
  }
}
