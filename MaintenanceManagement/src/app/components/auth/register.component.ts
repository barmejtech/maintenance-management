import { Component, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  if (!password || !confirmPassword) return null;
  return password.value !== confirmPassword.value ? { passwordMismatch: true } : null;
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <div class="auth-header">
          <h1>🔧 Maintenance Management</h1>
          <h2>Create Account</h2>
        </div>
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <div class="form-row">
            <div class="form-group">
              <label>First Name</label>
              <input type="text" formControlName="firstName" placeholder="First name" />
            </div>
            <div class="form-group">
              <label>Last Name</label>
              <input type="text" formControlName="lastName" placeholder="Last name" />
            </div>
          </div>
          <div class="form-group">
            <label>Email</label>
            <input type="email" formControlName="email" placeholder="Enter email" />
          </div>
          <div class="form-group">
            <label>Password</label>
            <input type="password" formControlName="password" placeholder="Min 6 characters" />
          </div>
          <div class="form-group">
            <label>Confirm Password</label>
            <input type="password" formControlName="confirmPassword" placeholder="Repeat password" />
            @if (form.hasError('passwordMismatch') && form.get('confirmPassword')?.touched) {
              <span class="error">Passwords do not match</span>
            }
          </div>
          @if (errorMessage()) {
            <div class="alert alert-error">{{ errorMessage() }}</div>
          }
          <button type="submit" [disabled]="form.invalid || isLoading()" class="btn-primary">
            {{ isLoading() ? 'Creating account...' : 'Create Account' }}
          </button>
        </form>
        <p class="auth-footer">
          Already have an account? <a routerLink="/login">Sign In</a>
        </p>
      </div>
    </div>
  `,
  styles: [`
    .auth-container {
      min-height: 100vh;
      display: flex;
      align-items: center;
      justify-content: center;
      background: linear-gradient(135deg, #1a1a2e 0%, #16213e 50%, #0f3460 100%);
    }
    .auth-card {
      background: white;
      padding: 2.5rem;
      border-radius: 1rem;
      box-shadow: 0 20px 60px rgba(0,0,0,0.3);
      width: 100%;
      max-width: 480px;
    }
    .auth-header { text-align: center; margin-bottom: 2rem; }
    .auth-header h1 { font-size: 1.5rem; color: #0f3460; margin-bottom: 0.5rem; }
    .auth-header h2 { font-size: 1.2rem; color: #666; font-weight: 400; }
    .form-row { display: grid; grid-template-columns: 1fr 1fr; gap: 1rem; }
    .form-group { margin-bottom: 1rem; }
    .form-group label { display: block; margin-bottom: 0.4rem; font-weight: 600; color: #333; font-size: 0.9rem; }
    .form-group input {
      width: 100%;
      padding: 0.75rem 1rem;
      border: 2px solid #e0e0e0;
      border-radius: 0.5rem;
      font-size: 1rem;
      transition: border-color 0.2s;
      box-sizing: border-box;
    }
    .form-group input:focus { outline: none; border-color: #0f3460; }
    .error { color: #e74c3c; font-size: 0.8rem; margin-top: 0.3rem; display: block; }
    .alert-error { background: #fdf0f0; color: #e74c3c; padding: 0.75rem; border-radius: 0.5rem; margin-bottom: 1rem; font-size: 0.9rem; }
    .btn-primary {
      width: 100%;
      padding: 0.875rem;
      background: #0f3460;
      color: white;
      border: none;
      border-radius: 0.5rem;
      font-size: 1rem;
      font-weight: 600;
      cursor: pointer;
    }
    .btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }
    .auth-footer { text-align: center; margin-top: 1.5rem; color: #666; font-size: 0.9rem; }
    .auth-footer a { color: #0f3460; font-weight: 600; text-decoration: none; }
  `]
})
export class RegisterComponent {
  isLoading = signal(false);
  errorMessage = signal('');
  form: ReturnType<FormBuilder['group']>;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordMatchValidator });
  }

  onSubmit() {
    if (this.form.invalid) return;
    this.isLoading.set(true);
    this.errorMessage.set('');

    const v = this.form.value;
    this.auth.register({
      firstName: v.firstName!,
      lastName: v.lastName!,
      email: v.email!,
      password: v.password!,
      confirmPassword: v.confirmPassword!
    }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'Registration failed.');
        this.isLoading.set(false);
      }
    });
  }
}
