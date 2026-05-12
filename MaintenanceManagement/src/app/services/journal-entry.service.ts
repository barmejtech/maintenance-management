import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { JournalEntry, CreateJournalEntryRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class JournalEntryService {
  private readonly base = `${environment.apiUrl}/journal-entries`;

  constructor(private http: HttpClient) {}

  getAll(): Observable<JournalEntry[]> { return this.http.get<JournalEntry[]>(this.base); }
  getById(id: string): Observable<JournalEntry> { return this.http.get<JournalEntry>(`${this.base}/${id}`); }
  create(dto: CreateJournalEntryRequest): Observable<JournalEntry> { return this.http.post<JournalEntry>(this.base, dto); }
  update(id: string, dto: CreateJournalEntryRequest): Observable<JournalEntry> { return this.http.put<JournalEntry>(`${this.base}/${id}`, dto); }
  post(id: string): Observable<JournalEntry> { return this.http.post<JournalEntry>(`${this.base}/${id}/post`, {}); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
