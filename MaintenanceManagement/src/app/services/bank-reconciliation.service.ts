import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { BankReconciliation, CreateBankReconciliationRequest, CompleteReconciliationRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BankReconciliationService {
  private readonly base = `${environment.apiUrl}/bank-reconciliations`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<BankReconciliation[]> { return this.http.get<BankReconciliation[]>(this.base); }
  getById(id: string): Observable<BankReconciliation> { return this.http.get<BankReconciliation>(`${this.base}/${id}`); }
  create(dto: CreateBankReconciliationRequest): Observable<BankReconciliation> { return this.http.post<BankReconciliation>(this.base, dto); }
  reconcile(id: string, dto: CompleteReconciliationRequest): Observable<BankReconciliation> {
    return this.http.post<BankReconciliation>(`${this.base}/${id}/reconcile`, dto);
  }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
