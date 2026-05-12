import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinancialReportService } from '../../../../services/financial-report.service';
import { ToastService } from '../../../../services/toast.service';
import { TrialBalance as TrialBalanceReport } from '../../../../models';

@Component({
  selector: 'app-trial-balance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './trial-balance.html',
  styleUrls: ['./trial-balance.css']
})
export class TrialBalanceComponent implements OnInit {
  report = signal<TrialBalanceReport | null>(null);
  isLoading = signal(false);
  filter = { asOfDate: new Date().toISOString().slice(0, 10) };

  constructor(private service: FinancialReportService, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.service.getTrialBalance(this.filter.asOfDate || undefined).subscribe({
      next: report => { this.report.set(report); this.isLoading.set(false); },
      error: () => { this.toast.error(); this.isLoading.set(false); }
    });
  }
}

