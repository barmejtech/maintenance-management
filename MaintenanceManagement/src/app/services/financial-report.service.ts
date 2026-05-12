import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TrialBalance, ProfitLoss, BalanceSheet, CashFlow, AgingReport } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class FinancialReportService {
  private readonly base = `${environment.apiUrl}/financial-reports`;

  constructor(private http: HttpClient) {}

  getTrialBalance(asOfDate?: string): Observable<TrialBalance> {
    let params = new HttpParams();
    if (asOfDate) params = params.set('asOfDate', asOfDate);
    return this.http.get<TrialBalance>(`${this.base}/trial-balance`, { params });
  }

  getProfitLoss(startDate: string, endDate: string): Observable<ProfitLoss> {
    const params = new HttpParams().set('startDate', startDate).set('endDate', endDate);
    return this.http.get<ProfitLoss>(`${this.base}/profit-loss`, { params });
  }

  getBalanceSheet(asOfDate?: string): Observable<BalanceSheet> {
    let params = new HttpParams();
    if (asOfDate) params = params.set('asOfDate', asOfDate);
    return this.http.get<BalanceSheet>(`${this.base}/balance-sheet`, { params });
  }

  getCashFlow(startDate: string, endDate: string): Observable<CashFlow> {
    const params = new HttpParams().set('startDate', startDate).set('endDate', endDate);
    return this.http.get<CashFlow>(`${this.base}/cash-flow`, { params });
  }

  getAgingReport(asOfDate?: string): Observable<AgingReport> {
    let params = new HttpParams();
    if (asOfDate) params = params.set('asOfDate', asOfDate);
    return this.http.get<AgingReport>(`${this.base}/aging-report`, { params });
  }
}
