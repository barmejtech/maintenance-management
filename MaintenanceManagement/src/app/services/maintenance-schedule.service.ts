import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { MaintenanceSchedule, CreateMaintenanceScheduleRequest, UpdateMaintenanceScheduleRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MaintenanceScheduleService {
  private readonly base = `${environment.apiUrl}/maintenanceschedules`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<MaintenanceSchedule[]> { return this.http.get<MaintenanceSchedule[]>(this.base); }
  getById(id: string): Observable<MaintenanceSchedule> { return this.http.get<MaintenanceSchedule>(`${this.base}/${id}`); }
  getByEquipment(equipmentId: string): Observable<MaintenanceSchedule[]> { return this.http.get<MaintenanceSchedule[]>(`${this.base}/equipment/${equipmentId}`); }
  getActive(): Observable<MaintenanceSchedule[]> { return this.http.get<MaintenanceSchedule[]>(`${this.base}/active`); }
  getOverdue(): Observable<MaintenanceSchedule[]> { return this.http.get<MaintenanceSchedule[]>(`${this.base}/overdue`); }
  getDueSoon(withinDays: number = 7): Observable<MaintenanceSchedule[]> {
    const params = new HttpParams().set('withinDays', withinDays.toString());
    return this.http.get<MaintenanceSchedule[]>(`${this.base}/due-soon`, { params });
  }
  create(dto: CreateMaintenanceScheduleRequest): Observable<MaintenanceSchedule> { return this.http.post<MaintenanceSchedule>(this.base, dto); }
  update(id: string, dto: UpdateMaintenanceScheduleRequest): Observable<MaintenanceSchedule> { return this.http.put<MaintenanceSchedule>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
