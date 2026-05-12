import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ExpenseService } from '../../../services/expense.service';
import { VendorService } from '../../../services/vendor.service';
import { RenovationService } from '../../../services/renovation.service';
import { ToastService } from '../../../services/toast.service';
import { TranslationService } from '../../../services/translate.service';
import { CreateExpenseRequest, Expense, ExpenseStatus, Renovation, UpdateExpenseRequest, Vendor } from '../../../models';

@Component({
  selector: 'app-expenses',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './expenses.html',
  styleUrls: ['./expenses.css']
})
export class ExpensesComponent implements OnInit {
  expenses = signal<Expense[]>([]);
  vendors = signal<Vendor[]>([]);
  renovations = signal<Renovation[]>([]);
  isLoading = signal(false);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  readonly statuses = Object.values(ExpenseStatus).filter(value => typeof value === 'number') as ExpenseStatus[];
  readonly today = new Date().toISOString().slice(0, 10);

  form: CreateExpenseRequest & { status: ExpenseStatus } = this.emptyForm();
  private editingId = '';

  constructor(
    private service: ExpenseService,
    private vendorService: VendorService,
    private renovationService: RenovationService,
    private toast: ToastService,
    private translation: TranslationService
  ) {}

  ngOnInit(): void {
    this.load();
    this.vendorService.getActive().subscribe({ next: vendors => this.vendors.set(vendors) });
    this.renovationService.getAll().subscribe({ next: renovations => this.renovations.set(renovations) });
  }

  load(): void {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: expenses => {
        this.expenses.set(expenses);
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

  openEdit(expense: Expense): void {
    this.isEditing.set(true);
    this.editingId = expense.id;
    this.form = {
      vendorId: expense.vendorId,
      amount: expense.amount,
      taxAmount: expense.taxAmount,
      expenseDate: expense.expenseDate.slice(0, 10),
      dueDate: expense.dueDate?.slice(0, 10),
      description: expense.description || '',
      invoiceNumber: expense.invoiceNumber || '',
      renovationId: expense.renovationId,
      sparePartId: expense.sparePartId,
      status: expense.status
    };
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  save(): void {
    const payload = {
      vendorId: this.form.vendorId,
      amount: Number(this.form.amount),
      taxAmount: this.form.taxAmount || undefined,
      expenseDate: this.form.expenseDate,
      dueDate: this.form.dueDate || undefined,
      description: this.form.description?.trim() || undefined,
      invoiceNumber: this.form.invoiceNumber?.trim() || undefined,
      renovationId: this.form.renovationId || undefined,
      sparePartId: this.form.sparePartId || undefined,
      status: this.form.status
    };

    this.isSaving.set(true);
    const request = this.isEditing()
      ? this.service.update(this.editingId, payload satisfies UpdateExpenseRequest)
      : this.service.create(payload satisfies CreateExpenseRequest);

    request.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showModal.set(false);
        this.load();
        this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created');
      },
      error: err => {
        this.isSaving.set(false);
        this.toast.show(err?.error?.message || this.translation.translate('messages.error'), 'error');
      }
    });
  }

  approve(expense: Expense): void {
    this.service.approve(expense.id).subscribe({
      next: () => {
        this.load();
        this.toast.success('messages.updated');
      },
      error: err => this.toast.show(err?.error?.message || this.translation.translate('messages.error'), 'error')
    });
  }

  delete(expense: Expense): void {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) {
      return;
    }
    this.service.delete(expense.id).subscribe({
      next: () => {
        this.load();
        this.toast.success('messages.deleted');
      },
      error: () => this.toast.error()
    });
  }

  getStatusLabel(status: ExpenseStatus): string {
    return ExpenseStatus[status] ?? 'Unknown';
  }

  private emptyForm(): CreateExpenseRequest & { status: ExpenseStatus } {
    return {
      vendorId: '',
      amount: 0,
      taxAmount: undefined,
      expenseDate: this.today,
      dueDate: undefined,
      description: '',
      invoiceNumber: '',
      renovationId: undefined,
      sparePartId: undefined,
      status: ExpenseStatus.Draft
    };
  }
}

