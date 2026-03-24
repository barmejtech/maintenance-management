import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { ReportService } from '../../services/report.service';
import { TaskOrderService } from '../../services/task-order.service';
import { MaintenanceReport, CreateReportRequest, TaskOrder } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { PdfService } from '../../services/pdf.service';

@Component({
  selector: 'app-reports',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe],
  templateUrl: './reports.component.html',
  styleUrls: ['./reports.component.css']
})
export class ReportsComponent implements OnInit {
  reports = signal<MaintenanceReport[]>([]);
  taskOrders = signal<TaskOrder[]>([]);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);

  form: CreateReportRequest = {
    title: '',
    content: '',
    technicianName: '',
    reportDate: '',
    laborHours: undefined,
    materialCost: undefined,
    recommendations: '',
    taskOrderId: ''
  };
  private editingId = '';

  constructor(
    private service: ReportService,
    private taskService: TaskOrderService,
    private translation: TranslationService,
    private pdf: PdfService
  ) {}

  ngOnInit() {
    this.load();
    this.taskService.getAll().subscribe({ next: d => this.taskOrders.set(d), error: () => {} });
  }

  load() {
    this.service.getAll().subscribe({ next: d => this.reports.set(d), error: () => {} });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { title: '', content: '', technicianName: '', reportDate: new Date().toISOString().substring(0, 10), laborHours: undefined, materialCost: undefined, recommendations: '', taskOrderId: '' };
    this.showModal.set(true);
  }

  openEdit(r: MaintenanceReport) {
    this.isEditing.set(true);
    this.editingId = r.id;
    this.form = {
      title: r.title,
      content: r.content,
      technicianName: r.technicianName,
      reportDate: r.reportDate ? r.reportDate.substring(0, 10) : '',
      laborHours: r.laborHours,
      materialCost: r.materialCost,
      recommendations: r.recommendations ?? '',
      taskOrderId: r.taskOrderId ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    const dto: CreateReportRequest = {
      ...this.form,
      taskOrderId: this.form.taskOrderId || undefined
    };
    const obs = this.isEditing()
      ? this.service.update(this.editingId, dto)
      : this.service.create(dto);
    obs.subscribe({
      next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
      error: () => this.isSaving.set(false)
    });
  }

  delete(id: string) {
    if (!confirm(this.translation.translate('reports.deleteConfirm'))) return;
    this.service.delete(id).subscribe({ next: () => this.load(), error: () => {} });
  }

  generatePdf(report: MaintenanceReport): void {
    this.pdf.generateReportPdf(report);
  }
}
