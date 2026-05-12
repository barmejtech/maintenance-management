import { Component, ElementRef, OnDestroy, OnInit, ViewChild, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Chart } from 'chart.js/auto';
import { MeterReadingService } from '../../../services/meter-reading.service';
import { UnitService } from '../../../services/unit.service';
import { ToastService } from '../../../services/toast.service';
import { TranslationService } from '../../../services/translate.service';
import { BulkMeterReadingRequest, BulkMeterReadingResult, CreateMeterReadingRequest, MeterReading, MeterReadingChartData, MeterType, UnitDto, UpdateMeterReadingRequest } from '../../../models';

@Component({
  selector: 'app-meter-readings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './meter-readings.html',
  styleUrls: ['./meter-readings.css']
})
export class MeterReadingsComponent implements OnInit, OnDestroy {
  @ViewChild('trendCanvas') trendCanvas?: ElementRef<HTMLCanvasElement>;
  readings = signal<MeterReading[]>([]);
  units = signal<UnitDto[]>([]);
  chartData = signal<MeterReadingChartData[]>([]);
  bulkResult = signal<BulkMeterReadingResult | null>(null);
  isLoading = signal(false);
  isSaving = signal(false);
  showModal = signal(false);
  showBulkModal = signal(false);
  isEditing = signal(false);
  readonly meterTypes = Object.values(MeterType).filter(value => typeof value === 'number') as MeterType[];
  readonly today = new Date().toISOString().slice(0, 10);
  chartFilter = { unitId: '', type: MeterType.Water, months: 12 };

  form: CreateMeterReadingRequest = this.emptyForm();
  bulkForm: BulkMeterReadingRequest = { type: MeterType.Water, readingDate: this.today, unitPrice: undefined, readings: [] };
  private editingId = '';
  private trendChart?: Chart;

  constructor(
    private service: MeterReadingService,
    private unitService: UnitService,
    private toast: ToastService,
    private translation: TranslationService
  ) {}

  ngOnInit(): void {
    this.load();
    this.unitService.getAll().subscribe({
      next: units => {
        this.units.set(units);
        this.chartFilter.unitId = units[0]?.id || '';
      }
    });
  }

  load(): void {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: readings => {
        this.readings.set(readings);
        this.isLoading.set(false);
      },
      error: () => {
        this.toast.error();
        this.isLoading.set(false);
      }
    });
  }

  openAdd(): void {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = this.emptyForm();
    this.showModal.set(true);
  }

  openEdit(reading: MeterReading): void {
    this.isEditing.set(true);
    this.editingId = reading.id;
    this.form = {
      unitId: reading.unitId,
      equipmentId: reading.equipmentId,
      type: reading.type,
      readingValue: reading.readingValue,
      readingDate: reading.readingDate.slice(0, 10),
      photoUrl: reading.photoUrl || '',
      notes: reading.notes || '',
      unitPrice: reading.unitPrice
    };
    this.showModal.set(true);
  }

  closeModal(): void { this.showModal.set(false); }

  save(): void {
    const payload = {
      unitId: this.form.unitId,
      equipmentId: this.form.equipmentId || undefined,
      type: this.form.type,
      readingValue: Number(this.form.readingValue),
      readingDate: this.form.readingDate,
      photoUrl: this.form.photoUrl?.trim() || undefined,
      notes: this.form.notes?.trim() || undefined,
      unitPrice: this.form.unitPrice || undefined
    };
    this.isSaving.set(true);
    const request = this.isEditing()
      ? this.service.update(this.editingId, { readingValue: payload.readingValue, photoUrl: payload.photoUrl, notes: payload.notes } satisfies UpdateMeterReadingRequest)
      : this.service.create(payload satisfies CreateMeterReadingRequest);
    request.subscribe({
      next: () => {
        this.isSaving.set(false);
        this.showModal.set(false);
        this.load();
        this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created');
      },
      error: () => {
        this.isSaving.set(false);
        this.toast.error();
      }
    });
  }

  delete(reading: MeterReading): void {
    if (!confirm(this.translation.translate('messages.confirmDelete'))) {
      return;
    }
    this.service.delete(reading.id).subscribe({
      next: () => {
        this.load();
        this.toast.success('messages.deleted');
      },
      error: () => this.toast.error()
    });
  }

  openBulk(): void {
    this.bulkForm = {
      type: MeterType.Water,
      readingDate: this.today,
      unitPrice: undefined,
      readings: this.units().map(unit => ({ unitId: unit.id, readingValue: 0 }))
    };
    this.bulkResult.set(null);
    this.showBulkModal.set(true);
  }

  closeBulk(): void { this.showBulkModal.set(false); }

  submitBulk(): void {
    this.isSaving.set(true);
    const payload: BulkMeterReadingRequest = {
      type: this.bulkForm.type,
      readingDate: this.bulkForm.readingDate,
      unitPrice: this.bulkForm.unitPrice || undefined,
      readings: this.bulkForm.readings.filter(item => item.readingValue > 0)
    };
    this.service.bulkCreate(payload).subscribe({
      next: result => {
        this.isSaving.set(false);
        this.bulkResult.set(result);
        this.load();
        this.toast.success('messages.created');
      },
      error: () => {
        this.isSaving.set(false);
        this.toast.error();
      }
    });
  }

  loadChart(): void {
    if (!this.chartFilter.unitId) {
      return;
    }
    this.service.getChartData(this.chartFilter.unitId, this.chartFilter.type, this.chartFilter.months).subscribe({
      next: data => {
        this.chartData.set(data);
        this.renderTrendChart();
      },
      error: () => this.toast.error()
    });
  }

  getTypeLabel(type: MeterType): string {
    return MeterType[type] ?? 'Unknown';
  }

  unitLabel(unitId: string): string {
    return this.units().find(unit => unit.id === unitId)?.unitNumber || unitId;
  }

  private emptyForm(): CreateMeterReadingRequest {
    return {
      unitId: '',
      equipmentId: undefined,
      type: MeterType.Water,
      readingValue: 0,
      readingDate: this.today,
      photoUrl: '',
      notes: '',
      unitPrice: undefined
    };
  }

  ngOnDestroy(): void {
    this.trendChart?.destroy();
  }

  private renderTrendChart(): void {
    if (!this.trendCanvas) {
      console.warn('Meter trend chart canvas is not ready yet.');
      return;
    }
    this.trendChart?.destroy();
    const points = this.chartData();
    this.trendChart = new Chart(this.trendCanvas.nativeElement, {
      type: 'line',
      data: {
        labels: points.map(point => point.label),
        datasets: [
          {
            label: 'Consumption',
            data: points.map(point => point.consumption),
            borderColor: '#0d6efd',
            backgroundColor: 'rgba(13, 110, 253, 0.2)',
            fill: true,
            tension: 0.35
          }
        ]
      },
      options: {
        responsive: true,
        plugins: {
          legend: { display: points.length > 0 }
        }
      }
    });
  }
}
