import { Injectable } from '@angular/core';
import { Invoice, InvoiceStatus, MaintenanceReport } from '../models';

@Injectable({
  providedIn: 'root'
})
export class PdfService {
  private readonly companyName = 'MaintenancePro';
  private readonly companyTagline = 'Professional Maintenance Management System';

  private statusLabels: Record<InvoiceStatus, string> = {
    [InvoiceStatus.Draft]: 'Draft',
    [InvoiceStatus.Sent]: 'Sent',
    [InvoiceStatus.Paid]: 'Paid',
    [InvoiceStatus.Overdue]: 'Overdue',
    [InvoiceStatus.Cancelled]: 'Cancelled'
  };

  private statusColors: Record<InvoiceStatus, string> = {
    [InvoiceStatus.Draft]: '#6c757d',
    [InvoiceStatus.Sent]: '#0d6efd',
    [InvoiceStatus.Paid]: '#198754',
    [InvoiceStatus.Overdue]: '#dc3545',
    [InvoiceStatus.Cancelled]: '#212529'
  };

  private formatDate(dateStr?: string | null): string {
    if (!dateStr) return '—';
    return new Date(dateStr).toLocaleDateString('en-US', {
      year: 'numeric',
      month: 'long',
      day: 'numeric'
    });
  }

  private getSharedStyles(): string {
    return `
      * { margin: 0; padding: 0; box-sizing: border-box; }
      body {
        font-family: 'Segoe UI', Arial, sans-serif;
        font-size: 14px;
        color: #212529;
        background: #fff;
        padding: 0;
      }
      .pdf-wrapper {
        max-width: 800px;
        margin: 0 auto;
        padding: 40px;
        min-height: 100vh;
        display: flex;
        flex-direction: column;
      }

      /* ── HEADER ── */
      .pdf-header {
        display: flex;
        justify-content: space-between;
        align-items: flex-start;
        padding-bottom: 24px;
        border-bottom: 3px solid #0d6efd;
        margin-bottom: 32px;
      }
      .pdf-header-left .company-name {
        font-size: 26px;
        font-weight: 700;
        color: #0d6efd;
        letter-spacing: 0.5px;
      }
      .pdf-header-left .company-tagline {
        font-size: 12px;
        color: #6c757d;
        margin-top: 4px;
      }
      .pdf-header-right {
        text-align: right;
      }
      .pdf-header-right .doc-type {
        font-size: 22px;
        font-weight: 700;
        color: #212529;
        text-transform: uppercase;
        letter-spacing: 1px;
      }
      .pdf-header-right .doc-number {
        font-size: 13px;
        color: #6c757d;
        margin-top: 4px;
      }
      .pdf-header-right .doc-date {
        font-size: 12px;
        color: #6c757d;
        margin-top: 2px;
      }

      /* ── SECTION ── */
      .pdf-section {
        margin-bottom: 28px;
      }
      .pdf-section-title {
        font-size: 11px;
        font-weight: 700;
        text-transform: uppercase;
        letter-spacing: 1px;
        color: #6c757d;
        border-bottom: 1px solid #dee2e6;
        padding-bottom: 6px;
        margin-bottom: 12px;
      }
      .info-grid {
        display: grid;
        grid-template-columns: 1fr 1fr;
        gap: 10px 24px;
      }
      .info-item label {
        font-size: 11px;
        font-weight: 600;
        color: #6c757d;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        display: block;
        margin-bottom: 2px;
      }
      .info-item span {
        font-size: 14px;
        color: #212529;
      }

      /* ── TABLE ── */
      .pdf-table {
        width: 100%;
        border-collapse: collapse;
        margin-top: 8px;
      }
      .pdf-table th {
        background: #f8f9fa;
        font-size: 11px;
        font-weight: 700;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        color: #495057;
        padding: 10px 12px;
        border-bottom: 2px solid #dee2e6;
        text-align: left;
      }
      .pdf-table th.text-right,
      .pdf-table td.text-right { text-align: right; }
      .pdf-table td {
        padding: 10px 12px;
        border-bottom: 1px solid #f0f0f0;
        font-size: 13px;
        color: #212529;
      }
      .pdf-table tr:last-child td { border-bottom: none; }
      .pdf-table tfoot td {
        padding: 8px 12px;
        font-size: 13px;
        border-top: 1px solid #dee2e6;
      }
      .pdf-table tfoot tr.total-row td {
        font-weight: 700;
        font-size: 15px;
        border-top: 2px solid #212529;
        color: #0d6efd;
      }

      /* ── BADGE ── */
      .status-badge {
        display: inline-block;
        padding: 4px 12px;
        border-radius: 20px;
        font-size: 12px;
        font-weight: 600;
        color: #fff;
      }

      /* ── NOTES / CONTENT ── */
      .pdf-box {
        background: #f8f9fa;
        border-left: 4px solid #0d6efd;
        border-radius: 4px;
        padding: 14px 16px;
        font-size: 13px;
        color: #495057;
        line-height: 1.7;
        white-space: pre-wrap;
      }
      .pdf-box.recommendation {
        border-left-color: #198754;
      }

      /* ── METRICS ROW ── */
      .metrics-row {
        display: flex;
        gap: 16px;
        flex-wrap: wrap;
      }
      .metric-card {
        flex: 1;
        min-width: 140px;
        background: #f8f9fa;
        border: 1px solid #dee2e6;
        border-radius: 8px;
        padding: 14px 16px;
        text-align: center;
      }
      .metric-card .metric-value {
        font-size: 22px;
        font-weight: 700;
        color: #0d6efd;
      }
      .metric-card .metric-label {
        font-size: 11px;
        color: #6c757d;
        text-transform: uppercase;
        letter-spacing: 0.5px;
        margin-top: 4px;
      }

      /* ── FOOTER ── */
      .pdf-footer {
        margin-top: auto;
        padding-top: 20px;
        border-top: 2px solid #dee2e6;
        display: flex;
        justify-content: space-between;
        align-items: center;
      }
      .pdf-footer .footer-left {
        font-size: 11px;
        color: #adb5bd;
      }
      .pdf-footer .footer-right {
        font-size: 11px;
        color: #adb5bd;
        text-align: right;
      }
      .pdf-footer .footer-brand {
        font-weight: 700;
        color: #0d6efd;
      }

      @media print {
        body { padding: 0; }
        .pdf-wrapper { padding: 24px; }
        @page { margin: 10mm; size: A4; }
      }
    `;
  }

