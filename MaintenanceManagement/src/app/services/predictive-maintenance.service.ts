import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EquipmentHealthPrediction } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PredictiveMaintenanceService {
  private readonly base = `${environment.apiUrl}/predictivemaintenance`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<EquipmentHealthPrediction[]> {
    return this.http.get<EquipmentHealthPrediction[]>(this.base);
  }

  getByEquipment(equipmentId: string): Observable<EquipmentHealthPrediction> {
    return this.http.get<EquipmentHealthPrediction>(`${this.base}/equipment/${equipmentId}`);
  }

  runPrediction(equipmentId: string): Observable<EquipmentHealthPrediction> {
    return this.http.post<EquipmentHealthPrediction>(`${this.base}/run/${equipmentId}`, {});
  }

  getHighRisk(threshold = 0.7): Observable<EquipmentHealthPrediction[]> {
    return this.http.get<EquipmentHealthPrediction[]>(`${this.base}/high-risk?threshold=${threshold}`);
  }
}
