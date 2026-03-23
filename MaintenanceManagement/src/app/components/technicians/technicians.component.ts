import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { TechnicianService } from '../../services/technician.service';
import { Technician, TechnicianStatus, CreateTechnicianRequest, UpdateTechnicianRequest } from '../../models';

@Component({
  selector: 'app-technicians',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './technicians.component.html',
  styleUrls: ['./technicians.component.css']
})
export class TechniciansComponent implements OnInit {
  technicians = signal<Technician[]>([]);
  isLoading = signal(true);
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);
  TechnicianStatus = TechnicianStatus;

  form: {
    firstName: string; lastName: string; email: string;
    phone: string; specialization: string; password: string;
    status: TechnicianStatus;
  } = { firstName: '', lastName: '', email: '', phone: '', specialization: '', password: '', status: TechnicianStatus.Available };
  private editingId = '';

  constructor(private service: TechnicianService) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: (data) => { this.technicians.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { firstName: '', lastName: '', email: '', phone: '', specialization: '', password: '', status: TechnicianStatus.Available };
    this.showModal.set(true);
  }

  openEdit(tech: Technician) {
    this.isEditing.set(true);
    this.editingId = tech.id;
    this.form = { firstName: tech.firstName, lastName: tech.lastName, email: tech.email, phone: tech.phone, specialization: tech.specialization, password: '', status: tech.status };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    if (this.isEditing()) {
      const dto: UpdateTechnicianRequest = { firstName: this.form.firstName, lastName: this.form.lastName, phone: this.form.phone, specialization: this.form.specialization, status: this.form.status };
      this.service.update(this.editingId, dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    } else {
      const dto: CreateTechnicianRequest = { firstName: this.form.firstName, lastName: this.form.lastName, email: this.form.email, phone: this.form.phone, specialization: this.form.specialization, password: this.form.password };
      this.service.create(dto).subscribe({
        next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); },
        error: () => this.isSaving.set(false)
      });
    }
  }

  delete(id: string) {
    if (!confirm('Delete this technician?')) return;
    this.service.delete(id).subscribe({ next: () => this.load(), error: () => {} });
  }

  getStatusLabel(status: TechnicianStatus): string {
    return ['Available', 'Busy', 'On Leave', 'Inactive'][status];
  }

  getStatusClass(status: TechnicianStatus): string {
    return ['status-available', 'status-busy', 'status-on-leave', 'status-inactive'][status];
  }
}
