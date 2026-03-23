import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PredictiveMaintenanceService } from '../../services/predictive-maintenance.service';
import { EquipmentService } from '../../services/equipment.service';
import { EquipmentHealthPrediction, Equipment } from '../../models';

@Component({
  selector: 'app-predictive-maintenance',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './predictive-maintenance.component.html',
  styleUrls: ['./predictive-maintenance.component.css']
})
export class PredictiveMaintenanceComponent implements OnInit {
  predictions = signal<EquipmentHealthPrediction[]>([]);
  equipment = signal<Equipment[]>([]);
  isRunning = signal<string | null>(null);
  activeTab = signal<'all' | 'highRisk'>('all');

  constructor(
    private service: PredictiveMaintenanceService,
    private equipmentService: EquipmentService
  ) {}

  ngOnInit() {
    this.load();
    this.equipmentService.getAll().subscribe({ next: e => this.equipment.set(e), error: () => {} });
  }

  load() {
    this.service.getAll().subscribe({ next: d => this.predictions.set(d), error: () => {} });
  }

  loadHighRisk() {
    this.service.getHighRisk(0.5).subscribe({ next: d => this.predictions.set(d), error: () => {} });
  }

  switchTab(tab: 'all' | 'highRisk') {
    this.activeTab.set(tab);
    if (tab === 'highRisk') this.loadHighRisk();
    else this.load();
  }

  runPrediction(equipmentId: string) {
    this.isRunning.set(equipmentId);
    this.service.runPrediction(equipmentId).subscribe({
      next: (result) => {
        this.isRunning.set(null);
        const current = this.predictions();
        const idx = current.findIndex(p => p.equipmentId === result.equipmentId);
        if (idx >= 0) {
          const updated = [...current];
          updated[idx] = result;
          this.predictions.set(updated);
        } else {
          this.predictions.set([result, ...current]);
        }
      },
      error: () => this.isRunning.set(null)
    });
  }

  runAllPredictions() {
    const eqs = this.equipment();
    if (!eqs.length) return;
    let pending = eqs.length;
    eqs.forEach(eq => {
      this.service.runPrediction(eq.id).subscribe({
        next: () => { pending--; if (pending === 0) this.load(); },
        error: () => { pending--; if (pending === 0) this.load(); }
      });
    });
  }

  getPredictionForEquipment(equipmentId: string): EquipmentHealthPrediction | undefined {
    return this.predictions().find(p => p.equipmentId === equipmentId);
  }

  getRiskClass(prob: number): string {
    if (prob >= 0.75) return 'bg-danger';
    if (prob >= 0.50) return 'bg-warning text-dark';
    if (prob >= 0.30) return 'bg-info text-dark';
    return 'bg-success';
  }

  getRiskLabel(prob: number): string {
    if (prob >= 0.75) return 'Critical';
    if (prob >= 0.50) return 'High';
    if (prob >= 0.30) return 'Moderate';
    return 'Low';
  }

  formatPercent(prob: number): string {
    return (prob * 100).toFixed(1) + '%';
  }
}
