import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Account, CreateAccountRequest, UpdateAccountRequest, TrialBalance, AccountBalance } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class GlAccountService {
  private readonly base = `${environment.apiUrl}/gl-accounts`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Account[]> { return this.http.get<Account[]>(this.base); }
  getById(id: string): Observable<Account> { return this.http.get<Account>(`${this.base}/${id}`); }
  getBalance(id: string): Observable<AccountBalance> { return this.http.get<AccountBalance>(`${this.base}/${id}/balance`); }

  getTrialBalance(asOfDate?: string): Observable<TrialBalance> {
    let params = new HttpParams();
    if (asOfDate) params = params.set('asOfDate', asOfDate);
    return this.http.get<TrialBalance>(`${this.base}/trial-balance`, { params });
  }

  create(dto: CreateAccountRequest): Observable<Account> { return this.http.post<Account>(this.base, dto); }
  update(id: string, dto: UpdateAccountRequest): Observable<Account> { return this.http.put<Account>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
