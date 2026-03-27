import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Client, Invoice } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ClientService {
  private readonly base = `${environment.apiUrl}/clients`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Client[]> { return this.http.get<Client[]>(this.base); }
  getById(id: string): Observable<Client> { return this.http.get<Client>(`${this.base}/${id}`); }
  getInvoices(clientId: string): Observable<Invoice[]> { return this.http.get<Invoice[]>(`${this.base}/${clientId}/invoices`); }
  create(dto: any): Observable<Client> { return this.http.post<Client>(this.base, dto); }
  update(id: string, dto: any): Observable<Client> { return this.http.put<Client>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
