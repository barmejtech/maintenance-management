import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AvailabilityService } from '../../services/availability.service';
import { TechnicianService } from '../../services/technician.service';
import { Availability, CreateAvailabilityRequest, Technician } from '../../models';
import { AuthService } from '../../services/auth.service';
import { ToastService } from '../../services/toast.service';

@Component({
  selector: 'app-availability',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './availability.component.html',
  styleUrls: ['./availability.component.css']
})
export class AvailabilityComponent implements OnInit {
  availabilities = signal<Availability[]>([]);
  technicians = signal<Technician[]>([]);
  errorMessage = signal('');
  showModal = signal(false);
  isEditing = signal(false);
  isSaving = signal(false);

  form: CreateAvailabilityRequest = {
    technicianId: '',
    startTime: '',
    endTime: '',
    isAvailable: true,
    notes: ''
  };
  private editingId = '';

  constructor(private service: AvailabilityService, private techService: TechnicianService, private toast: ToastService, public auth: AuthService) {}

  ngOnInit() {
    this.load();
    this.techService.getAll().subscribe({ next: d => this.technicians.set(d), error: () => {} });
  }

  load() {
    this.service.getAll().subscribe({
      next: d => { this.availabilities.set(d); this.errorMessage.set(''); },
      error: () => this.errorMessage.set('Failed to load availability records.')
    });
  }

  openAdd() {
    this.isEditing.set(false);
    this.editingId = '';
    this.form = { technicianId: '', startTime: '', endTime: '', isAvailable: true, notes: '' };
    this.showModal.set(true);
  }

  openEdit(avail: Availability) {
    this.isEditing.set(true);
    this.editingId = avail.id;
    this.form = {
      technicianId: avail.technicianId,
      startTime: avail.startTime ? avail.startTime.substring(0, 16) : '',
      endTime: avail.endTime ? avail.endTime.substring(0, 16) : '',
      isAvailable: avail.isAvailable,
      notes: avail.notes ?? ''
    };
    this.showModal.set(true);
  }

  closeModal() { this.showModal.set(false); }

  save() {
    this.isSaving.set(true);
    const obs = this.isEditing()
      ? this.service.update(this.editingId, this.form)
      : this.service.create(this.form);
    obs.subscribe({
      next: () => { this.isSaving.set(false); this.showModal.set(false); this.load(); this.toast.success(this.isEditing() ? 'messages.updated' : 'messages.created'); },
      error: () => { this.isSaving.set(false); this.toast.error(); }
    });
  }

  delete(id: string) {
    if (!confirm('Delete this availability record?')) return;
    this.service.delete(id).subscribe({ next: () => { this.load(); this.toast.success('messages.deleted'); }, error: () => this.toast.error() });
  }
}
