import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Invoice } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly base = `${environment.apiUrl}/invoices`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Invoice[]> { return this.http.get<Invoice[]>(this.base); }
  getById(id: string): Observable<Invoice> { return this.http.get<Invoice>(`${this.base}/${id}`); }
  getByReport(reportId: string): Observable<Invoice[]> { return this.http.get<Invoice[]>(`${this.base}/report/${reportId}`); }
  getByUnit(unitId: string): Observable<Invoice[]> { return this.http.get<Invoice[]>(`${this.base}/unit/${unitId}`); }
  getByTenant(tenantId: string): Observable<Invoice[]> { return this.http.get<Invoice[]>(`${this.base}/tenant/${tenantId}`); }
  getByUnitOwnership(unitOwnershipId: string): Observable<Invoice[]> { return this.http.get<Invoice[]>(`${this.base}/ownership/${unitOwnershipId}`); }
  getUnpaid(asOfDate?: string): Observable<Invoice[]> {
    const params = asOfDate ? { asOfDate } : undefined;
    return this.http.get<Invoice[]>(`${this.base}/unpaid`, { params });
  }
  create(dto: any): Observable<Invoice> { return this.http.post<Invoice>(this.base, dto); }
  update(id: string, dto: any): Observable<Invoice> { return this.http.put<Invoice>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
