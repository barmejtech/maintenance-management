import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MaintenanceRequest, MaintenanceRequestStatus } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MaintenanceRequestService {
  private readonly base = `${environment.apiUrl}/maintenancerequests`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<MaintenanceRequest[]> { return this.http.get<MaintenanceRequest[]>(this.base); }
  getById(id: string): Observable<MaintenanceRequest> { return this.http.get<MaintenanceRequest>(`${this.base}/${id}`); }
  getByClient(clientId: string): Observable<MaintenanceRequest[]> { return this.http.get<MaintenanceRequest[]>(`${this.base}/client/${clientId}`); }
  getByStatus(status: MaintenanceRequestStatus): Observable<MaintenanceRequest[]> { return this.http.get<MaintenanceRequest[]>(`${this.base}/status/${status}`); }
  create(dto: any): Observable<MaintenanceRequest> { return this.http.post<MaintenanceRequest>(this.base, dto); }
  update(id: string, dto: any): Observable<MaintenanceRequest> { return this.http.put<MaintenanceRequest>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
