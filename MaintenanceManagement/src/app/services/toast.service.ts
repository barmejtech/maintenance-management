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
}
