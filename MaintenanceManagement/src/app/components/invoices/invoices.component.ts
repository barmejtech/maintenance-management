import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { InvoiceService } from '../../services/invoice.service';
import { Invoice, InvoiceStatus } from '../../models';

@Component({
  selector: 'app-invoices',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './invoices.component.html',
  styleUrls: ['./invoices.component.css']
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
