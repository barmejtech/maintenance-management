import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Renovation, CreateRenovationRequest, UpdateRenovationRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class RenovationService {
  private readonly base = `${environment.apiUrl}/renovations`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Renovation[]> { return this.http.get<Renovation[]>(this.base); }
  getById(id: string): Observable<Renovation> { return this.http.get<Renovation>(`${this.base}/${id}`); }
  getByUnit(unitId: string): Observable<Renovation[]> { return this.http.get<Renovation[]>(`${this.base}/unit/${unitId}`); }
  create(dto: CreateRenovationRequest): Observable<Renovation> { return this.http.post<Renovation>(this.base, dto); }
  update(id: string, dto: UpdateRenovationRequest): Observable<Renovation> { return this.http.put<Renovation>(`${this.base}/${id}`, dto); }
  approve(id: string): Observable<Renovation> { return this.http.post<Renovation>(`${this.base}/${id}/approve`, {}); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
