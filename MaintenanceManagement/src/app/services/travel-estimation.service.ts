import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../environments/environment';

export interface TravelEstimationResult {
  distanceKm: number;
  durationMinutes: number;
  formattedDistance: string;
  formattedDuration: string;
  technicianName: string;
  clientAddress: string;
  technicianHasLocation: boolean;
}

export interface TravelEstimationResponse {
  success: boolean;
  message?: string;
  data: TravelEstimationResult;
}

@Injectable({ providedIn: 'root' })
export class TravelEstimationService {
  private readonly base = `${environment.apiUrl}/travelestimation`;

  constructor(private http: HttpClient) {}

  estimate(technicianId: string, clientId: string): Observable<TravelEstimationResponse> {
    const params = new HttpParams()
      .set('technicianId', technicianId)
      .set('clientId', clientId);
    return this.http.get<TravelEstimationResponse>(this.base, { params });
  }
}
