import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { ClientService } from '../../services/client.service';
import { MaintenanceRequestService } from '../../services/maintenance-request.service';
import { InvoiceService } from '../../services/invoice.service';
import { Client, MaintenanceRequest, MaintenanceRequestStatus, Invoice, InvoiceStatus } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-clients',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './clients.component.html',
  styleUrls: ['./clients.component.css']
})
export class ClientsComponent implements OnInit {
  clients = signal<Client[]>([]);
  currentPage = signal(1);
  readonly pageSize = 10;
  isLoading = signal(false);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);

  // Detail view state
  selectedClient = signal<Client | null>(null);
  clientRequests = signal<MaintenanceRequest[]>([]);
  clientInvoices = signal<Invoice[]>([]);
  showDetail = signal(false);

  // Maintenance request modal
  showRequestModal = signal(false);
  isEditingRequest = signal(false);
  isSavingRequest = signal(false);
  private editingRequestId = '';

  MaintenanceRequestStatus = MaintenanceRequestStatus;
  InvoiceStatus = InvoiceStatus;

  form = {
    name: '',
    companyName: '',
    email: '',
    phone: '',
    address: '',
    notes: ''
  };
  private editingId = '';

  requestForm = {
    title: '',
    description: '',
    equipmentDescription: '',
    requestDate: '',
    notes: '',
    status: MaintenanceRequestStatus.Pending
  };

  constructor(
    private clientService: ClientService,
    private requestService: MaintenanceRequestService,
    private invoiceService: InvoiceService,
    public auth: AuthService,
    private translation: TranslationService,
    private toast: ToastService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);
    this.clientService.getAll().subscribe({
      next: d => { this.clients.set(d); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { name: '', companyName: '', email: '', phone: '', address: '', notes: '' };
    this.showModal.set(true);
  }

  openEdit(client: Client) {
    this.isEditing.set(true);
    this.editingId = client.id;
    this.form = {
      name: client.name,
      companyName: client.companyName ?? '',
      email: client.email,
      phone: client.phone ?? '',
      address: client.address ?? '',
      notes: client.notes ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    const dto = {
      name: this.form.name,
      companyName: this.form.companyName || null,
      email: this.form.email,
      phone: this.form.phone || null,
      address: this.form.address || null,
      notes: this.form.notes || null
    };
    const obs = this.isEditing()
      ? this.clientService.update(this.editingId, dto)
      : this.clientService.create(dto);
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
    if (!confirm(this.translation.translate('clients.deleteConfirm'))) return;
    this.clientService.delete(id).subscribe({
      next: () => { this.load(); this.toast.success('messages.deleted'); },
      error: () => this.toast.error()
    });
  }

  viewClient(client: Client) {
    this.selectedClient.set(client);
    this.showDetail.set(true);
    this.clientRequests.set([]);
    this.clientInvoices.set([]);
    this.requestService.getByClient(client.id).subscribe({
      next: r => this.clientRequests.set(r),
      error: () => {}
    });
    this.clientService.getInvoices(client.id).subscribe({
      next: inv => this.clientInvoices.set(inv),
      error: () => {}
    });
  }

  backToList() {
    this.showDetail.set(false);
    this.selectedClient.set(null);
  }

  openAddRequest() {
    this.isEditingRequest.set(false);
    this.editingRequestId = '';
    this.requestForm = {
      title: '',
      description: '',
      equipmentDescription: '',
      requestDate: new Date().toISOString().substring(0, 10),
      notes: '',
      status: MaintenanceRequestStatus.Pending
    };
    this.showRequestModal.set(true);
  }

  openEditRequest(req: MaintenanceRequest) {
    this.isEditingRequest.set(true);
    this.editingRequestId = req.id;
    this.requestForm = {
      title: req.title,
      description: req.description ?? '',
      equipmentDescription: req.equipmentDescription ?? '',
      requestDate: req.requestDate ? req.requestDate.substring(0, 10) : '',
      notes: req.notes ?? '',
      status: req.status
    };
    this.showRequestModal.set(true);
  }

  closeRequestModal() { this.showRequestModal.set(false); }

  saveRequest() {
    const client = this.selectedClient();
    if (!client) return;
    this.isSavingRequest.set(true);
    if (this.isEditingRequest()) {
      const dto = {
        title: this.requestForm.title,
        description: this.requestForm.description || null,
        equipmentDescription: this.requestForm.equipmentDescription || null,
        requestDate: this.requestForm.requestDate || null,
        status: Number(this.requestForm.status),
        notes: this.requestForm.notes || null
      };
      this.requestService.update(this.editingRequestId, dto).subscribe({
        next: () => {
          this.isSavingRequest.set(false);
          this.showRequestModal.set(false);
          this.viewClient(client);
          this.toast.success('messages.updated');
        },
        error: () => { this.isSavingRequest.set(false); this.toast.error(); }
      });
    } else {
      const dto = {
        title: this.requestForm.title,
        description: this.requestForm.description || null,
        equipmentDescription: this.requestForm.equipmentDescription || null,
        requestDate: this.requestForm.requestDate || null,
        clientId: client.id,
        notes: this.requestForm.notes || null
      };
      this.requestService.create(dto).subscribe({
        next: () => {
          this.isSavingRequest.set(false);
          this.showRequestModal.set(false);
          this.viewClient(client);
          this.load();
          this.toast.success('messages.created');
        },
        error: () => { this.isSavingRequest.set(false); this.toast.error(); }
      });
    }
  }

  deleteRequest(id: string) {
    const client = this.selectedClient();
    if (!client) return;
    if (!confirm(this.translation.translate('clients.requests.deleteConfirm'))) return;
    this.requestService.delete(id).subscribe({
      next: () => { this.viewClient(client); this.toast.success('messages.deleted'); },
      error: () => this.toast.error()
    });
  }

  getRequestStatusLabel(s: MaintenanceRequestStatus): string {
    const keys = [
      'clients.requests.statuses.pending',
      'clients.requests.statuses.underReview',
      'clients.requests.statuses.approved',
      'clients.requests.statuses.inProgress',
      'clients.requests.statuses.completed',
      'clients.requests.statuses.rejected',
      'clients.requests.statuses.cancelled'
    ];
    return this.translation.translate(keys[s] ?? keys[0]);
  }

  getRequestStatusClass(s: MaintenanceRequestStatus): string {
    return ['bg-secondary', 'bg-info', 'bg-primary', 'bg-warning', 'bg-success', 'bg-danger', 'bg-dark'][s] ?? 'bg-secondary';
  }

  getInvoiceStatusLabel(s: InvoiceStatus): string {
    const keys = ['invoices.statuses.draft', 'invoices.statuses.sent', 'invoices.statuses.paid', 'invoices.statuses.overdue', 'invoices.statuses.cancelled'];
    return this.translation.translate(keys[s] ?? keys[0]);
  }

  getInvoiceStatusClass(s: InvoiceStatus): string {
    return ['bg-secondary', 'bg-primary', 'bg-success', 'bg-danger', 'bg-dark'][s] ?? 'bg-secondary';
  }

  getTotalInvoiceAmount(): number {
    return this.clientInvoices().reduce((sum, i) => sum + (i.totalAmount ?? 0), 0);
  }
}
