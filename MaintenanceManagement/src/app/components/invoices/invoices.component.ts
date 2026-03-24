import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { InvoiceService } from '../../services/invoice.service';
import { Invoice, InvoiceStatus } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { PdfService } from '../../services/pdf.service';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe],
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.css']
})
export class InvoicesComponent implements OnInit {
  invoices = signal<Invoice[]>([]);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  InvoiceStatus = InvoiceStatus;

  form = {
    invoiceNumber: '',
    clientName: '',
    clientEmail: '',
    clientAddress: '',
    issueDate: '',
    dueDate: '',
    subTotal: 0,
    taxRate: 0,
    notes: '',
    status: InvoiceStatus.Draft
  };
  private editingId = '';

  constructor(private service: InvoiceService, private translation: TranslationService, private pdf: PdfService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.service.getAll().subscribe({ next: d => this.invoices.set(d), error: () => {} });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { invoiceNumber: '', clientName: '', clientEmail: '', clientAddress: '', issueDate: '', dueDate: '', subTotal: 0, taxRate: 0, notes: '', status: InvoiceStatus.Draft };
    this.showModal.set(true);
  }

  openEdit(inv: Invoice) {
    this.isEditing.set(true);
    this.editingId = inv.id;
    this.form = {
      invoiceNumber: inv.invoiceNumber,
      clientName: inv.clientName,
      clientEmail: inv.clientEmail ?? '',
      clientAddress: inv.clientAddress ?? '',
      issueDate: inv.issueDate ? inv.issueDate.substring(0, 10) : '',
      dueDate: inv.dueDate ? inv.dueDate.substring(0, 10) : '',
      subTotal: inv.subTotal,
      taxRate: inv.taxRate,
      notes: inv.notes ?? '',
      status: inv.status
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    const taxAmount = Number(this.form.subTotal) * Number(this.form.taxRate) / 100;
    const dto = {
      ...this.form,
      subTotal: Number(this.form.subTotal),
      taxRate: Number(this.form.taxRate),
      taxAmount,
      totalAmount: Number(this.form.subTotal) + taxAmount,
      status: Number(this.form.status),
      issueDate: this.form.issueDate || null,
      dueDate: this.form.dueDate || null
    };
    const obs = this.isEditing()
      ? this.service.update(this.editingId, dto)
      : this.service.create(dto);
    obs.subscribe({
      next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
      error: () => this.isSaving.set(false)
    });
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('invoices.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => this.load(), error: () => {} });
  }

  getStatusLabel(s: InvoiceStatus): string {
    const keys = ['invoices.statuses.draft', 'invoices.statuses.sent', 'invoices.statuses.paid', 'invoices.statuses.overdue', 'invoices.statuses.cancelled'];
    return this.translation.translate(keys[s] ?? keys[0]);
  }
  getStatusClass(s: InvoiceStatus): string { return ['bg-secondary', 'bg-primary', 'bg-success', 'bg-danger', 'bg-dark'][s]; }

  generatePdf(inv: Invoice): void {
    this.pdf.generateInvoicePdf(inv);
  }
}
