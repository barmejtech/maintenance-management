import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';
import { Observable, tap, catchError, throwError } from 'rxjs';
import { AuthResponse, LoginRequest, RegisterRequest, RefreshTokenRequest } from '../models';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private readonly apiUrl = 'http://localhost:5214/api/auth';
  private readonly TOKEN_KEY = 'access_token';
  private readonly REFRESH_TOKEN_KEY = 'refresh_token';
  private readonly USER_KEY = 'current_user';

  private currentUserSignal = signal<AuthResponse | null>(this.loadUser());
  readonly currentUser = computed(() => this.currentUserSignal());
  readonly isAuthenticated = computed(() => !!this.currentUserSignal());
  readonly userRoles = computed(() => this.currentUserSignal()?.roles ?? []);

  constructor(private http: HttpClient, private router: Router) {}

  register(request: RegisterRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/register`, request).pipe(
      tap(response => this.storeAuth(response))
    );
  }

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, request).pipe(
      tap(response => this.storeAuth(response))
    );
  }

  refreshToken(): Observable<AuthResponse> {
    const request: RefreshTokenRequest = {
      accessToken: this.getAccessToken() ?? '',
      refreshToken: this.getRefreshToken() ?? ''
    };
    return this.http.post<AuthResponse>(`${this.apiUrl}/refresh`, request).pipe(
      tap(response => this.storeAuth(response)),
      catchError(err => {
        this.logout();
        return throwError(() => err);
      })
    );
  }

  logout(): void {
    if (this.isAuthenticated()) {
      this.http.post(`${this.apiUrl}/revoke`, {}).subscribe();
    }
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.REFRESH_TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.currentUserSignal.set(null);
    this.router.navigate(['/login']);
  }

  getAccessToken(): string | null {
    return localStorage.getItem(this.TOKEN_KEY);
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(this.REFRESH_TOKEN_KEY);
  }

  hasRole(role: string): boolean {
    return this.userRoles().includes(role);
  }

  isAdmin(): boolean {
    return this.hasRole('Admin');
  }

  isManager(): boolean {
    return this.hasRole('Admin') || this.hasRole('Manager');
  }

  private storeAuth(response: AuthResponse): void {
    localStorage.setItem(this.TOKEN_KEY, response.accessToken);
    localStorage.setItem(this.REFRESH_TOKEN_KEY, response.refreshToken);
    localStorage.setItem(this.USER_KEY, JSON.stringify(response));
    this.currentUserSignal.set(response);
  }

  private loadUser(): AuthResponse | null {
    const stored = localStorage.getItem(this.USER_KEY);
    return stored ? JSON.parse(stored) : null;
  }
}
