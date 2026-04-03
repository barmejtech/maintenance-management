import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';
import { TranslationService } from './translate.service';

@Injectable({ providedIn: 'root' })
export class ToastService {
  constructor(private translation: TranslationService) {}

  success(messageKey = 'messages.success'): void {
    Swal.fire({
      icon: 'success',
      title: this.translation.translate(messageKey),
      toast: true,
      position: this.translation.isRtl ? 'top-start' : 'top-end',
      showConfirmButton: false,
      timer: 3000,
      timerProgressBar: true
    });
  }

  error(messageKey = 'messages.error'): void {
    Swal.fire({
      icon: 'error',
      title: this.translation.translate(messageKey),
      toast: true,
      position: this.translation.isRtl ? 'top-start' : 'top-end',
      showConfirmButton: false,
      timer: 4000,
      timerProgressBar: true
    });
  }

  /** Show a toast with a literal message string (no translation lookup). */
  show(message: string, type: 'success' | 'error' | 'warning' | 'info' = 'info'): void {
    Swal.fire({
      icon: type,
      title: message,
      toast: true,
      position: this.translation.isRtl ? 'top-start' : 'top-end',
      showConfirmButton: false,
      timer: type === 'error' ? 4000 : 3000,
      timerProgressBar: true
    });
  }
}
