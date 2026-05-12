import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Vendor, CreateVendorRequest, UpdateVendorRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class VendorService {
  private readonly base = `${environment.apiUrl}/vendors`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Vendor[]> { return this.http.get<Vendor[]>(this.base); }
  getActive(): Observable<Vendor[]> { return this.http.get<Vendor[]>(`${this.base}/active`); }
  getById(id: string): Observable<Vendor> { return this.http.get<Vendor>(`${this.base}/${id}`); }
  create(dto: CreateVendorRequest): Observable<Vendor> { return this.http.post<Vendor>(this.base, dto); }
  update(id: string, dto: UpdateVendorRequest): Observable<Vendor> { return this.http.put<Vendor>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
