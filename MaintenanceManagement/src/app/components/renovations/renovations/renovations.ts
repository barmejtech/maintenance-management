import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RenovationService } from '../../../services/renovation.service';
import { UnitService } from '../../../services/unit.service';
import { ToastService } from '../../../services/toast.service';
import { TranslationService } from '../../../services/translate.service';
import { CreateRenovationRequest, Renovation, RenovationStatus, UnitDto, UpdateRenovationRequest } from '../../../models';

@Component({
  selector: 'app-renovations',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './renovations.html',
  styleUrls: ['./renovations.css']
})
export class RenovationsComponent implements OnInit {
  renovations = signal<Renovation[]>([]);
  units = signal<UnitDto[]>([]);
  selected = signal<Renovation | null>(null);
  isLoading = signal(false);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  readonly statuses = Object.values(RenovationStatus).filter(value => typeof value === 'number') as RenovationStatus[];
  readonly today = new Date().toISOString().slice(0, 10);
  form: CreateRenovationRequest & { status: RenovationStatus; actualCost: number } = this.emptyForm();
  private editingId = '';

  constructor(
    private service: RenovationService,
    private unitService: UnitService,
    private toast: ToastService,
    private translation: TranslationService
  ) {}

  ngOnInit(): void {
    this.load();
    this.unitService.getAll().subscribe({ next: units => this.units.set(units) });
  }

  load(): void {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: renovations => {
        this.renovations.set(renovations);
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

  openEdit(renovation: Renovation): void {
    this.isEditing.set(true);
    this.editingId = renovation.id;
    this.form = {
      unitId: renovation.unitId,
      title: renovation.title,
      description: renovation.description || '',
      startDate: renovation.startDate.slice(0, 10),
      endDate: renovation.endDate?.slice(0, 10),
      budget: renovation.budget,
      contractorName: renovation.contractorName || '',
      contractorPhone: renovation.contractorPhone || '',
      notes: renovation.notes || '',
      status: renovation.status,
      actualCost: renovation.actualCost
    };
    this.showModal.set(true);
  }

  closeModal(): void { this.showModal.set(false); }

  save(): void {
    const payload = {
      unitId: this.form.unitId,
      title: this.form.title.trim(),
      description: this.form.description?.trim() || undefined,
      startDate: this.form.startDate,
      endDate: this.form.endDate || undefined,
      budget: Number(this.form.budget),
      contractorName: this.form.contractorName?.trim() || undefined,
      contractorPhone: this.form.contractorPhone?.trim() || undefined,
      notes: this.form.notes?.trim() || undefined,
      status: this.form.status,
      actualCost: Number(this.form.actualCost) || 0
    };
    this.isSaving.set(true);
    const request = this.isEditing()
      ? this.service.update(this.editingId, payload satisfies UpdateRenovationRequest)
      : this.service.create(payload satisfies CreateRenovationRequest);
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

  approve(renovation: Renovation): void {
    this.service.approve(renovation.id).subscribe({
      next: updated => {
        this.selected.set(updated);
        this.load();
        this.toast.success('messages.updated');
      },
      error: () => this.toast.error()
    });
  }

  delete(renovation: Renovation): void {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) {
      return;
    }
    this.service.delete(renovation.id).subscribe({
      next: () => {
        this.selected.set(this.selected()?.id === renovation.id ? null : this.selected());
        this.load();
        this.toast.success('messages.deleted');
      },
      error: () => this.toast.error()
    });
  }

  select(renovation: Renovation): void {
    this.selected.set(renovation);
  }

  getStatusLabel(status: RenovationStatus): string {
    return RenovationStatus[status] ?? 'Unknown';
  }

  private emptyForm(): CreateRenovationRequest & { status: RenovationStatus; actualCost: number } {
    return {
      unitId: '',
      title: '',
      description: '',
      startDate: this.today,
      endDate: undefined,
      budget: 0,
      contractorName: '',
      contractorPhone: '',
      notes: '',
      status: RenovationStatus.Planned,
      actualCost: 0
    };
  }
}

