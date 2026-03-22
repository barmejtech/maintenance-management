import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { HVACService } from '../../services/hvac.service';
import { HVACSystem } from '../../models';

@Component({
  selector: 'app-hvac',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './hvac.component.html',
  styleUrls: ['./hvac.component.css']
})
export class HVACComponent implements OnInit {
  systems = signal<HVACSystem[]>([]);

  constructor(private service: HVACService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.systems.set(d), error: () => {} });
  }
}
