import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { NgxPaginationModule } from 'ngx-pagination';
import { UserService } from '../../services/user.service';
import { FileUploadService } from '../../services/file-upload.service';
import { User } from '../../models';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, RouterLink, TranslatePipe, NgxPaginationModule],
  templateUrl: './users.component.html',
  styleUrls: ['./users.component.css']
})
export class UsersComponent implements OnInit {
  users = signal<User[]>([]);
  currentPage = signal(1);
  readonly pageSize = 12;
  isLoading = signal(true);

  constructor(
    private service: UserService,
    private fileService: FileUploadService,
    private translation: TranslationService
  ) {}

  ngOnInit() {
    this.load();
  }

  load() {
    this.isLoading.set(true);
    this.service.getAll().subscribe({
      next: (data) => { this.users.set(data); this.isLoading.set(false); },
      error: () => this.isLoading.set(false)
    });
  }

  getInitials(user: User): string {
    return `${user.firstName.length > 0 ? user.firstName[0] : ''}${user.lastName.length > 0 ? user.lastName[0] : ''}`.toUpperCase();
  }

  getPhotoUrl(url: string | undefined): string {
    return this.fileService.getPhotoUrl(url ?? '');
  }

  getRoleBadgeClass(role: string): string {
    switch (role.toLowerCase()) {
      case 'admin': return 'bg-danger';
      case 'manager': return 'bg-success';
      case 'technician': return 'bg-primary';
      default: return 'bg-secondary';
    }
  }

  getRoleLabel(role: string): string {
    return this.translation.translate(`users.roles.${role.toLowerCase()}`) || role;
  }
}
