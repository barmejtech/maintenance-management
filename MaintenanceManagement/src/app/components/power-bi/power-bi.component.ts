import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { DomSanitizer, SafeResourceUrl } from '@angular/platform-browser';
import { TranslatePipe } from '../../pipes/translate.pipe';
import { TranslationService } from '../../services/translate.service';

@Component({
  selector: 'app-power-bi',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, TranslatePipe],
  templateUrl: './power-bi.component.html',
  styleUrls: ['./power-bi.component.css']
})
export class PowerBiComponent {
  reportUrl = '';
  embedUrl = signal<SafeResourceUrl | null>(null);
  isFullscreen = signal(false);
  urlError = signal('');

  private readonly allowedDomains = ['app.powerbi.com', 'powerbi.com', 'msit.powerbi.com'];

  constructor(
    private sanitizer: DomSanitizer,
    public translation: TranslationService
  ) {}

  loadReport(): void {
    const url = this.reportUrl.trim();
    if (!url) return;

    try {
      const parsed = new URL(url);
      const hostname = parsed.hostname.toLowerCase();
      const isAllowed = this.allowedDomains.some(d => hostname === d || hostname.endsWith('.' + d));
      if (!isAllowed) {
        this.urlError.set(this.translation.translate('powerBi.invalidDomain'));
        return;
      }
    } catch {
      this.urlError.set(this.translation.translate('powerBi.invalidUrl'));
      return;
    }

    this.urlError.set('');
    this.embedUrl.set(this.sanitizer.bypassSecurityTrustResourceUrl(url));
  }

  clearReport(): void {
    this.reportUrl = '';
    this.embedUrl.set(null);
    this.isFullscreen.set(false);
    this.urlError.set('');
  }

  toggleFullscreen(): void {
    this.isFullscreen.update(v => !v);
  }
}
