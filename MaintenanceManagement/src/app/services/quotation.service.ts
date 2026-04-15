import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Quotation } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class QuotationService {
  private readonly base = `${environment.apiUrl}/quotations`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Quotation[]> { return this.http.get<Quotation[]>(this.base); }
  getById(id: string): Observable<Quotation> { return this.http.get<Quotation>(`${this.base}/${id}`); }
  getByStatus(status: number): Observable<Quotation[]> { return this.http.get<Quotation[]>(`${this.base}/status/${status}`); }
  getByClient(clientId: string): Observable<Quotation[]> { return this.http.get<Quotation[]>(`${this.base}/client/${clientId}`); }
  getByRequest(requestId: string): Observable<Quotation[]> { return this.http.get<Quotation[]>(`${this.base}/request/${requestId}`); }
  create(dto: any): Observable<Quotation> { return this.http.post<Quotation>(this.base, dto); }
  update(id: string, dto: any): Observable<Quotation> { return this.http.put<Quotation>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
  sendEmail(id: string): Observable<any> { return this.http.post(`${this.base}/${id}/send-email`, {}); }
}
