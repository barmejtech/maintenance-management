import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { AccountProfile, UpdateProfileRequest, ChangePasswordRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  private readonly apiUrl = `${environment.apiUrl}/account`;

  constructor(private http: HttpClient) {}

  getProfile(): Observable<AccountProfile> {
    return this.http.get<AccountProfile>(`${this.apiUrl}/profile`);
  }

  updateProfile(request: UpdateProfileRequest): Observable<AccountProfile> {
    return this.http.put<AccountProfile>(`${this.apiUrl}/profile`, request);
  }

  changePassword(request: ChangePasswordRequest): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/change-password`, request);
  }
}
