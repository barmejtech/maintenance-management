import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { InvoiceService } from '../../services/invoice.service';
import { ClientService } from '../../services/client.service';
import { TaskOrderService } from '../../services/task-order.service';
import { EquipmentService } from '../../services/equipment.service';
import { Invoice, InvoiceStatus, Client, TaskOrder, Equipment, EquipmentStatus } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { PdfService } from '../../services/pdf.service';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.css']
})
export class InvoicesComponent implements OnInit {
  invoices = signal<Invoice[]>([]);
  clients = signal<Client[]>([]);
  taskOrders = signal<TaskOrder[]>([]);
  equipmentMap = signal<Record<string, Equipment>>({});
  currentPage = signal(1);
  readonly pageSize = 10;
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  InvoiceStatus = InvoiceStatus;
  EquipmentStatus = EquipmentStatus;

  selectedTaskOrder = signal<TaskOrder | null>(null);
  selectedEquipment = signal<Equipment | null>(null);

  form = {
    invoiceNumber: '',
    clientId: '',
    clientName: '',
    clientEmail: '',
    clientAddress: '',
    taskOrderId: '',
    issueDate: '',
    dueDate: '',
    subTotal: 0,
    taxRate: 0,
    notes: '',
    status: InvoiceStatus.Draft
  };
  private editingId = '';

  constructor(
    private service: InvoiceService,
    private clientService: ClientService,
    private taskOrderService: TaskOrderService,
    private equipmentService: EquipmentService,
    private translation: TranslationService,
    private pdf: PdfService,
    private toast: ToastService,
    public auth: AuthService
  ) {}

  ngOnInit() {
    this.load();
    this.loadClients();
    this.loadTaskOrders();
  }

  load() {
    this.service.getAll().subscribe({ next: d => this.invoices.set(d), error: () => {} });
  }

  loadClients() {
    this.clientService.getAll().subscribe({ next: d => this.clients.set(d), error: () => {} });
  }

  loadTaskOrders() {
    this.taskOrderService.getAll().subscribe({
      next: orders => {
        this.taskOrders.set(orders);
        const equipmentIds = [...new Set(orders.filter(o => o.equipmentId).map(o => o.equipmentId!))];
        if (equipmentIds.length > 0) {
          this.equipmentService.getAll().subscribe({
            next: eqs => {
              const map: Record<string, Equipment> = {};
              eqs.forEach(e => { map[e.id] = e; });
              this.equipmentMap.set(map);
            },
            error: () => {}
          });
        }
      },
      error: () => {}
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { invoiceNumber: '', clientId: '', clientName: '', clientEmail: '', clientAddress: '', taskOrderId: '', issueDate: '', dueDate: '', subTotal: 0, taxRate: 0, notes: '', status: InvoiceStatus.Draft };
    this.selectedTaskOrder.set(null);
    this.selectedEquipment.set(null);
    this.showModal.set(true);
  }

  openEdit(inv: Invoice) {
    this.isEditing.set(true);
    this.editingId = inv.id;
    this.form = {
      invoiceNumber: inv.invoiceNumber,
      clientId: inv.clientId ?? '',
      clientName: inv.clientName,
      clientEmail: inv.clientEmail ?? '',
      clientAddress: inv.clientAddress ?? '',
      taskOrderId: inv.taskOrderId ?? '',
      issueDate: inv.issueDate ? inv.issueDate.substring(0, 10) : '',
      dueDate: inv.dueDate ? inv.dueDate.substring(0, 10) : '',
      subTotal: inv.subTotal,
      taxRate: inv.taxRate,
      notes: inv.notes ?? '',
      status: inv.status
    };
    // Restore selected task order and equipment
    if (inv.taskOrderId) {
      const to = this.taskOrders().find(t => t.id === inv.taskOrderId) ?? null;
      this.selectedTaskOrder.set(to);
      if (to?.equipmentId) {
        this.selectedEquipment.set(this.equipmentMap()[to.equipmentId] ?? null);
      } else {
        this.selectedEquipment.set(null);
      }
    } else {
      this.selectedTaskOrder.set(null);
      this.selectedEquipment.set(null);
    }
    this.showModal.set(true);
  }

  onClientSelect() {
    const clientId = this.form.clientId;
    if (!clientId) {
      this.form.clientName = '';
      this.form.clientEmail = '';
      this.form.clientAddress = '';
      return;
    }
    const client = this.clients().find(c => c.id === clientId);
    if (client) {
      this.form.clientName = client.name;
      this.form.clientEmail = client.email ?? '';
      this.form.clientAddress = client.address ?? '';
    }
  }

  onTaskOrderSelect() {
    const taskOrderId = this.form.taskOrderId;
    if (!taskOrderId) {
      this.selectedTaskOrder.set(null);
      this.selectedEquipment.set(null);
      return;
    }
    const taskOrder = this.taskOrders().find(t => t.id === taskOrderId) ?? null;
    this.selectedTaskOrder.set(taskOrder);
    if (taskOrder?.equipmentId) {
      this.selectedEquipment.set(this.equipmentMap()[taskOrder.equipmentId] ?? null);
    } else {
      this.selectedEquipment.set(null);
    }
  }

  closeModal() {
    this.showModal.set(false);
    this.selectedTaskOrder.set(null);
    this.selectedEquipment.set(null);
  }

  save() {
    this.isSaving.set(true);
    const taxAmount = Number(this.form.subTotal) * Number(this.form.taxRate) / 100;
    const dto = {
      ...this.form,
      clientId: this.form.clientId || null,
      taskOrderId: this.form.taskOrderId || null,
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
      next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created'); },
      error: () => { this.isSaving.set(false); this.toast.error(); }
    });
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('invoices.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => { this.load(); this.toast.success('messages.deleted'); }, error: () => this.toast.error() });
  }

  getStatusLabel(s: InvoiceStatus): string {
    const keys = ['invoices.statuses.draft', 'invoices.statuses.sent', 'invoices.statuses.paid', 'invoices.statuses.overdue', 'invoices.statuses.cancelled'];
    return this.translation.translate(keys[s] ?? keys[0]);
  }
  getStatusClass(s: InvoiceStatus): string { return ['bg-secondary', 'bg-primary', 'bg-success', 'bg-danger', 'bg-dark'][s]; }
  getStatusIcon(s: InvoiceStatus): string { return ['bi-pencil', 'bi-send', 'bi-check-circle', 'bi-exclamation-circle', 'bi-x-circle'][s] ?? 'bi-circle'; }

  getEquipmentStatusLabel(s: EquipmentStatus): string {
    const keys = ['asset.status.operational', 'asset.status.underMaintenance', 'asset.status.outOfService', 'asset.status.decommissioned'];
    return this.translation.translate(keys[s] ?? keys[0]);
  }
  getEquipmentStatusClass(s: EquipmentStatus): string {
    return ['bg-success', 'bg-warning text-dark', 'bg-danger', 'bg-secondary'][s] ?? 'bg-secondary';
  }

  isOverdue(inv: Invoice): boolean {
    if (inv.status !== InvoiceStatus.Overdue) return false;
    if (!inv.dueDate) return false;
    return new Date(inv.dueDate) < new Date();
  }

  getPaidCount(): number { return this.invoices().filter(i => i.status === InvoiceStatus.Paid).length; }
  getOverdueCount(): number { return this.invoices().filter(i => i.status === InvoiceStatus.Overdue).length; }
  getTotalAmount(): number { return this.invoices().reduce((sum, i) => sum + (i.totalAmount ?? 0), 0); }

  generatePdf(inv: Invoice): void {
    this.pdf.generateInvoicePdf(inv);
  }
}
