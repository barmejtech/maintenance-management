import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TenantDto, CreateTenantDto, UpdateTenantDto } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TenantService {
  private readonly base = `${environment.apiUrl}/tenants`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<TenantDto[]> { return this.http.get<TenantDto[]>(this.base); }
  getById(id: string): Observable<TenantDto> { return this.http.get<TenantDto>(`${this.base}/${id}`); }
  create(dto: CreateTenantDto): Observable<TenantDto> { return this.http.post<TenantDto>(this.base, dto); }
  update(id: string, dto: UpdateTenantDto): Observable<TenantDto> { return this.http.put<TenantDto>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
