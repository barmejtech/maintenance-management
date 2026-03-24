import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PerformanceService } from '../../services/performance.service';
import { TechnicianService } from '../../services/technician.service';
import { TechnicianPerformanceScore, Technician } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';

@Component({
  selector: 'app-performance',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './performance.component.html',
  styleUrls: ['./performance.component.css']
})
export class PerformanceComponent implements OnInit {
  scores = signal<TechnicianPerformanceScore[]>([]);
  technicians = signal<Technician[]>([]);
  activeTab = signal<'all' | 'top'>('all');
  isRecalculating = signal<string | null>(null);
  isRecalculatingAll = signal(false);
  topCount = 10;

  // KPI aggregates
  avgRepairTime = computed(() => {
    const s = this.scores().filter(x => x.averageInterventionTimeMinutes != null);
    if (!s.length) return 0;
    return s.reduce((acc, x) => acc + (x.averageInterventionTimeMinutes ?? 0), 0) / s.length;
  });

  firstTimeFixRate = computed(() => {
    const s = this.scores();
    if (!s.length) return 0;
    return s.reduce((acc, x) => acc + x.successRate, 0) / s.length;
  });

  totalCompletedTasks = computed(() =>
    this.scores().reduce((acc, x) => acc + x.totalTasksCompleted, 0)
  );

  avgCustomerRating = computed(() => {
    const s = this.scores();
    if (!s.length) return 0;
    return s.reduce((acc, x) => acc + x.customerSatisfactionScore, 0) / s.length;
  });

  avgOnTimeRate = computed(() => {
    const s = this.scores();
    if (!s.length) return 0;
    return s.reduce((acc, x) => acc + x.onTimeRate, 0) / s.length;
  });

  topPerformer = computed(() => {
    const s = this.scores();
    if (!s.length) return null;
    return s.reduce((best, x) => x.successRate > best.successRate ? x : best, s[0]);
  });

  constructor(
    private performanceService: PerformanceService,
    private technicianService: TechnicianService,
    public translation: TranslationService
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
    this.isRecalculatingAll.set(true);
    let pending = techs.length;
    techs.forEach(t => {
      this.performanceService.recalculate(t.id).subscribe({
        next: () => { pending--; if (pending === 0) { this.isRecalculatingAll.set(false); this.loadAll(); } },
        error: () => { pending--; if (pending === 0) { this.isRecalculatingAll.set(false); this.loadAll(); } }
      });
    });
  }

  getScoreClass(value: number): string {
    if (value >= 80) return 'text-success fw-bold';
    if (value >= 60) return 'text-warning fw-bold';
    return 'text-danger fw-bold';
  }

  getScoreBgClass(value: number): string {
    if (value >= 80) return 'bg-success';
    if (value >= 60) return 'bg-warning';
    return 'bg-danger';
  }

  getSatisfactionStars(score: number): string {
    const full = Math.floor(score);
    return '★'.repeat(full) + '☆'.repeat(5 - full);
  }

  getBarWidth(value: number, max: number = 100): string {
    return `${Math.min(100, (value / max) * 100).toFixed(1)}%`;
  }
}
