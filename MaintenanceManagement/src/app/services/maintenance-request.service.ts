import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MaintenanceRequest, MaintenanceRequestStatus, SubmitMaintenanceRequestRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MaintenanceRequestService {
  private readonly base = `${environment.apiUrl}/maintenancerequests`;
  constructor(private http: HttpClient) {}

  getAll(filters?: { status?: MaintenanceRequestStatus; search?: string; from?: string; to?: string }): Observable<MaintenanceRequest[]> {
    let params = new HttpParams();
    if (filters?.status !== undefined && filters.status !== null) params = params.set('status', filters.status.toString());
    if (filters?.search) params = params.set('search', filters.search);
    if (filters?.from) params = params.set('from', filters.from);
    if (filters?.to) params = params.set('to', filters.to);
    return this.http.get<MaintenanceRequest[]>(this.base, { params });
  }

  getById(id: string): Observable<MaintenanceRequest> { return this.http.get<MaintenanceRequest>(`${this.base}/${id}`); }
  getByClient(clientId: string): Observable<MaintenanceRequest[]> { return this.http.get<MaintenanceRequest[]>(`${this.base}/client/${clientId}`); }
  getByStatus(status: MaintenanceRequestStatus): Observable<MaintenanceRequest[]> { return this.http.get<MaintenanceRequest[]>(`${this.base}/status/${status}`); }

  /** Returns the current client's own maintenance requests (uses JWT to identify the client). */
  getMyRequests(): Observable<MaintenanceRequest[]> { return this.http.get<MaintenanceRequest[]>(`${this.base}/my`); }

  create(dto: any): Observable<MaintenanceRequest> { return this.http.post<MaintenanceRequest>(this.base, dto); }

  /** Clients submit their own requests — clientId is resolved from the JWT server-side. */
  submitRequest(dto: SubmitMaintenanceRequestRequest): Observable<MaintenanceRequest> {
    return this.http.post<MaintenanceRequest>(`${this.base}/submit`, dto);
  }

  /** Admin/Manager: approve a request. */
  approve(id: string, reviewNotes?: string): Observable<MaintenanceRequest> {
    return this.http.put<MaintenanceRequest>(`${this.base}/${id}/approve`, { reviewNotes });
  }

  /** Admin/Manager: reject a request. */
  reject(id: string, reviewNotes?: string): Observable<MaintenanceRequest> {
    return this.http.put<MaintenanceRequest>(`${this.base}/${id}/reject`, { reviewNotes });
  }

  /** Admin/Manager: assign technicians to a request. */
  assignTechnicians(id: string, technicianIds: string[]): Observable<MaintenanceRequest> {
    return this.http.post<MaintenanceRequest>(`${this.base}/${id}/assign`, { technicianIds });
  }

  /** Get audit log for a request. */
  getAuditLog(id: string): Observable<any[]> {
    return this.http.get<any[]>(`${this.base}/${id}/audit`);
  }

  update(id: string, dto: any): Observable<MaintenanceRequest> { return this.http.put<MaintenanceRequest>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
