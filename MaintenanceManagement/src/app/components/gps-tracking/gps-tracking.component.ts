import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import { GpsTrackingService } from '../../services/gps-tracking.service';
import { TechnicianService } from '../../services/technician.service';
import { ToastService } from '../../services/toast.service';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { Technician, TechnicianGpsLog, TechnicianDistance } from '../../models';

@Component({
  selector: 'app-gps-tracking',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe, NgxPaginationModule],
  templateUrl: './gps-tracking.component.html',
  styleUrls: ['./gps-tracking.component.css']
})
export class GpsTrackingComponent implements OnInit {
  technicians = signal<Technician[]>([]);
  selectedTechnicianId = '';
  latestLocation = signal<TechnicianGpsLog | null>(null);
  history = signal<TechnicianGpsLog[]>([]);
  distanceResult = signal<TechnicianDistance | null>(null);

  isLoadingLatest = signal(false);
  isLoadingHistory = signal(false);
  isCalculatingDistance = signal(false);

  serviceLatitude: number | null = null;
  serviceLongitude: number | null = null;

  historyPage = 1;
  historyPageSize = 10;

  constructor(
    private gpsService: GpsTrackingService,
    private technicianService: TechnicianService,
    private toast: ToastService,
    public translation: TranslationService
  ) {}

  ngOnInit() {
    this.technicianService.getAll().subscribe({
      next: techs => this.technicians.set(techs),
      error: () => this.toast.show(this.translation.translate('gpsTracking.failedLoadTechnicians'), 'error')
    });
  }

  onTechnicianChange() {
    const id = this.selectedTechnicianId;
    if (!id) {
      this.latestLocation.set(null);
      this.history.set([]);
      this.distanceResult.set(null);
      return;
    }
    this.loadLatestLocation(id);
    this.loadHistory(id);
    this.distanceResult.set(null);
  }

  loadLatestLocation(id: string) {
    this.isLoadingLatest.set(true);
    this.gpsService.getLatest(id).subscribe({
      next: loc => { this.latestLocation.set(loc); this.isLoadingLatest.set(false); },
      error: () => { this.latestLocation.set(null); this.isLoadingLatest.set(false); }
    });
  }

  loadHistory(id: string) {
    this.isLoadingHistory.set(true);
    this.gpsService.getHistory(id).subscribe({
      next: logs => { this.history.set(logs); this.isLoadingHistory.set(false); },
      error: () => { this.history.set([]); this.isLoadingHistory.set(false); }
    });
  }

  calculateDistance() {
    const id = this.selectedTechnicianId;
    if (!id || this.serviceLatitude == null || this.serviceLongitude == null) return;
    this.isCalculatingDistance.set(true);
    this.distanceResult.set(null);
    this.gpsService.getDistance(id, this.serviceLatitude, this.serviceLongitude).subscribe({
      next: result => { this.distanceResult.set(result); this.isCalculatingDistance.set(false); },
      error: () => {
        this.toast.show(this.translation.translate('gpsTracking.failedCalculateDistance'), 'error');
        this.isCalculatingDistance.set(false);
      }
    });
  }

  isStale(recordedAt: string): boolean {
    const recorded = new Date(recordedAt).getTime();
    const now = Date.now();
    return (now - recorded) > 4 * 60 * 60 * 1000;
  }
}
