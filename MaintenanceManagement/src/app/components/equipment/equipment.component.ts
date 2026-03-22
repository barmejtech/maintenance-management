import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { EquipmentService } from '../../services/equipment.service';
import { Equipment, EquipmentStatus } from '../../models';

@Component({
  selector: 'app-equipment',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './equipment.component.html',
  styleUrls: ['./equipment.component.css']
})
export class EquipmentComponent implements OnInit {
  equipment = signal<Equipment[]>([]);

  constructor(private service: EquipmentService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.equipment.set(d), error: () => {} });
  }

  getStatusLabel(s: EquipmentStatus): string { return ['Operational', 'Under Maintenance', 'Out of Service', 'Decommissioned'][s]; }
  getStatusClass(s: EquipmentStatus): string { return ['s-operational', 's-maintenance', 's-out-of-service', 's-decommissioned'][s]; }
}
