import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http';
import { AuthService } from '../../services/auth.service';
import { environment } from '../../../environments/environment';
import { ClientType } from '../../models';

@Component({
  selector: 'app-client-profile',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="profile-page">
      <div class="page-header">
        <h1>My Profile</h1>
        <p>Manage your account information</p>
      </div>

      <div class="profile-grid">
        <!-- Profile Info Card -->
        <div class="profile-card">
          <div class="avatar-section">
            <div class="avatar">
              <i class="bi bi-person-circle"></i>
            </div>
            <div class="user-meta">
              <h2>{{ auth.currentUser()?.firstName }} {{ auth.currentUser()?.lastName }}</h2>
              <span class="role-badge">
                <i class="bi bi-person-check"></i> Client
              </span>
              @if (auth.currentUser()?.companyName) {
                <span class="company-badge">
                  <i class="bi bi-building"></i>
                  {{ auth.currentUser()?.companyName }}
                </span>
              }
              @if (auth.currentUser()?.clientType === ClientType.Company) {
                <span class="type-badge company">Company Account</span>
              } @else {
                <span class="type-badge individual">Individual Account</span>
              }
            </div>
          </div>

          <div class="info-rows">
            <div class="info-row">
              <i class="bi bi-envelope"></i>
              <span class="label">Email</span>
              <span class="value">{{ auth.currentUser()?.email }}</span>
            </div>
            <div class="info-row">
              <i class="bi bi-person-badge"></i>
              <span class="label">Client ID</span>
              <span class="value mono">{{ auth.currentUser()?.clientRecordId ?? '—' }}</span>
            </div>
          </div>
        </div>

        <!-- Edit Form Card -->
        <div class="form-card">
          <h3>Update Profile</h3>

          @if (successMsg()) {
            <div class="alert-success"><i class="bi bi-check-circle"></i> {{ successMsg() }}</div>
          }
          @if (errorMsg()) {
            <div class="alert-error"><i class="bi bi-exclamation-triangle"></i> {{ errorMsg() }}</div>
          }

          <form [formGroup]="form" (ngSubmit)="onSave()">
            <div class="form-row">
              <div class="form-group">
                <label>First Name</label>
                <input type="text" formControlName="firstName" />
              </div>
              <div class="form-group">
                <label>Last Name</label>
                <input type="text" formControlName="lastName" />
              </div>
            </div>
            <div class="form-group">
              <label>Phone Number</label>
              <input type="tel" formControlName="phoneNumber" placeholder="+1 (555) 000-0000" />
            </div>
            @if (auth.currentUser()?.clientType === ClientType.Company) {
              <div class="form-group">
                <label>Company Name</label>
                <input type="text" formControlName="companyName" />
              </div>
            }
            <button type="submit" class="btn-save" [disabled]="isSaving()">
              @if (isSaving()) { Saving... } @else { Save Changes }
            </button>
          </form>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .profile-page { max-width: 900px; margin: 0 auto; }
    .page-header { margin-bottom: 28px; }
    .page-header h1 { font-size: 1.5rem; font-weight: 700; color: #1a237e; margin: 0 0 4px; }
    .page-header p { color: #666; margin: 0; }

    .profile-grid { display: grid; grid-template-columns: 320px 1fr; gap: 24px; }

    .profile-card, .form-card {
      background: white;
      border-radius: 14px;
      padding: 28px;
      box-shadow: 0 2px 10px rgba(0,0,0,0.06);
    }

    .avatar-section { display: flex; flex-direction: column; align-items: center; text-align: center; margin-bottom: 24px; }
    .avatar { font-size: 72px; color: #1565c0; margin-bottom: 12px; line-height: 1; }
    .user-meta h2 { font-size: 1.2rem; font-weight: 700; color: #1a237e; margin: 0 0 10px; }
    .role-badge, .company-badge, .type-badge {
      display: inline-flex;
      align-items: center;
      gap: 4px;
      padding: 4px 12px;
      border-radius: 20px;
      font-size: 0.8rem;
      font-weight: 600;
      margin: 3px;
    }
    .role-badge { background: #e3f2fd; color: #1565c0; }
    .company-badge { background: #f3e5f5; color: #6a1b9a; }
    .type-badge.company { background: #fce4ec; color: #c62828; }
    .type-badge.individual { background: #e8f5e9; color: #2e7d32; }

    .info-rows { display: flex; flex-direction: column; gap: 14px; }
    .info-row { display: grid; grid-template-columns: 20px 80px 1fr; gap: 8px; align-items: center; padding: 10px; background: #f9f9f9; border-radius: 8px; }
    .info-row i { color: #1565c0; }
    .label { font-size: 0.8rem; color: #888; font-weight: 500; }
    .value { font-size: 0.9rem; color: #333; font-weight: 500; overflow: hidden; text-overflow: ellipsis; }
    .mono { font-family: monospace; font-size: 0.8rem; }

    .form-card h3 { font-size: 1.1rem; font-weight: 600; color: #1a237e; margin: 0 0 20px; }
    .alert-success, .alert-error { display: flex; align-items: center; gap: 8px; padding: 12px 16px; border-radius: 8px; margin-bottom: 16px; font-size: 0.9rem; }
    .alert-success { background: #e8f5e9; color: #2e7d32; }
    .alert-error { background: #fdecea; color: #c62828; }

    form { display: flex; flex-direction: column; gap: 16px; }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .form-group { display: flex; flex-direction: column; gap: 6px; }
    .form-group label { font-weight: 500; color: #333; font-size: 0.9rem; }
    .form-group input {
      padding: 10px 14px;
      border: 1.5px solid #e0e0e0;
      border-radius: 8px;
      font-size: 0.95rem;
      outline: none;
    }
    .form-group input:focus { border-color: #1565c0; }

    .btn-save {
      padding: 11px 28px;
      background: linear-gradient(135deg, #1565c0, #1a237e);
      color: white;
      border: none;
      border-radius: 8px;
      font-size: 0.95rem;
      font-weight: 600;
      cursor: pointer;
      align-self: flex-start;
    }
    .btn-save:disabled { opacity: 0.6; cursor: not-allowed; }

    @media (max-width: 768px) { .profile-grid { grid-template-columns: 1fr; } .form-row { grid-template-columns: 1fr; } }
  `]
})
export class ClientProfileComponent {
  ClientType = ClientType;
  isSaving = signal(false);
  successMsg = signal('');
  errorMsg = signal('');
  form: ReturnType<FormBuilder['group']>;

  constructor(
    private fb: FormBuilder,
    private http: HttpClient,
    public auth: AuthService
  ) {
    const user = auth.currentUser();
    this.form = this.fb.group({
      firstName: [user?.firstName ?? '', Validators.required],
      lastName: [user?.lastName ?? '', Validators.required],
      phoneNumber: [''],
      companyName: [user?.companyName ?? '']
    });
  }

  onSave(): void {
    if (this.form.invalid) return;
    this.isSaving.set(true);
    this.successMsg.set('');
    this.errorMsg.set('');

    const v = this.form.value;
    this.http.put(`${environment.apiUrl}/auth/profile`, v).subscribe({
      next: () => {
        this.successMsg.set('Profile updated successfully!');
        this.isSaving.set(false);
        this.auth.updateCurrentUserInfo(v.firstName!, v.lastName!);
      },
      error: (err) => {
        this.errorMsg.set(err.error?.message ?? 'Failed to update profile');
        this.isSaving.set(false);
      }
    });
  }
}
