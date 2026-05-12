import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinancialReportService } from '../../../../services/financial-report.service';
import { ToastService } from '../../../../services/toast.service';
import { AgingReport as AgingReportData } from '../../../../models';

@Component({
  selector: 'app-aging-report',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './aging-report.html',
  styleUrls: ['./aging-report.css']
})
export class AgingReportComponent implements OnInit {
  report = signal<AgingReportData | null>(null);
  filter = { asOfDate: new Date().toISOString().slice(0, 10) };

  constructor(private service: FinancialReportService, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.service.getAgingReport(this.filter.asOfDate || undefined).subscribe({
      next: report => this.report.set(report),
      error: () => this.toast.error()
    });
  }
}

export { AgingReportComponent as AgingReport };
