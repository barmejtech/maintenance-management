import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UnitDto, CreateUnitDto, UpdateUnitDto, UnitMassUpdateDto } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class UnitService {
  private readonly base = `${environment.apiUrl}/units`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<UnitDto[]> { return this.http.get<UnitDto[]>(this.base); }
  getById(id: string): Observable<UnitDto> { return this.http.get<UnitDto>(`${this.base}/${id}`); }
  create(dto: CreateUnitDto): Observable<UnitDto> { return this.http.post<UnitDto>(this.base, dto); }
  update(id: string, dto: UpdateUnitDto): Observable<UnitDto> { return this.http.put<UnitDto>(`${this.base}/${id}`, dto); }
  massUpdate(dto: UnitMassUpdateDto): Observable<void> { return this.http.put<void>(`${this.base}/mass-update`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
