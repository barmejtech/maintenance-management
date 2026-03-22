import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { InvoiceService } from '../../services/invoice.service';
import { Invoice, InvoiceStatus } from '../../models';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h2>💰 Invoices</h2>
        <a routerLink="/dashboard" class="btn-back">← Dashboard</a>
      </div>
      <div class="table-container">
        <table class="data-table">
          <thead>
            <tr>
              <th>Invoice #</th>
              <th>Client</th>
              <th>Issue Date</th>
              <th>Due Date</th>
              <th>Total</th>
              <th>Status</th>
            </tr>
          </thead>
          <tbody>
            @for (inv of invoices(); track inv.id) {
              <tr>
                <td><strong>{{ inv.invoiceNumber }}</strong></td>
                <td>{{ inv.clientName }}</td>
                <td>{{ inv.issueDate | date:'mediumDate' }}</td>
                <td>{{ inv.dueDate ? (inv.dueDate | date:'mediumDate') : '—' }}</td>
                <td><strong>\${{ inv.totalAmount.toFixed(2) }}</strong></td>
                <td><span class="badge" [class]="getStatusClass(inv.status)">{{ getStatusLabel(inv.status) }}</span></td>
              </tr>
            }
          </tbody>
        </table>
        @if (invoices().length === 0) {
          <div class="empty-state">No invoices found.</div>
        }
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; }
    .page-header h2 { font-size: 1.5rem; color: #333; }
    .btn-back { color: #0f3460; text-decoration: none; font-weight: 500; }
    .table-container { background: white; border-radius: 0.75rem; box-shadow: 0 2px 8px rgba(0,0,0,0.06); overflow: hidden; }
    .data-table { width: 100%; border-collapse: collapse; }
    .data-table th { background: #f8f9fa; padding: 0.875rem 1rem; text-align: left; font-size: 0.85rem; font-weight: 600; color: #555; border-bottom: 1px solid #e0e0e0; }
    .data-table td { padding: 0.875rem 1rem; border-bottom: 1px solid #f0f0f0; font-size: 0.9rem; color: #333; }
    .data-table tr:last-child td { border-bottom: none; }
    .badge { padding: 0.25rem 0.75rem; border-radius: 1rem; font-size: 0.75rem; font-weight: 600; }
    .s-draft { background: #f5f5f5; color: #616161; }
    .s-sent { background: #e3f2fd; color: #1565c0; }
    .s-paid { background: #d4edda; color: #155724; }
    .s-overdue { background: #f8d7da; color: #721c24; }
    .s-cancelled { background: #f0f0f0; color: #888; }
    .empty-state { text-align: center; padding: 3rem; color: #888; }
  `]
})
export class InvoicesComponent implements OnInit {
  invoices = signal<Invoice[]>([]);

  constructor(private service: InvoiceService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.invoices.set(d), error: () => {} });
  }

  getStatusLabel(s: InvoiceStatus): string { return ['Draft', 'Sent', 'Paid', 'Overdue', 'Cancelled'][s]; }
  getStatusClass(s: InvoiceStatus): string { return ['s-draft', 's-sent', 's-paid', 's-overdue', 's-cancelled'][s]; }
}
