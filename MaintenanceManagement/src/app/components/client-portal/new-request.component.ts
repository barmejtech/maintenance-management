import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { MaintenanceRequestService } from '../../services/maintenance-request.service';

@Component({
  selector: 'app-new-request',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="new-request-page">
      <div class="page-header">
        <a routerLink="/client-portal/my-requests" class="back-link">
          <i class="bi bi-arrow-left"></i> Back to My Requests
        </a>
        <h1>Submit New Maintenance Request</h1>
        <p>Describe your maintenance issue and we'll handle it promptly</p>
      </div>

      @if (successMessage()) {
        <div class="alert-success">
          <i class="bi bi-check-circle-fill"></i>
          {{ successMessage() }}
          <a routerLink="/client-portal/my-requests">View My Requests</a>
        </div>
      }

      <div class="form-card">
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <!-- Title -->
          <div class="form-group">
            <label for="title">Request Title *</label>
            <input id="title" type="text" formControlName="title"
              placeholder="e.g. Broken air conditioning unit in office 3B" />
            @if (form.get('title')?.invalid && form.get('title')?.touched) {
              <span class="field-error">Please provide a title for your request</span>
            }
          </div>

          <!-- Description -->
          <div class="form-group">
            <label for="description">Description</label>
            <textarea id="description" formControlName="description" rows="4"
              placeholder="Describe the issue in detail — what happened, when it started, severity, etc."></textarea>
          </div>

          <!-- Equipment Description -->
          <div class="form-group">
            <label for="equipmentDescription">Equipment / Location</label>
            <input id="equipmentDescription" type="text" formControlName="equipmentDescription"
              placeholder="e.g. AC unit model XYZ-200 in 3rd floor office" />
            <small>Optional: Describe the equipment or location involved</small>
          </div>

          <!-- Request Date -->
          <div class="form-group">
            <label for="requestDate">Preferred Service Date</label>
            <input id="requestDate" type="date" formControlName="requestDate" [min]="today" />
            <small>Optional: When would you prefer the service?</small>
          </div>

          <!-- Notes -->
          <div class="form-group">
            <label for="notes">Additional Notes</label>
            <textarea id="notes" formControlName="notes" rows="2"
              placeholder="Any additional information, access instructions, or special requirements..."></textarea>
          </div>

          @if (errorMessage()) {
            <div class="alert-error">
              <i class="bi bi-exclamation-triangle-fill"></i>
              {{ errorMessage() }}
            </div>
          }

          <div class="form-actions">
            <a routerLink="/client-portal/my-requests" class="btn-cancel">Cancel</a>
            <button type="submit" class="btn-submit" [disabled]="form.invalid || isLoading()">
              @if (isLoading()) {
                <span><i class="bi bi-hourglass-split"></i> Submitting...</span>
              } @else {
                <span><i class="bi bi-send"></i> Submit Request</span>
              }
            </button>
          </div>
        </form>
      </div>

      <!-- Info Box -->
      <div class="info-box">
        <h3><i class="bi bi-info-circle"></i> What happens next?</h3>
        <ol>
          <li>Your request is reviewed by our support team</li>
          <li>A technician is assigned to your request</li>
          <li>You receive updates via notifications and chat</li>
          <li>Work is completed and you confirm the resolution</li>
        </ol>
      </div>
    </div>
  `,
  styles: [`
    .new-request-page { max-width: 720px; margin: 0 auto; }
    .page-header { margin-bottom: 28px; }
    .back-link { display: inline-flex; align-items: center; gap: 6px; color: #1565c0; text-decoration: none; font-size: 0.9rem; margin-bottom: 16px; }
    .back-link:hover { text-decoration: underline; }
    .page-header h1 { font-size: 1.5rem; font-weight: 700; color: #1a237e; margin: 0 0 6px; }
    .page-header p { color: #666; margin: 0; }

    .alert-success {
      display: flex;
      align-items: center;
      gap: 10px;
      background: #e8f5e9;
      color: #2e7d32;
      border: 1px solid #a5d6a7;
      border-radius: 10px;
      padding: 16px 20px;
      margin-bottom: 24px;
      flex-wrap: wrap;
    }
    .alert-success a { color: #1b5e20; font-weight: 600; margin-left: auto; }
    .alert-error {
      display: flex;
      align-items: center;
      gap: 10px;
      background: #fdecea;
      color: #c62828;
      border-radius: 8px;
      padding: 12px 16px;
      margin-bottom: 16px;
    }

    .form-card {
      background: white;
      border-radius: 14px;
      padding: 32px;
      box-shadow: 0 2px 12px rgba(0,0,0,0.07);
      margin-bottom: 24px;
    }
    form { display: flex; flex-direction: column; gap: 20px; }
    .form-group { display: flex; flex-direction: column; gap: 6px; }
    .form-group label { font-weight: 600; color: #333; font-size: 0.9rem; }
    .form-group input, .form-group textarea {
      padding: 11px 14px;
      border: 1.5px solid #e0e0e0;
      border-radius: 8px;
      font-size: 0.95rem;
      outline: none;
      font-family: inherit;
      resize: vertical;
      transition: border-color 0.2s;
    }
    .form-group input:focus, .form-group textarea:focus { border-color: #1565c0; }
    .form-group small { color: #888; font-size: 0.8rem; }
    .field-error { color: #c62828; font-size: 0.82rem; }

    .form-actions { display: flex; justify-content: flex-end; gap: 12px; margin-top: 8px; }
    .btn-cancel {
      padding: 11px 24px;
      border: 1.5px solid #ddd;
      border-radius: 8px;
      color: #666;
      text-decoration: none;
      font-weight: 500;
    }
    .btn-cancel:hover { border-color: #999; }
    .btn-submit {
      display: flex;
      align-items: center;
      gap: 8px;
      padding: 11px 28px;
      background: linear-gradient(135deg, #1565c0, #1a237e);
      color: white;
      border: none;
      border-radius: 8px;
      font-size: 0.95rem;
      font-weight: 600;
      cursor: pointer;
      transition: opacity 0.2s;
    }
    .btn-submit:disabled { opacity: 0.6; cursor: not-allowed; }
    .btn-submit:hover:not(:disabled) { opacity: 0.9; }

    .info-box {
      background: #e3f2fd;
      border-radius: 12px;
      padding: 20px 24px;
      color: #1565c0;
    }
    .info-box h3 { display: flex; align-items: center; gap: 8px; margin: 0 0 12px; font-size: 1rem; }
    .info-box ol { margin: 0; padding-left: 20px; }
    .info-box li { margin-bottom: 6px; font-size: 0.9rem; }
  `]
})
export class NewRequestComponent {
  isLoading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  today = new Date().toISOString().split('T')[0];

  form: ReturnType<FormBuilder['group']>;

  constructor(
    private fb: FormBuilder,
    private requestService: MaintenanceRequestService,
    private router: Router
  ) {
    this.form = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(5)]],
      description: [''],
      equipmentDescription: [''],
      requestDate: [''],
      notes: ['']
    });
  }

  onSubmit(): void {
    if (this.form.invalid) return;
    this.isLoading.set(true);
    this.errorMessage.set('');

    const v = this.form.value;
    this.requestService.submitRequest({
      title: v.title!,
      description: v.description || undefined,
      equipmentDescription: v.equipmentDescription || undefined,
      requestDate: v.requestDate || undefined,
      notes: v.notes || undefined
    }).subscribe({
      next: () => {
        this.successMessage.set('Your maintenance request has been submitted successfully!');
        this.isLoading.set(false);
        this.form.reset();
        setTimeout(() => this.router.navigate(['/client-portal/my-requests']), 2000);
      },
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'Failed to submit request. Please try again.');
        this.isLoading.set(false);
      }
    });
  }
}
