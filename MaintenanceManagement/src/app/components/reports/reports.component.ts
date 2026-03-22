import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { ReportService } from '../../services/report.service';
import { MaintenanceReport } from '../../models';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent implements OnInit {
  reports = signal<MaintenanceReport[]>([]);

  constructor(private service: ReportService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.reports.set(d), error: () => {} });
  }
}
