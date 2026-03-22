import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Availability, CreateAvailabilityRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AvailabilityService {
  private readonly base = `${environment.apiUrl}/availabilities`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Availability[]> { return this.http.get<Availability[]>(this.base); }
  getById(id: string): Observable<Availability> { return this.http.get<Availability>(`${this.base}/${id}`); }
  getByTechnician(id: string): Observable<Availability[]> { return this.http.get<Availability[]>(`${this.base}/technician/${id}`); }
  create(dto: CreateAvailabilityRequest): Observable<Availability> { return this.http.post<Availability>(this.base, dto); }
  update(id: string, dto: CreateAvailabilityRequest): Observable<Availability> { return this.http.put<Availability>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
