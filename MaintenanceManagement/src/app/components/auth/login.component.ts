import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="auth-container">
      <div class="auth-card">
        <div class="auth-header">
          <h1>🔧 Maintenance Management</h1>
          <h2>Sign In</h2>
        </div>
        <form [formGroup]="form" (ngSubmit)="onSubmit()">
          <div class="form-group">
            <label for="email">Email</label>
            <input id="email" type="email" formControlName="email" placeholder="Enter email" />
            @if (form.get('email')?.invalid && form.get('email')?.touched) {
              <span class="error">Valid email is required</span>
            }
          </div>
          <div class="form-group">
            <label for="password">Password</label>
            <input id="password" type="password" formControlName="password" placeholder="Enter password" />
            @if (form.get('password')?.invalid && form.get('password')?.touched) {
              <span class="error">Password is required</span>
            }
          </div>
          @if (errorMessage()) {
            <div class="alert alert-error">{{ errorMessage() }}</div>
          }
          <button type="submit" [disabled]="form.invalid || isLoading()" class="btn-primary">
            {{ isLoading() ? 'Signing in...' : 'Sign In' }}
          </button>
        </form>
        <p class="auth-footer">
          Don't have an account? <a routerLink="/register">Register</a>
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
      max-width: 420px;
    }
    .auth-header { text-align: center; margin-bottom: 2rem; }
    .auth-header h1 { font-size: 1.5rem; color: #0f3460; margin-bottom: 0.5rem; }
    .auth-header h2 { font-size: 1.2rem; color: #666; font-weight: 400; }
    .form-group { margin-bottom: 1.2rem; }
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
    .error { color: #e74c3c; font-size: 0.8rem; margin-top: 0.3rem; }
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
      transition: background 0.2s;
    }
    .btn-primary:hover:not(:disabled) { background: #1a5276; }
    .btn-primary:disabled { opacity: 0.6; cursor: not-allowed; }
    .auth-footer { text-align: center; margin-top: 1.5rem; color: #666; font-size: 0.9rem; }
    .auth-footer a { color: #0f3460; font-weight: 600; text-decoration: none; }
  `]
})
export class LoginComponent {
  isLoading = signal(false);
  errorMessage = signal('');
  form: ReturnType<FormBuilder['group']>;

  constructor(private fb: FormBuilder, private auth: AuthService, private router: Router) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', Validators.required]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;
    this.isLoading.set(true);
    this.errorMessage.set('');

    const { email, password } = this.form.value;
    this.auth.login({ email: email!, password: password! }).subscribe({
      next: () => this.router.navigate(['/dashboard']),
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? 'Login failed. Please try again.');
        this.isLoading.set(false);
      }
    });
  }
}
