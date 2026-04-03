import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { TechnicianGpsLog, TechnicianDistance } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class GpsTrackingService {
  private readonly base = `${environment.apiUrl}/gps-tracking`;

  constructor(private http: HttpClient) {}

  updateLocation(technicianId: string, latitude: number, longitude: number): Observable<any> {
    return this.http.post<any>(`${this.base}/${technicianId}/update-location`, { latitude, longitude });
  }

  getHistory(technicianId: string): Observable<TechnicianGpsLog[]> {
    return this.http.get<TechnicianGpsLog[]>(`${this.base}/${technicianId}/history`);
  }

  getLatest(technicianId: string): Observable<TechnicianGpsLog> {
    return this.http.get<TechnicianGpsLog>(`${this.base}/${technicianId}/latest`);
  }

  getDistance(technicianId: string, serviceLatitude: number, serviceLongitude: number): Observable<TechnicianDistance> {
    const params = new HttpParams()
      .set('serviceLatitude', serviceLatitude.toString())
      .set('serviceLongitude', serviceLongitude.toString());
    return this.http.get<TechnicianDistance>(`${this.base}/${technicianId}/distance`, { params });
  }
}
