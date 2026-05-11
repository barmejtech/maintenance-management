import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OwnerDto, CreateOwnerDto, UpdateOwnerDto, TransferOwnershipDto } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OwnerService {
  private readonly base = `${environment.apiUrl}/owners`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<OwnerDto[]> { return this.http.get<OwnerDto[]>(this.base); }
  getById(id: string): Observable<OwnerDto> { return this.http.get<OwnerDto>(`${this.base}/${id}`); }
  create(dto: CreateOwnerDto): Observable<OwnerDto> { return this.http.post<OwnerDto>(this.base, dto); }
  update(id: string, dto: UpdateOwnerDto): Observable<OwnerDto> { return this.http.put<OwnerDto>(`${this.base}/${id}`, dto); }
  transferOwnership(id: string, dto: TransferOwnershipDto): Observable<void> { return this.http.post<void>(`${this.base}/${id}/transfers`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
