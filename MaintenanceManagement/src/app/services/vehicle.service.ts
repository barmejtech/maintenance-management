import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Vehicle } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class VehicleService {
  private readonly base = `${environment.apiUrl}/vehicles`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Vehicle[]> { return this.http.get<Vehicle[]>(this.base); }
  getById(id: string): Observable<Vehicle> { return this.http.get<Vehicle>(`${this.base}/${id}`); }
  getDueService(): Observable<Vehicle[]> { return this.http.get<Vehicle[]>(`${this.base}/due-service`); }
  create(dto: any): Observable<Vehicle> { return this.http.post<Vehicle>(this.base, dto); }
  update(id: string, dto: any): Observable<Vehicle> { return this.http.put<Vehicle>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
