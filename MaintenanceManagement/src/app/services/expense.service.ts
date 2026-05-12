import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Expense, CreateExpenseRequest, UpdateExpenseRequest, ExpenseStatus } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ExpenseService {
  private readonly base = `${environment.apiUrl}/expenses`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<Expense[]> { return this.http.get<Expense[]>(this.base); }
  getById(id: string): Observable<Expense> { return this.http.get<Expense>(`${this.base}/${id}`); }
  getByVendor(vendorId: string): Observable<Expense[]> { return this.http.get<Expense[]>(`${this.base}/vendor/${vendorId}`); }
  getByStatus(status: ExpenseStatus): Observable<Expense[]> { return this.http.get<Expense[]>(`${this.base}/status/${status}`); }
  create(dto: CreateExpenseRequest): Observable<Expense> { return this.http.post<Expense>(this.base, dto); }
  update(id: string, dto: UpdateExpenseRequest): Observable<Expense> { return this.http.put<Expense>(`${this.base}/${id}`, dto); }
  approve(id: string): Observable<Expense> { return this.http.post<Expense>(`${this.base}/${id}/approve`, {}); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
