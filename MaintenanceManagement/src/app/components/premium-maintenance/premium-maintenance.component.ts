import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { PremiumMaintenanceService } from '../../services/premium-maintenance.service';
import { PaymentService } from '../../services/payment.service';
import { ClientService } from '../../services/client.service';
import {
  PremiumService, PremiumMaintenanceRequest, Client, PaymentCheckout,
  PremiumServiceType, PremiumMaintenanceStatus, PaymentStatus, PaymentMethod
} from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-premium-maintenance',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe, NgxPaginationModule],
  templateUrl: './premium-maintenance.component.html',
  styleUrls: ['./premium-maintenance.component.css']
})
export class PremiumMaintenanceComponent implements OnInit {
  // Tab state
  activeTab = signal<'services' | 'requests'>('services');

  // Services
  services = signal<PremiumService[]>([]);
  servicesPage = signal(1);
  readonly pageSize = 9;
  isLoadingServices = signal(false);
  showServiceModal = signal(false);
  isEditingService = signal(false);
  isSavingService = signal(false);
  private editingServiceId = '';

  serviceForm = {
    name: '',
    description: '',
    serviceType: PremiumServiceType.Preventive,
    price: 0,
    durationHours: 1,
    priorityLevel: 1,
    isActive: true,
    features: ''
  };

  // Requests
  requests = signal<PremiumMaintenanceRequest[]>([]);
  requestsPage = signal(1);
  isLoadingRequests = signal(false);
  showRequestModal = signal(false);
  isSavingRequest = signal(false);
  clients = signal<Client[]>([]);

  requestForm = {
    clientId: '',
    premiumServiceId: '',
    scheduledDate: '',
    notes: '',
    address: ''
  };

  // Payment modal
  showPaymentModal = signal(false);
  isProcessingPayment = signal(false);
  selectedRequest = signal<PremiumMaintenanceRequest | null>(null);
  checkoutData = signal<PaymentCheckout | null>(null);
  paymentForm = {
    cardNumber: '',
    cardName: '',
    expiry: '',
    cvv: '',
    method: PaymentMethod.OnlinePayment
  };

  // Enums for template
  PremiumServiceType = PremiumServiceType;
  PremiumMaintenanceStatus = PremiumMaintenanceStatus;
  PaymentStatus = PaymentStatus;
  PaymentMethod = PaymentMethod;

  serviceTypeOptions = [
    { value: PremiumServiceType.Preventive, label: 'Preventive' },
    { value: PremiumServiceType.Emergency, label: 'Emergency' },
    { value: PremiumServiceType.Inspection, label: 'Inspection' },
    { value: PremiumServiceType.FullOverhaul, label: 'Full Overhaul' },
    { value: PremiumServiceType.Corrective, label: 'Corrective' },
    { value: PremiumServiceType.Consultation, label: 'Consultation' }
  ];

  priorityOptions = [
    { value: 0, label: 'Low' },
    { value: 1, label: 'Medium' },
    { value: 2, label: 'High' },
    { value: 3, label: 'Critical' }
  ];

  constructor(
    private pmService: PremiumMaintenanceService,
    private paymentService: PaymentService,
    private clientService: ClientService,
    public auth: AuthService,
    private translation: TranslationService,
    private toast: ToastService
  ) {}

  ngOnInit() {
    this.loadServices();
    this.loadRequests();
    this.clientService.getAll().subscribe({ next: c => this.clients.set(c) });
  }

  setTab(tab: 'services' | 'requests') {
    this.activeTab.set(tab);
  }

  isManagerOrAdmin(): boolean {
    return this.auth.isManager();
  }

  // ======================== SERVICES ========================

  loadServices() {
    this.isLoadingServices.set(true);
    this.pmService.getAllServices().subscribe({
      next: d => { this.services.set(d); this.isLoadingServices.set(false); },
      error: () => this.isLoadingServices.set(false)
    });
  }

  openAddService() {
    this.isEditingService.set(false);
    this.editingServiceId = '';
    this.serviceForm = {
      name: '', description: '', serviceType: PremiumServiceType.Preventive,
      price: 0, durationHours: 1, priorityLevel: 1, isActive: true, features: ''
    };
    this.showServiceModal.set(true);
  }

  openEditService(s: PremiumService) {
    this.isEditingService.set(true);
    this.editingServiceId = s.id;
    this.serviceForm = {
      name: s.name,
      description: s.description ?? '',
      serviceType: s.serviceType,
      price: s.price,
      durationHours: s.durationHours,
      priorityLevel: s.priorityLevel,
      isActive: s.isActive,
      features: s.features ?? ''
    };
    this.showServiceModal.set(true);
  }

  closeServiceModal() { this.showServiceModal.set(false); }

  saveService() {
    this.isSavingService.set(true);
    const dto = {
      name: this.serviceForm.name,
      description: this.serviceForm.description || undefined,
      serviceType: Number(this.serviceForm.serviceType),
      price: Number(this.serviceForm.price),
      durationHours: Number(this.serviceForm.durationHours),
      priorityLevel: Number(this.serviceForm.priorityLevel),
      isActive: this.serviceForm.isActive,
      features: this.serviceForm.features || undefined
    };
    const obs = this.isEditingService()
      ? this.pmService.updateService(this.editingServiceId, dto)
      : this.pmService.createService(dto);
    obs.subscribe({
      next: () => {
        this.isSavingService.set(false);
        this.showServiceModal.set(false);
        this.loadServices();
        this.toast.success(this.isEditingService() ? 'messages.updated' : 'messages.created');
      },
      error: () => { this.isSavingService.set(false); this.toast.error(); }
    });
  }

