import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Equipment } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EquipmentService {
  private readonly base = `${environment.apiUrl}/equipment`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Equipment[]> { return this.http.get<Equipment[]>(this.base); }
  getById(id: string): Observable<Equipment> { return this.http.get<Equipment>(`${this.base}/${id}`); }
  getDueMaintenance(): Observable<Equipment[]> { return this.http.get<Equipment[]>(`${this.base}/due-maintenance`); }
  create(dto: any): Observable<Equipment> { return this.http.post<Equipment>(this.base, dto); }
  update(id: string, dto: any): Observable<Equipment> { return this.http.put<Equipment>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
