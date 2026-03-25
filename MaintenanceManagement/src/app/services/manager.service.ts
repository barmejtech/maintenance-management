import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Manager, CreateManagerRequest, UpdateManagerRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ManagerService {
  private readonly base = `${environment.apiUrl}/managers`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Manager[]> { return this.http.get<Manager[]>(this.base); }
  getById(id: string): Observable<Manager> { return this.http.get<Manager>(`${this.base}/${id}`); }
  create(dto: CreateManagerRequest): Observable<Manager> { return this.http.post<Manager>(this.base, dto); }
  update(id: string, dto: UpdateManagerRequest): Observable<Manager> { return this.http.put<Manager>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
