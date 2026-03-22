import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { HVACSystem } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class HVACService {
  private readonly base = `${environment.apiUrl}/hvac`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<HVACSystem[]> { return this.http.get<HVACSystem[]>(this.base); }
  getById(id: string): Observable<HVACSystem> { return this.http.get<HVACSystem>(`${this.base}/${id}`); }
  getDueInspection(): Observable<HVACSystem[]> { return this.http.get<HVACSystem[]>(`${this.base}/due-inspection`); }
  create(dto: any): Observable<HVACSystem> { return this.http.post<HVACSystem>(this.base, dto); }
  update(id: string, dto: any): Observable<HVACSystem> { return this.http.put<HVACSystem>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
