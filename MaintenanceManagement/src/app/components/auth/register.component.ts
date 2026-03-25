import { Component, signal } from '@angular/core';
import { AbstractControl, FormBuilder, ReactiveFormsModule, ValidationErrors, ValidatorFn, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translate.service';
import { TranslatePipe } from '../../pipes/translate.pipe';

const passwordMatchValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
  const password = control.get('password');
  const confirmPassword = control.get('confirmPassword');
  if (!password || !confirmPassword) return null;
  return password.value !== confirmPassword.value ? { passwordMismatch: true } : null;
};

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslatePipe],
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent {
  isLoading = signal(false);
  errorMessage = signal('');
  form: ReturnType<FormBuilder['group']>;

  readonly roles = ['Technician', 'Manager'];

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    private router: Router,
    public translation: TranslationService
  ) {
    this.form = this.fb.group({
      firstName: ['', Validators.required],
      lastName: ['', Validators.required],
      email: ['', [Validators.required, Validators.email]],
      role: ['Technician', Validators.required],
      password: ['', [Validators.required, Validators.minLength(6)]],
      confirmPassword: ['', Validators.required]
    }, { validators: passwordMatchValidator });
  }

  onSubmit() {
    if (this.form.invalid) return;
    this.isLoading.set(true);
    this.errorMessage.set('');
    this.successMessage.set('');

    const v = this.form.value;
    this.auth.register({
      firstName: v.firstName!,
      lastName: v.lastName!,
      email: v.email!,
      role: v.role!,
      password: v.password!,
      confirmPassword: v.confirmPassword!
    }).subscribe({
      next: () => this.router.navigate(['/technicians']),
      error: (err) => {
        this.errorMessage.set(err.error?.message ?? this.translation.translate('register.failed'));
        this.isLoading.set(false);
      }
    });
  }
}
