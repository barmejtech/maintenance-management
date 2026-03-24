import { Injectable, signal } from '@angular/core';

export type Theme = 'light' | 'dark';

@Injectable({
  providedIn: 'root'
})
export class ThemeService {
  private currentTheme = signal<Theme>('light');

  readonly theme = this.currentTheme.asReadonly();

  get isDark(): boolean {
    return this.currentTheme() === 'dark';
  }

  constructor() {
    const saved = localStorage.getItem('theme') as Theme | null;
    const initial: Theme = saved === 'dark' ? 'dark' : 'light';
    this.currentTheme.set(initial);
    this.applyTheme(initial);
  }

  toggleTheme(): void {
    const next: Theme = this.currentTheme() === 'light' ? 'dark' : 'light';
    this.currentTheme.set(next);
    localStorage.setItem('theme', next);
    this.applyTheme(next);
  }

  private applyTheme(theme: Theme): void {
    document.documentElement.setAttribute('data-theme', theme);
  }
}
