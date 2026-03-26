import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { DataEntry, CreateDataEntryRequest, UpdateDataEntryRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class DataEntryService {
  private readonly base = `${environment.apiUrl}/dataentry`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<DataEntry[]> { return this.http.get<DataEntry[]>(this.base); }
  getById(id: string): Observable<DataEntry> { return this.http.get<DataEntry>(`${this.base}/${id}`); }
  create(dto: CreateDataEntryRequest): Observable<DataEntry> { return this.http.post<DataEntry>(this.base, dto); }
  update(id: string, dto: UpdateDataEntryRequest): Observable<DataEntry> { return this.http.put<DataEntry>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
