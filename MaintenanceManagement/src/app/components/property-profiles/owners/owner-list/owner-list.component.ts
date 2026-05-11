import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { OwnerService } from '../../../../services/owner.service';
import { UnitService } from '../../../../services/unit.service';
import { AuthService } from '../../../../services/auth.service';
import { TranslationService } from '../../../../services/translate.service';
import { ToastService } from '../../../../services/toast.service';
import { TranslatePipe } from '../../../../pipes/translate.pipe';
import { OwnerDto, UnitDto, CreateOwnerDto, TransferOwnershipDto } from '../../../../models';

@Component({
  selector: 'app-owner-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './owner-list.component.html',
  styleUrls: ['./owner-list.component.css']
})
export class OwnerListComponent implements OnInit {
  owners = signal<OwnerDto[]>([]);
  units = signal<UnitDto[]>([]);
  search = signal('');
  currentPage = signal(1);
  readonly pageSize = 10;
  isLoading = signal(false);

  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  selectedOwner = signal<OwnerDto | null>(null);

  showTransferModal = signal(false);
  isTransferring = signal(false);
  transferOwnerId = signal('');
  transferForm: TransferOwnershipDto = { unitId: '', ownershipPercentage: 100, purchaseDate: new Date().toISOString().substring(0, 10) };

  form: CreateOwnerDto = { fullName: '', email: '', phone: '', address: '' };
  private editingId = '';

  constructor(
    private service: OwnerService,
    private unitService: UnitService,
    public auth: AuthService,
    private translation: TranslationService,
    private toast: ToastService
  ) {}

  ngOnInit() { this.load(); this.loadUnits(); }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: d => { this.owners.set(d); this.isLoading.set(false); },
      error: () => { this.toast.error(); this.isLoading.set(false); }
    });
  }

  loadUnits() {
    this.unitService.getAll().subscribe({ next: d => this.units.set(d) });
  }

  get filtered(): OwnerDto[] {
    const q = this.search().toLowerCase();
    return q ? this.owners().filter(o => o.fullName.toLowerCase().includes(q) || o.email.toLowerCase().includes(q)) : this.owners();
  }

  getActiveUnitsCount(o: OwnerDto): number {
    return o.ownershipHistory?.filter(h => h.isActive).length ?? 0;
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { fullName: '', email: '', phone: '', address: '' };
    this.showModal.set(true);
  }

  openEdit(o: OwnerDto) {
    this.isEditing.set(true);
    this.editingId = o.id;
    this.form = { fullName: o.fullName, email: o.email, phone: o.phone ?? '', address: o.address ?? '' };
    this.showModal.set(true);
  }

  viewDetail(o: OwnerDto) { this.selectedOwner.set(o); }
  closeDetail() { this.selectedOwner.set(null); }
  closeModal() { this.showModal.set(false); }

  save() {
    if (!this.form.fullName.trim() || !this.form.email.trim()) return;
    this.isSaving.set(true);
    const dto = { fullName: this.form.fullName.trim(), email: this.form.email.trim(), phone: this.form.phone || undefined, address: this.form.address || undefined };
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

  openTransfer(ownerId: string) {
    this.transferOwnerId.set(ownerId);
    this.transferForm = { unitId: '', ownershipPercentage: 100, purchaseDate: new Date().toISOString().substring(0, 10) };
    this.showTransferModal.set(true);
  }

  closeTransfer() { this.showTransferModal.set(false); }

  doTransfer() {
    if (!this.transferForm.unitId) return;
    this.isTransferring.set(true);
    this.service.transferOwnership(this.transferOwnerId(), this.transferForm).subscribe({
      next: () => { this.isTransferring.set(false); this.showTransferModal.set(false); this.load(); this.toast.success('messages.updated'); },
      error: () => { this.isTransferring.set(false); this.toast.error(); }
    });
  }
}
