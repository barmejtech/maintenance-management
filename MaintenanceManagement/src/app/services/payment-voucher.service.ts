import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaymentVoucher, CreatePaymentVoucherRequest, UpdatePaymentVoucherRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PaymentVoucherService {
  private readonly base = `${environment.apiUrl}/payment-vouchers`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<PaymentVoucher[]> { return this.http.get<PaymentVoucher[]>(this.base); }
  getById(id: string): Observable<PaymentVoucher> { return this.http.get<PaymentVoucher>(`${this.base}/${id}`); }
  create(dto: CreatePaymentVoucherRequest): Observable<PaymentVoucher> { return this.http.post<PaymentVoucher>(this.base, dto); }
  update(id: string, dto: UpdatePaymentVoucherRequest): Observable<PaymentVoucher> { return this.http.put<PaymentVoucher>(`${this.base}/${id}`, dto); }
  markPrinted(id: string): Observable<PaymentVoucher> { return this.http.post<PaymentVoucher>(`${this.base}/${id}/mark-printed`, {}); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
