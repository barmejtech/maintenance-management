import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { UnitTypeDto, CreateUnitTypeDto, UpdateUnitTypeDto } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class UnitTypeService {
  private readonly base = `${environment.apiUrl}/unittypes`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<UnitTypeDto[]> { return this.http.get<UnitTypeDto[]>(this.base); }
  getById(id: string): Observable<UnitTypeDto> { return this.http.get<UnitTypeDto>(`${this.base}/${id}`); }
  create(dto: CreateUnitTypeDto): Observable<UnitTypeDto> { return this.http.post<UnitTypeDto>(this.base, dto); }
  update(id: string, dto: UpdateUnitTypeDto): Observable<UnitTypeDto> { return this.http.put<UnitTypeDto>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
