import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PerformanceService } from '../../services/performance.service';
import { TechnicianService } from '../../services/technician.service';
import { TechnicianPerformanceScore, Technician } from '../../models';

@Component({
  selector: 'app-performance',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './performance.component.html',
  styleUrls: ['./performance.component.css']
})
export class PerformanceComponent implements OnInit {
  scores = signal<TechnicianPerformanceScore[]>([]);
  technicians = signal<Technician[]>([]);
  activeTab = signal<'all' | 'top'>('all');
  isRecalculating = signal<string | null>(null);
  topCount = 10;

  constructor(
    private performanceService: PerformanceService,
    private technicianService: TechnicianService
  ) {}

  ngOnInit() {
    this.loadAll();
    this.technicianService.getAll().subscribe({ next: t => this.technicians.set(t), error: () => {} });
  }

  loadAll() {
    this.performanceService.getAll().subscribe({ next: s => this.scores.set(s), error: () => {} });
  }

  loadTop() {
    this.performanceService.getTopPerformers(this.topCount).subscribe({ next: s => this.scores.set(s), error: () => {} });
  }

  switchTab(tab: 'all' | 'top') {
    this.activeTab.set(tab);
    if (tab === 'top') this.loadTop();
    else this.loadAll();
  }

  recalculate(technicianId: string) {
    this.isRecalculating.set(technicianId);
    this.performanceService.recalculate(technicianId).subscribe({
      next: (result) => {
        this.isRecalculating.set(null);
        const current = this.scores();
        const idx = current.findIndex(s => s.technicianId === result.technicianId);
        if (idx >= 0) {
          const updated = [...current];
          updated[idx] = result;
          this.scores.set(updated);
        } else {
          this.scores.set([result, ...current]);
        }
      },
      error: () => this.isRecalculating.set(null)
    });
  }

  recalculateAll() {
    const techs = this.technicians();
    if (!techs.length) return;
    let pending = techs.length;
    techs.forEach(t => {
      this.performanceService.recalculate(t.id).subscribe({
        next: () => { pending--; if (pending === 0) this.loadAll(); },
        error: () => { pending--; if (pending === 0) this.loadAll(); }
      });
    });
  }

  getScoreClass(value: number): string {
    if (value >= 80) return 'text-success fw-bold';
    if (value >= 60) return 'text-warning fw-bold';
    return 'text-danger fw-bold';
  }

  getSatisfactionStars(score: number): string {
    const full = Math.floor(score);
    return '★'.repeat(full) + '☆'.repeat(5 - full);
  }
}
