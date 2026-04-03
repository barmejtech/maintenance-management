import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';
import { MaintenanceRequestStatus } from '../models';

export interface AdminDashboardRow {
  requestId: string;
  clientName: string;
  dateRequest: string;
  technicians: string;
  dateOfTask: string | null;
  status: MaintenanceRequestStatus;
  price: number | null;
  isPaid: boolean;
  datePay: string | null;
}

export interface DailyStat {
  date: string;
  count: number;
}

export interface AdminDashboardData {
  rows: AdminDashboardRow[];
  dailyStats: DailyStat[];
  weeklyAverage: number;
}

@Injectable({ providedIn: 'root' })
export class AdminDashboardService {
  private readonly base = `${environment.apiUrl}/admindashboard`;

  constructor(private http: HttpClient) {}

  getDashboard(): Observable<AdminDashboardData> {
    return this.http.get<AdminDashboardData>(this.base);
  }
}
