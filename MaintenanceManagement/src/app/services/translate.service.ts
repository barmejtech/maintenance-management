import { Injectable, signal } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { catchError, tap } from 'rxjs/operators';

export type Language = 'en' | 'ar';

@Injectable({
  providedIn: 'root'
})
export class TranslationService {
  private translationsData = signal<Record<string, any>>({});
  private currentLang = signal<Language>('en');
  private loadedLangs = new Set<string>();

  readonly lang = this.currentLang.asReadonly();

  constructor(private http: HttpClient) {
    const saved = localStorage.getItem('lang') as Language | null;
    const initialLang: Language = (saved === 'ar' || saved === 'en') ? saved : 'en';
    this.currentLang.set(initialLang);
    this.applyDirection(initialLang);
    this.loadTranslations(initialLang).subscribe();
  }

  get currentLanguage(): Language {
    return this.currentLang();
  }

  get isRtl(): boolean {
    return this.currentLang() === 'ar';
  }

  switchLanguage(lang: Language): void {
    if (lang === this.currentLang()) return;
    this.loadTranslations(lang).subscribe(() => {
      this.currentLang.set(lang);
      localStorage.setItem('lang', lang);
      this.applyDirection(lang);
    });
  }

  toggleLanguage(): void {
    this.switchLanguage(this.currentLang() === 'en' ? 'ar' : 'en');
  }

  translate(key: string): string {
    const lang = this.currentLang();
    const all = this.translationsData();
    const trans = all[lang];
    if (!trans) return key;
    return this.getNestedValue(trans, key) ?? key;
  }

  private getNestedValue(obj: any, path: string): string | undefined {
    const parts = path.split('.');
    let current = obj;
    for (const part of parts) {
      if (current == null || typeof current !== 'object') return undefined;
      current = current[part];
    }
    return typeof current === 'string' ? current : undefined;
  }

  private loadTranslations(lang: Language): Observable<any> {
    if (this.loadedLangs.has(lang)) {
      return of(this.translationsData()[lang]);
    }
    return this.http.get<any>(`/i18n/${lang}.json`).pipe(
      tap(data => {
        this.translationsData.update(curr => ({ ...curr, [lang]: data }));
        this.loadedLangs.add(lang);
      }),
      catchError(() => {
        console.warn(`Could not load translations for language: ${lang}`);
        return of({});
      })
    );
  }

  private applyDirection(lang: Language): void {
    const dir = lang === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.setAttribute('dir', dir);
    document.documentElement.setAttribute('lang', lang);
  }
}