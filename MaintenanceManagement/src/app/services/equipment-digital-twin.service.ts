import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { EquipmentDigitalTwin, UpsertDigitalTwinRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class EquipmentDigitalTwinService {
  private readonly base = `${environment.apiUrl}/equipmentdigitaltwin`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<EquipmentDigitalTwin[]> {
    return this.http.get<EquipmentDigitalTwin[]>(this.base);
  }

  getByEquipment(equipmentId: string): Observable<EquipmentDigitalTwin> {
    return this.http.get<EquipmentDigitalTwin>(`${this.base}/equipment/${equipmentId}`);
  }

  upsert(dto: UpsertDigitalTwinRequest): Observable<EquipmentDigitalTwin> {
    return this.http.post<EquipmentDigitalTwin>(this.base, dto);
  }

  syncFromEquipment(equipmentId: string): Observable<EquipmentDigitalTwin> {
    return this.http.post<EquipmentDigitalTwin>(`${this.base}/sync/${equipmentId}`, {});
  }
}
