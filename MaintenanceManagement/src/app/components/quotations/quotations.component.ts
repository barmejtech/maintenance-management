import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { QuotationService } from '../../services/quotation.service';
import { ClientService } from '../../services/client.service';
import { MaintenanceRequestService } from '../../services/maintenance-request.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';
import {
  Quotation,
  QuotationStatus,
  Client,
  MaintenanceRequest,
  CreateQuotationLineItemRequest
} from '../../models';

@Component({
  selector: 'app-quotations',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './quotations.component.html',
  styleUrls: ['./quotations.component.css']
})
export class QuotationsComponent implements OnInit {
  quotations = signal<Quotation[]>([]);
  clients = signal<Client[]>([]);
  maintenanceRequests = signal<MaintenanceRequest[]>([]);

  currentPage = signal(1);
  readonly pageSize = 10;
  showModal = signal(false);
  showDetailModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  isSending = signal<Record<string, boolean>>({});

  selectedQuotation = signal<Quotation | null>(null);
  QuotationStatus = QuotationStatus;

  lineItems = signal<CreateQuotationLineItemRequest[]>([]);

  form = {
    clientId: '',
    clientName: '',
    clientEmail: '',
    clientAddress: '',
    clientPhone: '',
    maintenanceRequestId: '',
    validUntil: '',
    estimatedDurationDays: 7,
    taxRate: 0,
    notes: '',
    termsAndConditions: '',
    status: QuotationStatus.Draft
  };
  private editingId = '';

  constructor(
    private service: QuotationService,
    private clientService: ClientService,
    private requestService: MaintenanceRequestService,
    private translation: TranslationService,
    private toast: ToastService,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.load();
    this.loadClients();
    this.loadRequests();
  }

  load() {
    this.service.getAll().subscribe({ next: d => this.quotations.set(d), error: () => {} });
  }

  loadClients() {
    this.clientService.getAll().subscribe({ next: d => this.clients.set(d), error: () => {} });
  }

  loadRequests() {
    this.requestService.getAll().subscribe({ next: d => this.maintenanceRequests.set(d), error: () => {} });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    const defaultDate = new Date();
    defaultDate.setDate(defaultDate.getDate() + 30);
    this.form = {
      clientId: '', clientName: '', clientEmail: '', clientAddress: '', clientPhone: '',
      maintenanceRequestId: '', validUntil: defaultDate.toISOString().substring(0, 10),
      estimatedDurationDays: 7, taxRate: 0, notes: '', termsAndConditions: '', status: QuotationStatus.Draft
    };
    this.lineItems.set([{ description: '', quantity: 1, unitPrice: 0 }]);
    this.showModal.set(true);
  }

  openEdit(q: Quotation) {
    this.isEditing.set(true);
    this.editingId = q.id;
    this.form = {
      clientId: q.clientId ?? '',
      clientName: q.clientName,
      clientEmail: q.clientEmail ?? '',
      clientAddress: q.clientAddress ?? '',
      clientPhone: q.clientPhone ?? '',
      maintenanceRequestId: q.maintenanceRequestId ?? '',
      validUntil: q.validUntil ? q.validUntil.substring(0, 10) : '',
      estimatedDurationDays: q.estimatedDurationDays,
      taxRate: q.taxRate,
      notes: q.notes ?? '',
      termsAndConditions: q.termsAndConditions ?? '',
      status: q.status
    };
    this.lineItems.set(q.lineItems.map(li => ({
      description: li.description,
      quantity: li.quantity,
      unitPrice: li.unitPrice
    })));
    this.showModal.set(true);
  }

  viewDetail(q: Quotation) {
    this.selectedQuotation.set(q);
    this.showDetailModal.set(true);
  }

  closeModal() {
    this.showModal.set(false);
  }

  closeDetailModal() {
    this.showDetailModal.set(false);
    this.selectedQuotation.set(null);
  }

  onClientSelect() {
    const clientId = this.form.clientId;
    if (!clientId) {
      this.form.clientName = '';
      this.form.clientEmail = '';
      this.form.clientAddress = '';
      this.form.clientPhone = '';
      return;
    }
    const client = this.clients().find(c => c.id === clientId);
    if (client) {
      this.form.clientName = client.name;
      this.form.clientEmail = client.email ?? '';
      this.form.clientAddress = client.address ?? '';
      this.form.clientPhone = client.phone ?? '';
    }
  }

