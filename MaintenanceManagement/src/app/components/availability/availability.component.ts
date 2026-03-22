import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AvailabilityService } from '../../services/availability.service';
import { Availability } from '../../models';

@Component({
  selector: 'app-availability',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './availability.component.html',
  styleUrls: ['./availability.component.css']
})
export class AvailabilityComponent implements OnInit {
  availabilities = signal<Availability[]>([]);
  errorMessage = signal('');

  constructor(private service: AvailabilityService) {}

  ngOnInit() {
    this.service.getAll().subscribe({
      next: d => this.availabilities.set(d),
      error: (err) => {
        console.error('Failed to load availabilities', err);
        this.errorMessage.set('Failed to load availability records. Please try again.');
      }
    });
  }
}
