import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { BankReconciliationService } from '../../../services/bank-reconciliation.service';
import { ToastService } from '../../../services/toast.service';
import { TranslationService } from '../../../services/translate.service';
import { BankReconciliation, CompleteReconciliationRequest, CreateBankReconciliationRequest } from '../../../models';

@Component({
  selector: 'app-bank-reconciliation',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './bank-reconciliation.html',
  styleUrls: ['./bank-reconciliation.css']
})
export class BankReconciliationComponent implements OnInit {
  reconciliations = signal<BankReconciliation[]>([]);
  selected = signal<BankReconciliation | null>(null);
  showCreateModal = signal(false);
  showReconcileModal = signal(false);
  isLoading = signal(false);
  isSaving = signal(false);
  readonly today = new Date().toISOString().slice(0, 10);
  form: CreateBankReconciliationRequest = this.emptyForm();
  reconcileForm: CompleteReconciliationRequest = { isReconciled: true, notes: '' };
  private reconcilingId = '';

  constructor(
    private service: BankReconciliationService,
    private toast: ToastService,
    private translation: TranslationService
  ) {}

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: data => {
        this.reconciliations.set(data);
        this.isLoading.set(false);
      },
      error: () => {
        this.toast.error();
        this.isLoading.set(false);
      }
    });
  }

  openAdd(): void {
    this.form = this.emptyForm();
    this.showCreateModal.set(true);
  }

  closeAdd(): void {
    this.showCreateModal.set(false);
  }

  save(): void {
    this.isSaving.set(true);
    this.service.create({
      ...this.form,
      notes: this.form.notes?.trim() || undefined
    }).subscribe({
      next: created => {
        this.isSaving.set(false);
        this.showCreateModal.set(false);
        this.selected.set(created);
        this.load();
        this.toast.success('messages.created');
      },
      error: () => {
        this.isSaving.set(false);
        this.toast.error();
      }
    });
  }

  openReconcile(item: BankReconciliation): void {
    this.reconcilingId = item.id;
    this.reconcileForm = { isReconciled: true, notes: item.notes || '' };
    this.showReconcileModal.set(true);
  }

  closeReconcile(): void {
    this.showReconcileModal.set(false);
  }

  reconcile(): void {
    this.isSaving.set(true);
    this.service.reconcile(this.reconcilingId, {
      isReconciled: this.reconcileForm.isReconciled ?? true,
      notes: this.reconcileForm.notes?.trim() || undefined
    }).subscribe({
      next: reconciled => {
        this.isSaving.set(false);
        this.showReconcileModal.set(false);
        this.selected.set(reconciled);
        this.load();
        this.toast.success('messages.updated');
      },
      error: () => {
        this.isSaving.set(false);
        this.toast.error();
      }
    });
  }

  delete(item: BankReconciliation): void {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) {
      return;
    }
    this.service.delete(item.id).subscribe({
      next: () => {
        this.selected.set(this.selected()?.id === item.id ? null : this.selected());
        this.load();
        this.toast.success('messages.deleted');
      },
      error: () => this.toast.error()
    });
  }

  select(item: BankReconciliation): void {
    this.selected.set(item);
  }

  private emptyForm(): CreateBankReconciliationRequest {
    return {
      bankAccountName: '',
      bankAccountNumber: '',
      statementDate: this.today,
      statementStartDate: this.today,
      statementEndDate: this.today,
      statementOpeningBalance: 0,
      statementClosingBalance: 0,
      notes: ''
    };
  }
}

export { BankReconciliationComponent as BankReconciliation };
