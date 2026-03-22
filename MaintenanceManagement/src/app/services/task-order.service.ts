import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TaskOrder, CreateTaskOrderRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class TaskOrderService {
  private readonly base = `${environment.apiUrl}/taskorders`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<TaskOrder[]> { return this.http.get<TaskOrder[]>(this.base); }
  getById(id: string): Observable<TaskOrder> { return this.http.get<TaskOrder>(`${this.base}/${id}`); }
  getByTechnician(id: string): Observable<TaskOrder[]> { return this.http.get<TaskOrder[]>(`${this.base}/technician/${id}`); }
  getByGroup(id: string): Observable<TaskOrder[]> { return this.http.get<TaskOrder[]>(`${this.base}/group/${id}`); }
  getCalendar(from: string, to: string): Observable<TaskOrder[]> {
    const params = new HttpParams().set('from', from).set('to', to);
    return this.http.get<TaskOrder[]>(`${this.base}/calendar`, { params });
  }
  create(dto: CreateTaskOrderRequest): Observable<TaskOrder> { return this.http.post<TaskOrder>(this.base, dto); }
  update(id: string, dto: CreateTaskOrderRequest): Observable<TaskOrder> { return this.http.put<TaskOrder>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
