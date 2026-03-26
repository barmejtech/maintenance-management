import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';
import { AccountService } from '../../services/account.service';
import { AuthService } from '../../services/auth.service';
import { FileUploadService } from '../../services/file-upload.service';
import { AccountProfile, UpdateProfileRequest, ChangePasswordRequest } from '../../models';

@Component({
  selector: 'app-account',
  standalone: true,
  imports: [CommonModule, FormsModule, TranslatePipe],
  templateUrl: './account.component.html',
  styleUrls: ['./account.component.css']
})
export class AccountComponent implements OnInit {
  profile = signal<AccountProfile | null>(null);
  isLoading = signal(true);
  isSavingProfile = signal(false);
  isChangingPassword = signal(false);
  isUploadingPhoto = signal(false);

  activeTab = signal<'profile' | 'password'>('profile');

  profileForm: UpdateProfileRequest = {
    firstName: '',
    lastName: '',
    phoneNumber: '',
    profilePhotoUrl: ''
  };

  passwordForm: ChangePasswordRequest = {
    currentPassword: '',
    newPassword: '',
    confirmNewPassword: ''
  };

  profileSuccess = signal('');
  profileError = signal('');
  passwordSuccess = signal('');
  passwordError = signal('');

  showCurrentPassword = signal(false);
  showNewPassword = signal(false);
  showConfirmPassword = signal(false);

  constructor(
    private accountService: AccountService,
    private authService: AuthService,
    private fileService: FileUploadService,
    public translation: TranslationService
  ) {}

  ngOnInit(): void {
    this.loadProfile();
  }

  loadProfile(): void {
    this.isLoading.set(true);
    this.accountService.getProfile().subscribe({
      next: (data) => {
        this.profile.set(data);
        this.profileForm = {
          firstName: data.firstName,
          lastName: data.lastName,
          phoneNumber: data.phoneNumber ?? '',
          profilePhotoUrl: data.profilePhotoUrl ?? ''
        };
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  setTab(tab: 'profile' | 'password'): void {
    this.activeTab.set(tab);
    this.clearMessages();
  }

  saveProfile(): void {
    if (!this.profileForm.firstName.trim() || !this.profileForm.lastName.trim()) {
      this.profileError.set(this.translation.translate('account.errors.nameRequired'));
      return;
    }
    this.clearMessages();
    this.isSavingProfile.set(true);
    this.accountService.updateProfile(this.profileForm).subscribe({
      next: (updated) => {
        this.profile.set(updated);
        this.authService.updateCurrentUserInfo(
          updated.firstName,
          updated.lastName,
          updated.profilePhotoUrl
        );
        this.profileSuccess.set(this.translation.translate('account.profileUpdated'));
        this.isSavingProfile.set(false);
      },
      error: (err) => {
        this.profileError.set(err.error?.message ?? this.translation.translate('account.errors.updateFailed'));
        this.isSavingProfile.set(false);
      }
    });
  }

  changePassword(): void {
    if (!this.passwordForm.currentPassword || !this.passwordForm.newPassword || !this.passwordForm.confirmNewPassword) {
      this.passwordError.set(this.translation.translate('account.errors.allFieldsRequired'));
      return;
    }
    if (this.passwordForm.newPassword !== this.passwordForm.confirmNewPassword) {
      this.passwordError.set(this.translation.translate('account.errors.passwordsMismatch'));
      return;
    }
    if (this.passwordForm.newPassword.length < 8) {
      this.passwordError.set(this.translation.translate('account.errors.passwordTooShort'));
      return;
    }
    this.clearMessages();
    this.isChangingPassword.set(true);
    this.accountService.changePassword(this.passwordForm).subscribe({
      next: () => {
        this.passwordSuccess.set(this.translation.translate('account.passwordChanged'));
        this.passwordForm = { currentPassword: '', newPassword: '', confirmNewPassword: '' };
        this.isChangingPassword.set(false);
      },
      error: (err) => {
        this.passwordError.set(err.error?.message ?? this.translation.translate('account.errors.passwordChangeFailed'));
        this.isChangingPassword.set(false);
      }
    });
  }

  uploadPhoto(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;
    this.isUploadingPhoto.set(true);
    this.fileService.uploadPhoto(Array.from(input.files)).subscribe({
      next: (results) => {
        if (results.length > 0) {
          this.profileForm.profilePhotoUrl = this.fileService.getPhotoUrl(results[0].url);
        }
        this.isUploadingPhoto.set(false);
      },
      error: () => {
        this.profileError.set(this.translation.translate('account.errors.photoUploadFailed'));
        this.isUploadingPhoto.set(false);
      }
    });
  }

  getPhotoUrl(url: string | undefined): string {
    return this.fileService.getPhotoUrl(url ?? '');
  }

  getRoleBadgeClass(role: string): string {
    switch (role.toLowerCase()) {
      case 'admin': return 'badge bg-danger';
      case 'manager': return 'badge bg-primary';
      default: return 'badge bg-success';
    }
  }

  private clearMessages(): void {
    this.profileSuccess.set('');
    this.profileError.set('');
    this.passwordSuccess.set('');
    this.passwordError.set('');
  }
}
