import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinancialReportService } from '../../../../services/financial-report.service';
import { ToastService } from '../../../../services/toast.service';
import { AccountBalance, BalanceSheet as BalanceSheetReport } from '../../../../models';

@Component({
  selector: 'app-balance-sheet',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './balance-sheet.html',
  styleUrls: ['./balance-sheet.css']
})
export class BalanceSheetComponent implements OnInit {
  report = signal<BalanceSheetReport | null>(null);
  filter = { asOfDate: new Date().toISOString().slice(0, 10) };
  readonly sections = [
    { key: 'assets' as const, label: 'Assets' },
    { key: 'liabilities' as const, label: 'Liabilities' },
    { key: 'equity' as const, label: 'Equity' }
  ];

  constructor(private service: FinancialReportService, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.service.getBalanceSheet(this.filter.asOfDate || undefined).subscribe({
      next: report => this.report.set(report),
      error: () => this.toast.error()
    });
  }

  list(section: 'assets' | 'liabilities' | 'equity'): AccountBalance[] {
    return this.report()?.[section] || [];
  }
}

export { BalanceSheetComponent as BalanceSheet };
