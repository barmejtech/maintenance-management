import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MaintenanceReport, CreateReportRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReportService {
  private readonly base = `${environment.apiUrl}/reports`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<MaintenanceReport[]> { return this.http.get<MaintenanceReport[]>(this.base); }
  getById(id: string): Observable<MaintenanceReport> { return this.http.get<MaintenanceReport>(`${this.base}/${id}`); }
  getByTaskOrder(id: string): Observable<MaintenanceReport[]> { return this.http.get<MaintenanceReport[]>(`${this.base}/task/${id}`); }
  create(dto: CreateReportRequest): Observable<MaintenanceReport> { return this.http.post<MaintenanceReport>(this.base, dto); }
  update(id: string, dto: CreateReportRequest): Observable<MaintenanceReport> { return this.http.put<MaintenanceReport>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
