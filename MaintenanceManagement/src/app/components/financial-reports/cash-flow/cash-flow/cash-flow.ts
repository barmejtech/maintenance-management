import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinancialReportService } from '../../../../services/financial-report.service';
import { ToastService } from '../../../../services/toast.service';
import { CashFlow as CashFlowReport, CashFlowItem } from '../../../../models';

@Component({
  selector: 'app-cash-flow',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './cash-flow.html',
  styleUrls: ['./cash-flow.css']
})
export class CashFlowComponent implements OnInit {
  report = signal<CashFlowReport | null>(null);
  readonly sections = [
    { key: 'operatingItems' as const, label: 'Operating activities' },
    { key: 'investingItems' as const, label: 'Investing activities' },
    { key: 'financingItems' as const, label: 'Financing activities' }
  ];
  filter = {
    startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().slice(0, 10),
    endDate: new Date().toISOString().slice(0, 10)
  };

  constructor(private service: FinancialReportService, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.service.getCashFlow(this.filter.startDate, this.filter.endDate).subscribe({
      next: report => this.report.set(report),
      error: () => this.toast.error()
    });
  }

  list(section: 'operatingItems' | 'investingItems' | 'financingItems'): CashFlowItem[] {
    return this.report()?.[section] || [];
  }
}