  addLineItem() {
    this.lineItems.update(items => [...items, { description: '', quantity: 1, unitPrice: 0 }]);
  }

  removeLineItem(index: number) {
    this.lineItems.update(items => items.filter((_, i) => i !== index));
  }

  getLineItemTotal(item: CreateQuotationLineItemRequest): number {
    return (item.quantity || 0) * (item.unitPrice || 0);
  }

  getSubTotal(): number {
    return this.lineItems().reduce((sum, li) => sum + this.getLineItemTotal(li), 0);
  }

  getTaxAmount(): number {
    return this.getSubTotal() * (this.form.taxRate || 0) / 100;
  }

  getGrandTotal(): number {
    return this.getSubTotal() + this.getTaxAmount();
  }

  save() {
    this.isSaving.set(true);
    const dto: any = {
      clientName: this.form.clientName,
      clientEmail: this.form.clientEmail || null,
      clientAddress: this.form.clientAddress || null,
      clientPhone: this.form.clientPhone || null,
      validUntil: this.form.validUntil || null,
      estimatedDurationDays: Number(this.form.estimatedDurationDays),
      taxRate: Number(this.form.taxRate),
      notes: this.form.notes || null,
      termsAndConditions: this.form.termsAndConditions || null,
      maintenanceRequestId: this.form.maintenanceRequestId || null,
      clientId: this.form.clientId || null,
      lineItems: this.lineItems().filter(li => li.description.trim()).map(li => ({
        description: li.description,
        quantity: Number(li.quantity),
        unitPrice: Number(li.unitPrice)
      }))
    };
    if (this.isEditing()) {
      dto.status = Number(this.form.status);
    }
    const obs = this.isEditing()
      ? this.service.update(this.editingId, dto)
      : this.service.create(dto);
    obs.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showModal.set(false);
        this.load();
        this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created');
      },
      error: () => { this.isSaving.set(false); this.toast.error(); }
    });
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('quotations.deleteConfirm'))) return;
    this.service.delete(id).subscribe({
      next: () => { this.load(); this.toast.success('messages.deleted'); },
      error: () => this.toast.error()
    });
  }

  sendEmail(q: Quotation) {
    if (!q.clientEmail) {
      this.toast.error('quotations.noEmailError');
      return;
    }
    this.isSending.update(s => ({ ...s, [q.id]: true }));
    this.service.sendEmail(q.id).subscribe({
      next: () => {
        this.isSending.update(s => ({ ...s, [q.id]: false }));
        this.load();
        this.toast.success('quotations.emailSent');
      },
      error: () => {
        this.isSending.update(s => ({ ...s, [q.id]: false }));
        this.toast.error();
      }
    });
  }

  getStatusLabel(s: QuotationStatus): string {
    const keys = [
      'quotations.statuses.draft', 'quotations.statuses.sent', 'quotations.statuses.accepted',
      'quotations.statuses.rejected', 'quotations.statuses.expired', 'quotations.statuses.cancelled'
    ];
    return this.translation.translate(keys[s] ?? keys[0]);
  }

  getStatusClass(s: QuotationStatus): string {
    return ['bg-secondary', 'bg-primary', 'bg-success', 'bg-danger', 'bg-warning text-dark', 'bg-dark'][s] ?? 'bg-secondary';
  }

  getStatusIcon(s: QuotationStatus): string {
    return ['bi-pencil', 'bi-send', 'bi-check-circle', 'bi-x-circle', 'bi-clock-history', 'bi-slash-circle'][s] ?? 'bi-circle';
  }

  isExpired(q: Quotation): boolean {
    return new Date(q.validUntil) < new Date();
  }

  // Stats
  getTotalCount(): number { return this.quotations().length; }
  getSentCount(): number { return this.quotations().filter(q => q.status === QuotationStatus.Sent).length; }
  getAcceptedCount(): number { return this.quotations().filter(q => q.status === QuotationStatus.Accepted).length; }
  getTotalValue(): number { return this.quotations().reduce((s, q) => s + (q.totalAmount ?? 0), 0); }
}
