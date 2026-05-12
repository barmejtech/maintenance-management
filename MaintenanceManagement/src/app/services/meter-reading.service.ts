import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  MeterReading,
  CreateMeterReadingRequest,
  UpdateMeterReadingRequest,
  BulkMeterReadingRequest,
  BulkMeterReadingResult,
  MeterReadingChartData,
  MeterType
} from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class MeterReadingService {
  private readonly base = `${environment.apiUrl}/meter-readings`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<MeterReading[]> { return this.http.get<MeterReading[]>(this.base); }
  getById(id: string): Observable<MeterReading> { return this.http.get<MeterReading>(`${this.base}/${id}`); }
  getByUnit(unitId: string): Observable<MeterReading[]> { return this.http.get<MeterReading[]>(`${this.base}/unit/${unitId}`); }

  getChartData(unitId: string, type: MeterType, months = 12): Observable<MeterReadingChartData[]> {
    const params = new HttpParams().set('type', type).set('months', months);
    return this.http.get<MeterReadingChartData[]>(`${this.base}/unit/${unitId}/chart`, { params });
  }

  create(dto: CreateMeterReadingRequest): Observable<MeterReading> { return this.http.post<MeterReading>(this.base, dto); }
  bulkCreate(dto: BulkMeterReadingRequest): Observable<BulkMeterReadingResult> { return this.http.post<BulkMeterReadingResult>(`${this.base}/bulk`, dto); }
  update(id: string, dto: UpdateMeterReadingRequest): Observable<MeterReading> { return this.http.put<MeterReading>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
