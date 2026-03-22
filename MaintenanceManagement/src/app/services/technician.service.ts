import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Technician, CreateTechnicianRequest, UpdateTechnicianRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TechnicianService {
  private readonly base = `${environment.apiUrl}/technicians`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Technician[]> { return this.http.get<Technician[]>(this.base); }
  getById(id: string): Observable<Technician> { return this.http.get<Technician>(`${this.base}/${id}`); }
  getMe(): Observable<Technician> { return this.http.get<Technician>(`${this.base}/me`); }
  create(dto: CreateTechnicianRequest): Observable<Technician> { return this.http.post<Technician>(this.base, dto); }
  update(id: string, dto: UpdateTechnicianRequest): Observable<Technician> { return this.http.put<Technician>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
  updateLocation(id: string, lat: number, lng: number): Observable<void> {
    return this.http.patch<void>(`${this.base}/${id}/location`, { latitude: lat, longitude: lng });
  }
}