  private formatTimestamp(): string {
    return new Date().toLocaleDateString('en-US', {
      year: 'numeric', month: 'long', day: 'numeric',
      hour: '2-digit', minute: '2-digit'
    });
  }

  generateInvoicePdf(inv: Invoice): void {
    const statusLabel = this.statusLabels[inv.status] ?? 'Draft';
    const statusColor = this.statusColors[inv.status] ?? '#6c757d';
    const generatedOn = this.formatTimestamp();

    const lineItemsRows = inv.lineItems?.length
      ? inv.lineItems.map(li => `
          <tr>
            <td>${this.escHtml(li.description)}</td>
            <td class="text-right">${li.quantity}</td>
            <td class="text-right">$${li.unitPrice.toFixed(2)}</td>
            <td class="text-right">$${li.total.toFixed(2)}</td>
          </tr>`).join('')
      : `<tr><td colspan="4" style="color:#adb5bd;text-align:center;padding:20px">No line items</td></tr>`;

    const html = `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Invoice ${this.escHtml(inv.invoiceNumber)}</title>
  <style>${this.getSharedStyles()}</style>
</head>
<body>
  <div class="pdf-wrapper">

    <!-- HEADER -->
    <header class="pdf-header">
      <div class="pdf-header-left">
        <div class="company-name">&#9881; ${this.companyName}</div>
        <div class="company-tagline">${this.companyTagline}</div>
      </div>
      <div class="pdf-header-right">
        <div class="doc-type">Invoice</div>
        <div class="doc-number"># ${this.escHtml(inv.invoiceNumber)}</div>
        <div class="doc-date">Generated: ${generatedOn}</div>
      </div>
    </header>

    <!-- CLIENT & INVOICE INFO -->
    <section class="pdf-section">
      <div class="info-grid">
        <div>
          <div class="pdf-section-title">Bill To</div>
          <div class="info-item">
            <span style="font-size:16px;font-weight:600">${this.escHtml(inv.clientName)}</span>
          </div>
          ${inv.clientEmail ? `<div class="info-item" style="margin-top:6px"><span style="color:#6c757d">${this.escHtml(inv.clientEmail)}</span></div>` : ''}
          ${inv.clientAddress ? `<div class="info-item" style="margin-top:4px"><span style="color:#6c757d">${this.escHtml(inv.clientAddress)}</span></div>` : ''}
        </div>
        <div>
          <div class="pdf-section-title">Invoice Details</div>
          <div class="info-grid" style="grid-template-columns:1fr 1fr">
            <div class="info-item">
              <label>Issue Date</label>
              <span>${this.formatDate(inv.issueDate)}</span>
            </div>
            <div class="info-item">
              <label>Due Date</label>
              <span>${this.formatDate(inv.dueDate)}</span>
            </div>
            ${inv.paidDate ? `
            <div class="info-item">
              <label>Paid Date</label>
              <span>${this.formatDate(inv.paidDate)}</span>
            </div>` : ''}
            <div class="info-item">
              <label>Status</label>
              <span><span class="status-badge" style="background:${statusColor}">${statusLabel}</span></span>
            </div>
          </div>
        </div>
      </div>
    </section>

    <!-- LINE ITEMS -->
    <section class="pdf-section">
      <div class="pdf-section-title">Services &amp; Items</div>
      <table class="pdf-table">
        <thead>
          <tr>
            <th>Description</th>
            <th class="text-right">Qty</th>
            <th class="text-right">Unit Price</th>
            <th class="text-right">Total</th>
          </tr>
        </thead>
        <tbody>
          ${lineItemsRows}
        </tbody>
        <tfoot>
          <tr>
            <td colspan="3" class="text-right" style="color:#6c757d">Subtotal</td>
            <td class="text-right">$${inv.subTotal.toFixed(2)}</td>
          </tr>
          <tr>
            <td colspan="3" class="text-right" style="color:#6c757d">Tax (${inv.taxRate}%)</td>
            <td class="text-right">$${inv.taxAmount.toFixed(2)}</td>
          </tr>
          <tr class="total-row">
            <td colspan="3" class="text-right">Total Amount</td>
            <td class="text-right">$${inv.totalAmount.toFixed(2)}</td>
          </tr>
        </tfoot>
      </table>
    </section>

    ${inv.notes ? `
    <!-- NOTES -->
    <section class="pdf-section">
      <div class="pdf-section-title">Notes</div>
      <div class="pdf-box">${this.escHtml(inv.notes)}</div>
    </section>` : ''}

    ${inv.taskTitle ? `
    <!-- LINKED TASK -->
    <section class="pdf-section">
      <div class="pdf-section-title">Related Task Order</div>
      <div class="info-item">
        <span>&#128279; ${this.escHtml(inv.taskTitle)}</span>
      </div>
    </section>` : ''}

    <!-- FOOTER -->
    <footer class="pdf-footer">
      <div class="footer-left">
        <span class="footer-brand">${this.companyName}</span> &mdash; ${this.companyTagline}<br>
        This document was automatically generated and is valid without a signature.
      </div>
      <div class="footer-right">
        Generated on ${generatedOn}<br>
        Invoice #${this.escHtml(inv.invoiceNumber)}
      </div>
    </footer>

  </div>
</body>
</html>`;

    this.printHtml(html, `Invoice-${inv.invoiceNumber}`);
  }

