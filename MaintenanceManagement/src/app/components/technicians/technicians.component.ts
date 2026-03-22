import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { TechnicianService } from '../../services/technician.service';
import { Technician, TechnicianStatus } from '../../models';

@Component({
  selector: 'app-technicians',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './technicians.component.html',
  styleUrls: ['./technicians.component.css']
})
export class TechniciansComponent implements OnInit {
  technicians = signal<Technician[]>([]);
  isLoading = signal(true);

  constructor(private service: TechnicianService) {}

  ngOnInit() {
    this.service.getAll().subscribe({
      next: (data) => { this.technicians.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  getStatusLabel(status: TechnicianStatus): string {
    return ['Available', 'Busy', 'On Leave', 'Inactive'][status];
  }

  getStatusClass(status: TechnicianStatus): string {
    return ['status-available', 'status-busy', 'status-on-leave', 'status-inactive'][status];
  }
}
