import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { UnitService } from '../../../../services/unit.service';
import { StrataRollService } from '../../../../services/strata-roll.service';
import { AuthService } from '../../../../services/auth.service';
import { TranslationService } from '../../../../services/translate.service';
import { ToastService } from '../../../../services/toast.service';
import { CreateStrataRollDto, StrataRollDto, StrataRollStatus, UnitDto } from '../../../../models';

@Component({
  selector: 'app-strata-roll-list',
  standalone: true,
  imports: [CommonModule, FormsModule, NgxPaginationModule],
  templateUrl: './strata-roll-list.component.html',
  styleUrls: ['./strata-roll-list.component.css']
})
export class StrataRollListComponent implements OnInit {
  rolls = signal<StrataRollDto[]>([]);
  units = signal<UnitDto[]>([]);
  isLoading = signal(false);
  showModal = signal(false);
  isSaving = signal(false);
  isEditing = signal(false);
  currentPage = signal(1);
  readonly pageSize = 10;
  filterYear = signal('');
  filterUnit = signal('');

  form: CreateStrataRollDto = {
    unitId: '',
    fiscalYear: new Date().getFullYear(),
    totalShareValue: 0,
    feeAmount: 0,
    dueDate: new Date().toISOString().slice(0, 10),
    notes: '',
    status: StrataRollStatus.Draft
  };
  private editingId = '';

  readonly statusOptions = [
    { value: StrataRollStatus.Draft, label: 'Draft' },
    { value: StrataRollStatus.Published, label: 'Published' },
    { value: StrataRollStatus.Closed, label: 'Closed' }
  ];

  constructor(
    private strataService: StrataRollService,
    private unitService: UnitService,
    public auth: AuthService,
    private translation: TranslationService,
    private toast: ToastService
  ) {}

  ngOnInit(): void {
    this.loadUnits();
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    this.strataService.getAll().subscribe({
      next: rolls => {
        this.rolls.set(rolls.map(roll => ({ ...roll, unitNumber: this.unitLabel(roll.unitId) })));
        this.isLoading.set(false);
      },
      error: () => {
        this.toast.error();
        this.isLoading.set(false);
      }
    });
  }

  loadUnits(): void {
    this.unitService.getAll().subscribe({
      next: units => {
        this.units.set(units);
        const labels = units.reduce<Record<string, string>>((acc, unit) => {
          acc[unit.id] = unit.unitNumber;
          return acc;
        }, {});
        this.strataService.seedUnitLabels(labels);
        this.rolls.update(rolls => rolls.map(roll => ({ ...roll, unitNumber: labels[roll.unitId] ?? roll.unitNumber })));
      },
      error: () => this.units.set([])
    });
  }

  get filtered(): StrataRollDto[] {
    let items = this.rolls();
    if (this.filterYear()) items = items.filter(r => String(r.fiscalYear) === this.filterYear());
    if (this.filterUnit()) items = items.filter(r => r.unitId === this.filterUnit());
    return items;
  }

  openAdd(): void {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = {
      unitId: '',
      fiscalYear: new Date().getFullYear(),
      totalShareValue: 0,
      feeAmount: 0,
      dueDate: new Date().toISOString().slice(0, 10),
      notes: '',
      status: StrataRollStatus.Draft
    };
    this.showModal.set(true);
  }

  openEdit(roll: StrataRollDto): void {
    this.isEditing.set(true);
    this.editingId = roll.id;
    this.form = {
      unitId: roll.unitId,
      fiscalYear: roll.fiscalYear,
      totalShareValue: roll.totalShareValue,
      feeAmount: roll.feeAmount,
      dueDate: roll.dueDate.slice(0, 10),
      notes: roll.notes ?? '',
      status: roll.status
    };
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  save(): void {
    if (!this.form.unitId || !this.form.dueDate) return;
    this.isSaving.set(true);
    const payload = {
      ...this.form,
      notes: this.form.notes?.trim() || undefined,
      feeAmount: Number(this.form.feeAmount),
      totalShareValue: Number(this.form.totalShareValue)
    };
    const request = this.isEditing()
      ? this.strataService.update(this.editingId, payload)
      : this.strataService.create(payload);

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

  delete(roll: StrataRollDto): void {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) return;
    this.strataService.delete(roll.id).subscribe({
      next: () => {
        this.load();
        this.toast.success('messages.deleted');
      },
      error: () => this.toast.error()
    });
  }

  getStatusLabel(status: StrataRollStatus): string {
    return this.statusOptions.find(option => option.value === status)?.label ?? 'Unknown';
  }

  unitLabel(unitId: string): string {
    return this.units().find(unit => unit.id === unitId)?.unitNumber ?? unitId;
  }
}
