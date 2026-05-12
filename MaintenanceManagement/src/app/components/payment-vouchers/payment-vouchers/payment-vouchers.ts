import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PaymentVoucherService } from '../../../services/payment-voucher.service';
import { ExpenseService } from '../../../services/expense.service';
import { InvoiceService } from '../../../services/invoice.service';
import { OwnerService } from '../../../services/owner.service';
import { ToastService } from '../../../services/toast.service';
import { TranslationService } from '../../../services/translate.service';
import { CreatePaymentVoucherRequest, Expense, Invoice, OwnerDto, PaymentMethod, PaymentVoucher, UpdatePaymentVoucherRequest } from '../../../models';

@Component({
  selector: 'app-payment-vouchers',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './payment-vouchers.html',
  styleUrls: ['./payment-vouchers.css']
})
export class PaymentVouchersComponent implements OnInit {
  vouchers = signal<PaymentVoucher[]>([]);
  expenses = signal<Expense[]>([]);
  invoices = signal<Invoice[]>([]);
  owners = signal<OwnerDto[]>([]);
  isLoading = signal(false);
  isSaving = signal(false);
  showModal = signal(false);
  isEditing = signal(false);
  readonly paymentMethods = Object.values(PaymentMethod).filter(value => typeof value === 'number') as PaymentMethod[];
  readonly today = new Date().toISOString().slice(0, 10);

  form: CreatePaymentVoucherRequest = this.emptyForm();
  private editingId = '';

  constructor(
    private service: PaymentVoucherService,
    private expenseService: ExpenseService,
    private invoiceService: InvoiceService,
    private ownerService: OwnerService,
    private toast: ToastService,
    private translation: TranslationService
  ) {}

  ngOnInit(): void {
    this.load();
    this.expenseService.getAll().subscribe({ next: expenses => this.expenses.set(expenses) });
    this.invoiceService.getAll().subscribe({ next: invoices => this.invoices.set(invoices) });
    this.ownerService.getAll().subscribe({ next: owners => this.owners.set(owners) });
  }

  load(): void {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: vouchers => {
        this.vouchers.set(vouchers);
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

  openEdit(voucher: PaymentVoucher): void {
    this.isEditing.set(true);
    this.editingId = voucher.id;
    this.form = {
      amount: voucher.amount,
      paymentMethod: voucher.paymentMethod,
      chequeNumber: voucher.chequeNumber || '',
      bankName: voucher.bankName || '',
      chequeDate: voucher.chequeDate || '',
      expenseId: voucher.expenseId,
      invoiceId: voucher.invoiceId,
      ownerId: voucher.ownerId,
      payeeName: voucher.payeeName || '',
      description: voucher.description || ''
    };
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
  }

  save(): void {
    const payload = {
      amount: Number(this.form.amount) || 0,
      paymentMethod: this.form.paymentMethod,
      chequeNumber: this.form.chequeNumber?.trim() || undefined,
      bankName: this.form.bankName?.trim() || undefined,
      chequeDate: this.form.chequeDate || undefined,
      expenseId: this.form.expenseId || undefined,
      invoiceId: this.form.invoiceId || undefined,
      ownerId: this.form.ownerId || undefined,
      payeeName: this.form.payeeName?.trim() || undefined,
      description: this.form.description?.trim() || undefined
    };

    this.isSaving.set(true);
    const request = this.isEditing()
      ? this.service.update(this.editingId, payload satisfies UpdatePaymentVoucherRequest)
      : this.service.create(payload satisfies CreatePaymentVoucherRequest);

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

  markPrinted(voucher: PaymentVoucher): void {
    this.service.markPrinted(voucher.id).subscribe({
      next: () => {
        this.load();
        this.toast.success('messages.updated');
      },
      error: () => this.toast.error()
    });
  }

  delete(voucher: PaymentVoucher): void {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) {
      return;
    }
    this.service.delete(voucher.id).subscribe({
      next: () => {
        this.load();
        this.toast.success('messages.deleted');
      },
      error: () => this.toast.error()
    });
  }

  getPaymentMethodLabel(method: PaymentMethod): string {
    return PaymentMethod[method] ?? 'Unknown';
  }

  private emptyForm(): CreatePaymentVoucherRequest {
    return {
      amount: 0,
      paymentMethod: PaymentMethod.BankTransfer,
      chequeNumber: '',
      bankName: '',
      chequeDate: this.today,
      expenseId: undefined,
      invoiceId: undefined,
      ownerId: undefined,
      payeeName: '',
      description: ''
    };
  }
}

