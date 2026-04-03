import { Component, signal } from '@angular/core';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../services/auth.service';
import { TranslationService } from '../../services/translate.service';
import { TranslatePipe } from '../../pipes/translate.pipe';

@Component({
  selector: 'app-forgot-password',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink, TranslatePipe],
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.css']
})
export class ForgotPasswordComponent {
  isLoading = signal(false);
  successMessage = signal('');
  errorMessage = signal('');
  submitted = signal(false);
  form: ReturnType<FormBuilder['group']>;

  constructor(
    private fb: FormBuilder,
    private auth: AuthService,
    public translation: TranslationService
  ) {
    this.form = this.fb.group({
      email: ['', [Validators.required, Validators.email]]
    });
  }

  onSubmit() {
    if (this.form.invalid) return;
    this.isLoading.set(true);
    this.errorMessage.set('');

    const { email } = this.form.value;
    this.auth.forgotPassword(email!).subscribe({
      next: () => {
        this.submitted.set(true);
        this.successMessage.set(this.translation.translate('forgotPassword.successMessage'));
        this.isLoading.set(false);
      },
      error: () => {
        // Still show success to prevent email enumeration
        this.submitted.set(true);
        this.successMessage.set(this.translation.translate('forgotPassword.successMessage'));
        this.isLoading.set(false);
      }
    });
  }
}
