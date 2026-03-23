import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TechnicianPerformanceScore, SmartDispatchResult } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PerformanceService {
  private readonly base = `${environment.apiUrl}/technicianperformance`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<TechnicianPerformanceScore[]> {
    return this.http.get<TechnicianPerformanceScore[]>(this.base);
  }

  getByTechnicianId(technicianId: string): Observable<TechnicianPerformanceScore> {
    return this.http.get<TechnicianPerformanceScore>(`${this.base}/technician/${technicianId}`);
  }

  getTopPerformers(count: number = 10): Observable<TechnicianPerformanceScore[]> {
    return this.http.get<TechnicianPerformanceScore[]>(`${this.base}/top?count=${count}`);
  }

  recalculate(technicianId: string): Observable<TechnicianPerformanceScore> {
    return this.http.post<TechnicianPerformanceScore>(`${this.base}/recalculate/${technicianId}`, {});
  }

  updateSatisfaction(technicianId: string, score: number): Observable<void> {
    return this.http.patch<void>(`${this.base}/satisfaction/${technicianId}`, { score });
  }

  getSmartDispatch(taskOrderId: string): Observable<SmartDispatchResult> {
    return this.http.get<SmartDispatchResult>(`${environment.apiUrl}/taskorders/${taskOrderId}/smart-dispatch`);
  }
}
