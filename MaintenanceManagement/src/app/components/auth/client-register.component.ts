import { Component, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { ClientType } from '../../models';

const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  if (!password || !confirmPassword) return null;
  return password.value !== confirmPassword.value ? { passwordMismatch: true } : null;
};

@Component({
  selector: 'app-client-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="client-register-page">
      <div class="register-card">
        <div class="register-header">
          <i class="bi bi-tools logo-icon"></i>
          <h1>Create Client Account</h1>
          <p>Join our maintenance management platform</p>
        </div>

        @if (errorMessage()) {
          <div class="alert alert-error">{{ errorMessage() }}</div>
        }
        @if (successMessage()) {
          <div class="alert alert-success">{{ successMessage() }}</div>
        }

        <form [formGroup]="form" (ngSubmit)="onSubmit()" class="register-form">
          <!-- Client Type -->
          <div class="form-group">
            <label>Account Type</label>
            <div class="client-type-selector">
              <label class="type-option" [class.selected]="form.get('clientType')?.value === ClientType.Person">
                <input type="radio" formControlName="clientType" [value]="ClientType.Person" />
                <i class="bi bi-person-fill"></i>
                <span>Individual</span>
              </label>
              <label class="type-option" [class.selected]="form.get('clientType')?.value === ClientType.Company">
                <input type="radio" formControlName="clientType" [value]="ClientType.Company" />
                <i class="bi bi-building"></i>
                <span>Company</span>
              </label>
            </div>
          </div>

          <!-- Company Name (conditionally shown) -->
          @if (form.get('clientType')?.value === ClientType.Company) {
            <div class="form-group">
              <label for="companyName">Company Name *</label>
              <input id="companyName" type="text" formControlName="companyName" placeholder="Enter company name" />
              @if (form.get('companyName')?.invalid && form.get('companyName')?.touched) {
                <span class="field-error">Company name is required for company accounts</span>
              }
            </div>
          }

          <!-- Name Fields -->
          <div class="form-row">
            <div class="form-group">
              <label for="firstName">First Name *</label>
              <input id="firstName" type="text" formControlName="firstName" placeholder="First name" />
              @if (form.get('firstName')?.invalid && form.get('firstName')?.touched) {
                <span class="field-error">First name is required</span>
              }
            </div>
            <div class="form-group">
              <label for="lastName">Last Name *</label>
              <input id="lastName" type="text" formControlName="lastName" placeholder="Last name" />
              @if (form.get('lastName')?.invalid && form.get('lastName')?.touched) {
                <span class="field-error">Last name is required</span>
              }
            </div>
          </div>

          <!-- Email -->
          <div class="form-group">
            <label for="email">Email Address *</label>
            <input id="email" type="email" formControlName="email" placeholder="your@email.com" />
            @if (form.get('email')?.invalid && form.get('email')?.touched) {
              <span class="field-error">Valid email is required</span>
            }
          </div>

          <!-- Phone -->
          <div class="form-group">
            <label for="phone">Phone Number</label>
            <input id="phone" type="tel" formControlName="phone" placeholder="+1 (555) 000-0000" />
          </div>

          <!-- Address -->
          <div class="form-group">
            <label for="address">Address</label>
            <input id="address" type="text" formControlName="address" placeholder="Your address" />
          </div>

          <!-- Password Fields -->
          <div class="form-row">
            <div class="form-group">
              <label for="password">Password *</label>
              <input id="password" type="password" formControlName="password" placeholder="Min. 8 characters" />
              @if (form.get('password')?.invalid && form.get('password')?.touched) {
                <span class="field-error">Password must be at least 8 characters</span>
              }
            </div>
            <div class="form-group">
              <label for="confirmPassword">Confirm Password *</label>
              <input id="confirmPassword" type="password" formControlName="confirmPassword" placeholder="Repeat password" />
            </div>
          </div>
          @if (form.errors?.['passwordMismatch'] && form.get('confirmPassword')?.touched) {
            <div class="field-error">Passwords do not match</div>
          }

          <button type="submit" class="btn-register" [disabled]="form.invalid || isLoading()">
            @if (isLoading()) {
              <span>Creating account...</span>
            } @else {
              <span>Create Account</span>
            }
          </button>
        </form>

        <div class="register-footer">
          <p>Already have an account? <a routerLink="/login">Sign in</a></p>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .client-register-page {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #1a237e 0%, #0d47a1 50%, #1565c0 100%);
      padding: 24px;
    }
    .register-card {
      background: white;
      border-radius: 16px;
      padding: 40px;
      width: 100%;
      max-width: 560px;
      box-shadow: 0 20px 60px rgba(0,0,0,0.15);
    }
    .register-header {
      text-align: center;
      margin-bottom: 32px;
    }
    .logo-icon {
      font-size: 48px;
      color: #1565c0;
      display: block;
      margin-bottom: 12px;
    }
    .register-header h1 {
      font-size: 1.75rem;
      font-weight: 700;
      color: #1a237e;
      margin: 0 0 8px;
    }
    .register-header p {
      color: #666;
      margin: 0;
    }
    .alert {
      padding: 12px 16px;
      border-radius: 8px;
      margin-bottom: 20px;
      font-size: 0.9rem;
    }
    .alert-error { background: #fdecea; color: #c62828; border: 1px solid #ef9a9a; }
    .alert-success { background: #e8f5e9; color: #2e7d32; border: 1px solid #a5d6a7; }
    .register-form { display: flex; flex-direction: column; gap: 16px; }
    .form-group { display: flex; flex-direction: column; gap: 6px; }
    .form-group label { font-weight: 500; color: #333; font-size: 0.9rem; }
    .form-group input {
      padding: 10px 14px;
      border: 1.5px solid #e0e0e0;
      border-radius: 8px;
      font-size: 0.95rem;
      transition: border-color 0.2s;
      outline: none;
    }
    .form-group input:focus { border-color: #1565c0; }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 16px; }
    .field-error { color: #c62828; font-size: 0.8rem; }
    .client-type-selector {
      display: flex;
      gap: 12px;
    }
    .type-option {
      flex: 1;
      display: flex;
      flex-direction: column;
      align-items: center;
      gap: 8px;
      padding: 16px;
      border: 2px solid #e0e0e0;
      border-radius: 10px;
      cursor: pointer;
      transition: all 0.2s;
      font-weight: 500;
      color: #555;
    }
    .type-option input[type="radio"] { display: none; }
    .type-option i { font-size: 24px; }
    .type-option.selected { border-color: #1565c0; color: #1565c0; background: #e3f2fd; }
    .btn-register {
      padding: 13px;
      background: linear-gradient(135deg, #1565c0, #1a237e);
      color: white;
      border: none;
      border-radius: 8px;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
      transition: opacity 0.2s;
      margin-top: 8px;
    }
    .btn-register:disabled { opacity: 0.7; cursor: not-allowed; }
    .btn-register:hover:not(:disabled) { opacity: 0.9; }
    .register-footer {
      text-align: center;
      margin-top: 24px;
      color: #666;
    }
    .register-footer a { color: #1565c0; font-weight: 500; text-decoration: none; }
    @media (max-width: 480px) {
      .register-card { padding: 24px; }
      .form-row { grid-template-columns: 1fr; }
    }
  `]
})
export class ClientRegisterComponent {
  ClientType = ClientType;
  isLoading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  form: ReturnType<FormBuilder['group']>;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router
  ) {
    this.form = this.fb.group({
      clientType: [ClientType.Person, Validators.required],
      companyName: [''],
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      phone: [''],
      address: [''],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordMatchValidator });

    // Add/remove companyName validator based on clientType
    this.form.get('clientType')?.valueChanges.subscribe(type => {
      const companyCtrl = this.form.get('companyName');
      if (type === ClientType.Company) {
        companyCtrl?.setValidators(Validators.required);
      } else {
        companyCtrl?.clearValidators();
      }
      companyCtrl?.updateValueAndValidity();
    });
  }

  onSubmit() {
    if (this.form.invalid) return;
    this.isLoading.set(true);
    this.errorMessage.set('');

    const v = this.form.value;
    this.auth.clientRegister({
      firstName: v.firstName!,
      lastName: v.lastName!,
      email: v.email!,
      password: v.password!,
      confirmPassword: v.confirmPassword!,
      clientType: v.clientType!,
      companyName: v.clientType === ClientType.Company ? v.companyName! : undefined,
      phone: v.phone || undefined,
      address: v.address || undefined
    }).subscribe({
      next: () => this.router.navigate(['/client-portal']),
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'Registration failed. Please try again.');
        this.isLoading.set(false);
      }
    });
  }
}
