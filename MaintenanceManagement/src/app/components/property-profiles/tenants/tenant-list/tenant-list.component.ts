import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { TenantService } from '../../../../services/tenant.service';
import { UnitService } from '../../../../services/unit.service';
import { AuthService } from '../../../../services/auth.service';
import { TranslationService } from '../../../../services/translate.service';
import { ToastService } from '../../../../services/toast.service';
import { TranslatePipe } from '../../../../pipes/translate.pipe';
import { TenantDto, UnitDto, CreateTenantDto } from '../../../../models';

@Component({
  selector: 'app-tenant-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './tenant-list.component.html',
  styleUrls: ['./tenant-list.component.css']
})
export class TenantListComponent implements OnInit {
  tenants = signal<TenantDto[]>([]);
  units = signal<UnitDto[]>([]);
  search = signal('');
  filterUnit = signal('');
  currentPage = signal(1);
  readonly pageSize = 10;
  isLoading = signal(false);

  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);

  form: CreateTenantDto = {
    unitId: '', fullName: '', email: '', phone: '', emergencyContactName: '', emergencyContactPhone: '',
    leaseStartDate: '', leaseEndDate: '', rentalAmount: 0, depositAmount: undefined
  };
  private editingId = '';

  constructor(
    private service: TenantService,
    private unitService: UnitService,
    public auth: AuthService,
    private translation: TranslationService,
    private toast: ToastService
  ) {}

  ngOnInit() { this.load(); this.loadUnits(); }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: d => { this.tenants.set(d); this.isLoading.set(false); },
      error: () => { this.toast.error(); this.isLoading.set(false); }
    });
  }

  loadUnits() {
    this.unitService.getAll().subscribe({ next: d => this.units.set(d) });
  }

  get filtered(): TenantDto[] {
    let items = this.tenants();
    const q = this.search().toLowerCase();
    if (q) items = items.filter(t => t.fullName.toLowerCase().includes(q) || t.email.toLowerCase().includes(q) || t.unitNumber.toLowerCase().includes(q));
    if (this.filterUnit()) items = items.filter(t => t.unitId === this.filterUnit());
    return items;
  }

  isExpiringSoon(t: TenantDto): boolean {
    const end = new Date(t.leaseEndDate);
    end.setHours(0, 0, 0, 0);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    const diff = (end.getTime() - today.getTime()) / (1000 * 60 * 60 * 24);
    return diff >= 0 && diff <= 30;
  }

  isExpired(t: TenantDto): boolean {
    const end = new Date(t.leaseEndDate);
    end.setHours(0, 0, 0, 0);
    const today = new Date();
    today.setHours(0, 0, 0, 0);
    return end < today;
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { unitId: '', fullName: '', email: '', phone: '', emergencyContactName: '', emergencyContactPhone: '', leaseStartDate: '', leaseEndDate: '', rentalAmount: 0, depositAmount: undefined };
    this.showModal.set(true);
  }

  openEdit(t: TenantDto) {
    this.isEditing.set(true);
    this.editingId = t.id;
    this.form = {
      unitId: t.unitId, fullName: t.fullName, email: t.email, phone: t.phone ?? '',
      emergencyContactName: t.emergencyContactName ?? '', emergencyContactPhone: t.emergencyContactPhone ?? '',
      leaseStartDate: t.leaseStartDate.substring(0, 10), leaseEndDate: t.leaseEndDate.substring(0, 10),
      rentalAmount: t.rentalAmount, depositAmount: t.depositAmount
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  isFormValid(): boolean {
    if (!this.form.unitId || !this.form.fullName.trim() || !this.form.email.trim()) return false;
    if (!this.form.leaseStartDate || !this.form.leaseEndDate) return false;
    if (new Date(this.form.leaseEndDate) <= new Date(this.form.leaseStartDate)) return false;
    if (!this.form.rentalAmount || this.form.rentalAmount <= 0) return false;
    return true;
  }

  save() {
    if (!this.isFormValid()) return;
    this.isSaving.set(true);
    const dto = {
      ...this.form,
      phone: this.form.phone || undefined,
      emergencyContactName: this.form.emergencyContactName || undefined,
      emergencyContactPhone: this.form.emergencyContactPhone || undefined,
      depositAmount: this.form.depositAmount || undefined
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
}
