import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { SparePart, CreateSparePartRequest, UpdateSparePartRequest, SparePartUsage, CreateSparePartUsageRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class SparePartService {
  private readonly base = `${environment.apiUrl}/spareparts`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<SparePart[]> { return this.http.get<SparePart[]>(this.base); }
  getById(id: string): Observable<SparePart> { return this.http.get<SparePart>(`${this.base}/${id}`); }
  getLowStock(): Observable<SparePart[]> { return this.http.get<SparePart[]>(`${this.base}/low-stock`); }
  create(dto: CreateSparePartRequest): Observable<SparePart> { return this.http.post<SparePart>(this.base, dto); }
  update(id: string, dto: UpdateSparePartRequest): Observable<SparePart> { return this.http.put<SparePart>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }

  getUsages(sparePartId: string): Observable<SparePartUsage[]> { return this.http.get<SparePartUsage[]>(`${this.base}/${sparePartId}/usages`); }
  addUsage(dto: CreateSparePartUsageRequest): Observable<SparePartUsage> { return this.http.post<SparePartUsage>(`${this.base}/usages`, dto); }
  deleteUsage(usageId: string): Observable<void> { return this.http.delete<void>(`${this.base}/usages/${usageId}`); }
}
