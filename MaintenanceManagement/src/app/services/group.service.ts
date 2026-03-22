import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TechnicianGroup, GroupDetail } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class GroupService {
  private readonly base = `${environment.apiUrl}/groups`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<TechnicianGroup[]> { return this.http.get<TechnicianGroup[]>(this.base); }
  getById(id: string): Observable<GroupDetail> { return this.http.get<GroupDetail>(`${this.base}/${id}`); }
  create(dto: any): Observable<TechnicianGroup> { return this.http.post<TechnicianGroup>(this.base, dto); }
  update(id: string, dto: any): Observable<TechnicianGroup> { return this.http.put<TechnicianGroup>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
  addMember(groupId: string, technicianId: string): Observable<void> {
    return this.http.post<void>(`${this.base}/${groupId}/members`, { technicianId });
  }
  removeMember(groupId: string, technicianId: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${groupId}/members/${technicianId}`);
  }
}
