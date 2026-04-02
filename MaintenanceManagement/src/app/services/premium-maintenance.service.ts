import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PremiumService, PremiumMaintenanceRequest, CreatePremiumServiceRequest, UpdatePremiumServiceRequest, CreatePremiumMaintenanceRequestRequest, UpdatePremiumMaintenanceRequestRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PremiumMaintenanceService {
  private readonly servicesBase = `${environment.apiUrl}/premiumservices`;
  private readonly requestsBase = `${environment.apiUrl}/premiummaintenancerequests`;

  constructor(private http: HttpClient) {}

  // Premium Services (catalog)
  getAllServices(): Observable<PremiumService[]> { return this.http.get<PremiumService[]>(this.servicesBase); }
  getActiveServices(): Observable<PremiumService[]> { return this.http.get<PremiumService[]>(`${this.servicesBase}/active`); }
  getServiceById(id: string): Observable<PremiumService> { return this.http.get<PremiumService>(`${this.servicesBase}/${id}`); }
  createService(dto: CreatePremiumServiceRequest): Observable<PremiumService> { return this.http.post<PremiumService>(this.servicesBase, dto); }
  updateService(id: string, dto: UpdatePremiumServiceRequest): Observable<PremiumService> { return this.http.put<PremiumService>(`${this.servicesBase}/${id}`, dto); }
  deleteService(id: string): Observable<void> { return this.http.delete<void>(`${this.servicesBase}/${id}`); }

  // Premium Maintenance Requests
  getAllRequests(): Observable<PremiumMaintenanceRequest[]> { return this.http.get<PremiumMaintenanceRequest[]>(this.requestsBase); }
  getRequestsByClient(clientId: string): Observable<PremiumMaintenanceRequest[]> { return this.http.get<PremiumMaintenanceRequest[]>(`${this.requestsBase}/client/${clientId}`); }
  getRequestById(id: string): Observable<PremiumMaintenanceRequest> { return this.http.get<PremiumMaintenanceRequest>(`${this.requestsBase}/${id}`); }
  createRequest(dto: CreatePremiumMaintenanceRequestRequest): Observable<PremiumMaintenanceRequest> { return this.http.post<PremiumMaintenanceRequest>(this.requestsBase, dto); }
  updateRequest(id: string, dto: UpdatePremiumMaintenanceRequestRequest): Observable<PremiumMaintenanceRequest> { return this.http.put<PremiumMaintenanceRequest>(`${this.requestsBase}/${id}`, dto); }
  deleteRequest(id: string): Observable<void> { return this.http.delete<void>(`${this.requestsBase}/${id}`); }
}