  generateReportPdf(report: MaintenanceReport): void {
    const generatedOn = this.formatTimestamp();

    const html = `<!DOCTYPE html>
<html lang="en">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1.0">
  <title>Maintenance Report – ${this.escHtml(report.title)}</title>
  <style>${this.getSharedStyles()}</style>
</head>
<body>
  <div class="pdf-wrapper">

    <!-- HEADER -->
    <header class="pdf-header">
      <div class="pdf-header-left">
        <div class="company-name">&#9881; ${this.companyName}</div>
        <div class="company-tagline">${this.companyTagline}</div>
      </div>
      <div class="pdf-header-right">
        <div class="doc-type">Maintenance Report</div>
        <div class="doc-number">${this.escHtml(report.title)}</div>
        <div class="doc-date">Generated: ${generatedOn}</div>
      </div>
    </header>

    <!-- REPORT DETAILS -->
    <section class="pdf-section">
      <div class="pdf-section-title">Report Information</div>
      <div class="info-grid">
        <div class="info-item">
          <label>Technician</label>
          <span>${this.escHtml(report.technicianName)}</span>
        </div>
        <div class="info-item">
          <label>Report Date</label>
          <span>${this.formatDate(report.reportDate)}</span>
        </div>
        ${report.taskTitle ? `
        <div class="info-item">
          <label>Related Task Order</label>
          <span>${this.escHtml(report.taskTitle)}</span>
        </div>` : ''}
        <div class="info-item">
          <label>Created At</label>
          <span>${this.formatDate(report.createdAt)}</span>
        </div>
      </div>
    </section>

    ${(report.laborHours || report.materialCost) ? `
    <!-- METRICS -->
    <section class="pdf-section">
      <div class="pdf-section-title">Work Summary</div>
      <div class="metrics-row">
        ${report.laborHours ? `
        <div class="metric-card">
          <div class="metric-value">${report.laborHours}</div>
          <div class="metric-label">Labor Hours</div>
        </div>` : ''}
        ${report.materialCost ? `
        <div class="metric-card">
          <div class="metric-value">$${report.materialCost.toFixed(2)}</div>
          <div class="metric-label">Material Cost</div>
        </div>` : ''}
      </div>
    </section>` : ''}

    <!-- CONTENT -->
    <section class="pdf-section">
      <div class="pdf-section-title">Report Content</div>
      <div class="pdf-box">${this.escHtml(report.content)}</div>
    </section>

    ${report.recommendations ? `
    <!-- RECOMMENDATIONS -->
    <section class="pdf-section">
      <div class="pdf-section-title">Recommendations</div>
      <div class="pdf-box recommendation">${this.escHtml(report.recommendations)}</div>
    </section>` : ''}

    <!-- FOOTER -->
    <footer class="pdf-footer">
      <div class="footer-left">
        <span class="footer-brand">${this.companyName}</span> &mdash; ${this.companyTagline}<br>
        This report was automatically generated. Technician: ${this.escHtml(report.technicianName)}
      </div>
      <div class="footer-right">
        Generated on ${generatedOn}<br>
        Report Date: ${this.formatDate(report.reportDate)}
      </div>
    </footer>

  </div>
</body>
</html>`;

    this.printHtml(html, `Report-${report.title}`);
  }

  private escHtml(text: string | null | undefined): string {
    if (text == null) return '';
    return text
      .replace(/&/g, '&amp;')
      .replace(/</g, '&lt;')
      .replace(/>/g, '&gt;')
      .replace(/"/g, '&quot;')
      .replace(/'/g, '&#39;');
  }

  private printHtml(html: string, filename: string): void {
    const win = window.open('', '_blank', 'width=900,height=700');
    if (!win) {
      alert('Popup blocked. Please allow popups for this site to generate PDFs.');
      return;
    }
    win.document.open();
    win.document.write(html);
    win.document.close();
    win.document.title = filename;
    win.onload = () => {
      win.focus();
      win.print();
    };
  }
}
