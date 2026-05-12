import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { FinancialReportService } from '../../../../services/financial-report.service';
import { ToastService } from '../../../../services/toast.service';
import { ProfitLoss as ProfitLossReport } from '../../../../models';

@Component({
  selector: 'app-profit-loss',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './profit-loss.html',
  styleUrls: ['./profit-loss.css']
})
export class ProfitLossComponent implements OnInit {
  report = signal<ProfitLossReport | null>(null);
  isLoading = signal(false);
  filter = {
    startDate: new Date(new Date().getFullYear(), new Date().getMonth(), 1).toISOString().slice(0, 10),
    endDate: new Date().toISOString().slice(0, 10)
  };

  constructor(private service: FinancialReportService, private toast: ToastService) {}

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.service.getProfitLoss(this.filter.startDate, this.filter.endDate).subscribe({
      next: report => { this.report.set(report); this.isLoading.set(false); },
      error: () => { this.toast.error(); this.isLoading.set(false); }
    });
  }
}

export { ProfitLossComponent as ProfitLoss };
