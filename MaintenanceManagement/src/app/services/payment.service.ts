import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Payment, PaymentCheckout } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PaymentService {
  private readonly base = `${environment.apiUrl}/payments`;

  constructor(private http: HttpClient) {}

  getByRequestId(requestId: string): Observable<Payment> { return this.http.get<Payment>(`${this.base}/request/${requestId}`); }
  initiatePayment(dto: { premiumMaintenanceRequestId: string; paymentMethod: number }): Observable<PaymentCheckout> { return this.http.post<PaymentCheckout>(`${this.base}/initiate`, dto); }
  confirmPayment(paymentId: string, transactionId: string): Observable<Payment> { return this.http.post<Payment>(`${this.base}/${paymentId}/confirm`, { transactionId }); }
  cancelPayment(paymentId: string): Observable<Payment> { return this.http.post<Payment>(`${this.base}/${paymentId}/cancel`, {}); }
}
