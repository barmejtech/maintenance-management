import { Component, OnInit, OnDestroy, signal, NgZone } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { NgxPaginationModule } from 'ngx-pagination';
import * as L from 'leaflet';
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
export class GpsTrackingComponent implements OnInit, OnDestroy {
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

  private latestMap: L.Map | null = null;
  private distanceMap: L.Map | null = null;

  constructor(
    private gpsService: GpsTrackingService,
    private technicianService: TechnicianService,
    private toast: ToastService,
    public translation: TranslationService,
    private ngZone: NgZone
  ) {}

  ngOnInit() {
    this.technicianService.getAll().subscribe({
      next: techs => this.technicians.set(techs),
      error: () => this.toast.show(this.translation.translate('gpsTracking.failedLoadTechnicians'), 'error')
    });
  }

  ngOnDestroy() {
    this.destroyMaps();
  }

  onTechnicianChange() {
    const id = this.selectedTechnicianId;
    if (!id) {
      this.latestLocation.set(null);
      this.history.set([]);
      this.distanceResult.set(null);
      this.destroyMaps();
      return;
    }
    this.loadLatestLocation(id);
    this.loadHistory(id);
    this.distanceResult.set(null);
    this.destroyMaps();
  }

  loadLatestLocation(id: string) {
    this.isLoadingLatest.set(true);
    this.gpsService.getLatest(id).subscribe({
      next: loc => {
        this.latestLocation.set(loc);
        this.isLoadingLatest.set(false);
        setTimeout(() => this.initLatestMap(), 100);
      },
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
      next: result => {
        this.distanceResult.set(result);
        this.isCalculatingDistance.set(false);
        setTimeout(() => this.initDistanceMap(), 100);
      },
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

  private initLatestMap() {
    const loc = this.latestLocation();
    if (!loc) return;

    const container = document.getElementById('latest-map');
    if (!container) return;

    if (this.latestMap) {
      this.latestMap.remove();
      this.latestMap = null;
    }

    this.ngZone.runOutsideAngular(() => {
      const map = L.map('latest-map', { zoomControl: true }).setView([loc.latitude, loc.longitude], 13);

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        maxZoom: 19
      }).addTo(map);

      const techIcon = L.divIcon({
        className: '',
        html: '<div class="map-marker-tech"><i class="bi bi-person-fill"></i></div>',
        iconSize: [36, 36],
        iconAnchor: [18, 36],
        popupAnchor: [0, -36]
      });

      const techName = this.getTechnicianName();
      L.marker([loc.latitude, loc.longitude], { icon: techIcon })
        .addTo(map)
        .bindPopup(`<strong>📍 ${techName}</strong><br/><small>${loc.latitude.toFixed(6)}, ${loc.longitude.toFixed(6)}</small>`)
        .openPopup();

      this.latestMap = map;
    });
  }

  private initDistanceMap() {
    const result = this.distanceResult();
    if (!result) return;

    const container = document.getElementById('distance-map');
    if (!container) return;

    if (this.distanceMap) {
      this.distanceMap.remove();
      this.distanceMap = null;
    }

    this.ngZone.runOutsideAngular(() => {
      const map = L.map('distance-map', { zoomControl: true });

      L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '© <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors',
        maxZoom: 19
      }).addTo(map);

      const techIcon = L.divIcon({
        className: '',
        html: '<div class="map-marker-tech"><i class="bi bi-person-fill"></i></div>',
        iconSize: [36, 36],
        iconAnchor: [18, 36],
        popupAnchor: [0, -36]
      });

      const serviceIcon = L.divIcon({
        className: '',
        html: '<div class="map-marker-service"><i class="bi bi-tools"></i></div>',
        iconSize: [36, 36],
        iconAnchor: [18, 36],
        popupAnchor: [0, -36]
      });

      const techLabel = this.translation.translate('gpsTracking.technicianMarker');
      const serviceLabel = this.translation.translate('gpsTracking.serviceMarker');

      const techMarker = L.marker([result.technicianLatitude, result.technicianLongitude], { icon: techIcon })
        .addTo(map)
        .bindPopup(`<strong>🔵 ${techLabel}</strong><br/><strong>${result.technicianName}</strong>${result.technicianAddress ? '<br/><small>' + result.technicianAddress + '</small>' : ''}<br/><small>${result.technicianLatitude.toFixed(6)}, ${result.technicianLongitude.toFixed(6)}</small>`);

      L.marker([result.serviceLatitude, result.serviceLongitude], { icon: serviceIcon })
        .addTo(map)
        .bindPopup(`<strong>🔴 ${serviceLabel}</strong><br/><small>${result.serviceLatitude.toFixed(6)}, ${result.serviceLongitude.toFixed(6)}</small>`);

      L.polyline(
        [[result.technicianLatitude, result.technicianLongitude], [result.serviceLatitude, result.serviceLongitude]],
        { color: '#0d6efd', weight: 3, opacity: 0.8, dashArray: '8, 6' }
      ).addTo(map);

      const bounds = L.latLngBounds(
        [result.technicianLatitude, result.technicianLongitude],
        [result.serviceLatitude, result.serviceLongitude]
      );
      map.fitBounds(bounds, { padding: [50, 50] });

      techMarker.openPopup();
      this.distanceMap = map;
    });
  }

  private getTechnicianName(): string {
    const id = this.selectedTechnicianId;
    const tech = this.technicians().find(t => t.id === id);
    return tech ? tech.fullName : this.translation.translate('gpsTracking.technicianMarker');
  }

  private destroyMaps() {
    if (this.latestMap) {
      this.latestMap.remove();
      this.latestMap = null;
    }
    if (this.distanceMap) {
      this.distanceMap.remove();
      this.distanceMap = null;
    }
  }
}
