import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { GroupService } from '../../services/group.service';
import { TechnicianGroup } from '../../models';

@Component({
  selector: 'app-groups',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './groups.component.html',
  styleUrls: ['./groups.component.css']
})
export class GroupsComponent implements OnInit {
  groups = signal<TechnicianGroup[]>([]);

  constructor(private service: GroupService) {}

  ngOnInit() {
    this.service.getAll().subscribe({ next: d => this.groups.set(d), error: () => {} });
  }
}