  deleteService(id: string) {
    if (!confirm(this.translation.translate('premiumMaintenance.deleteServiceConfirm'))) return;
    this.pmService.deleteService(id).subscribe({
      next: () => { this.loadServices(); this.toast.success('messages.deleted'); },
      error: () => this.toast.error()
    });
  }

  // ======================== REQUESTS ========================

  loadRequests() {
    this.isLoadingRequests.set(true);
    this.pmService.getAllRequests().subscribe({
      next: d => { this.requests.set(d); this.isLoadingRequests.set(false); },
      error: () => this.isLoadingRequests.set(false)
    });
  }

  openNewRequest() {
    this.requestForm = { clientId: '', premiumServiceId: '', scheduledDate: '', notes: '', address: '' };
    this.showRequestModal.set(true);
  }

  closeRequestModal() { this.showRequestModal.set(false); }

  saveRequest() {
    this.isSavingRequest.set(true);
    const dto = {
      clientId: this.requestForm.clientId,
      premiumServiceId: this.requestForm.premiumServiceId,
      scheduledDate: this.requestForm.scheduledDate || undefined,
      notes: this.requestForm.notes || undefined,
      address: this.requestForm.address || undefined
    };
    this.pmService.createRequest(dto).subscribe({
      next: () => {
        this.isSavingRequest.set(false);
        this.showRequestModal.set(false);
        this.loadRequests();
        this.setTab('requests');
        this.toast.success('messages.created');
      },
      error: () => { this.isSavingRequest.set(false); this.toast.error(); }
    });
  }

  deleteRequest(id: string) {
    if (!confirm(this.translation.translate('premiumMaintenance.deleteRequestConfirm'))) return;
    this.pmService.deleteRequest(id).subscribe({
      next: () => { this.loadRequests(); this.toast.success('messages.deleted'); },
      error: () => this.toast.error()
    });
  }

  // ======================== PAYMENT ========================

  openPayment(req: PremiumMaintenanceRequest) {
    this.selectedRequest.set(req);
    this.paymentForm = { cardNumber: '', cardName: '', expiry: '', cvv: '', method: PaymentMethod.OnlinePayment };
    this.checkoutData.set(null);
    this.showPaymentModal.set(true);
  }

  closePaymentModal() { this.showPaymentModal.set(false); this.selectedRequest.set(null); }

  initiatePayment() {
    const req = this.selectedRequest();
    if (!req) return;
    this.isProcessingPayment.set(true);
    this.paymentService.initiatePayment({
      premiumMaintenanceRequestId: req.id,
      paymentMethod: Number(this.paymentForm.method)
    }).subscribe({
      next: checkout => {
        this.checkoutData.set(checkout);
        this.isProcessingPayment.set(false);
      },
      error: () => { this.isProcessingPayment.set(false); this.toast.error(); }
    });
  }

  confirmPayment() {
    const checkout = this.checkoutData();
    if (!checkout) return;
    this.isProcessingPayment.set(true);
    // NOTE: In production, this transaction ID is provided by the payment gateway (e.g. Stripe/PayPal)
    // after card processing. This simulated ID is used for development/testing only.
    const simulatedTransactionId = `TXN-${Date.now()}-${Math.random().toString(36).substring(2, 9).toUpperCase()}`;
    this.paymentService.confirmPayment(checkout.paymentId, simulatedTransactionId).subscribe({
      next: () => {
        this.isProcessingPayment.set(false);
        this.showPaymentModal.set(false);
        this.selectedRequest.set(null);
        this.checkoutData.set(null);
        this.loadRequests();
        this.toast.success('premiumMaintenance.paymentSuccess');
      },
      error: () => { this.isProcessingPayment.set(false); this.toast.error(); }
    });
  }

  // ======================== HELPERS ========================

  getServiceTypeName(t: PremiumServiceType): string {
    return this.serviceTypeOptions.find(o => o.value === t)?.label ?? '';
  }

  getServiceTypeClass(t: PremiumServiceType): string {
    return ['badge-preventive', 'badge-emergency', 'badge-inspection', 'badge-overhaul', 'badge-corrective', 'badge-consultation'][t] ?? '';
  }

  getPriorityName(p: number): string {
    return this.priorityOptions.find(o => o.value === p)?.label ?? '';
  }

  getPriorityClass(p: number): string {
    return ['text-secondary', 'text-primary', 'text-warning', 'text-danger'][p] ?? '';
  }

  getStatusName(s: PremiumMaintenanceStatus): string {
    const names = ['Draft', 'Payment Pending', 'Paid', 'In Progress', 'Completed', 'Cancelled', 'Refunded'];
    return names[s] ?? '';
  }

  getStatusClass(s: PremiumMaintenanceStatus): string {
    return ['bg-secondary', 'bg-warning', 'bg-success', 'bg-primary', 'bg-success', 'bg-danger', 'bg-info'][s] ?? 'bg-secondary';
  }

  getPaymentStatusName(s?: PaymentStatus): string {
    if (s === undefined || s === null) return '-';
    const names = ['Pending', 'Processing', 'Completed', 'Failed', 'Refunded', 'Cancelled'];
    return names[s] ?? '-';
  }

  canPay(req: PremiumMaintenanceRequest): boolean {
    return req.status === PremiumMaintenanceStatus.Draft || req.status === PremiumMaintenanceStatus.PaymentPending;
  }
}
