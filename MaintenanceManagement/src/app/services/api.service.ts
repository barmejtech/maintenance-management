import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import {
  Technician, CreateTechnicianRequest, UpdateTechnicianRequest,
  TaskOrder, CreateTaskOrderRequest,
  TechnicianGroup, GroupDetail,
  Equipment,
  HVACSystem,
  MaintenanceReport, CreateReportRequest,
  Invoice,
  Availability, CreateAvailabilityRequest
} from '../models';
import { environment } from '../../environments/environment';

const API = environment.apiUrl;

@Injectable({ providedIn: 'root' })
export class TechnicianService {
  private readonly base = `${API}/technicians`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Technician[]> { return this.http.get<Technician[]>(this.base); }
  getById(id: string): Observable<Technician> { return this.http.get<Technician>(`${this.base}/${id}`); }
  getMe(): Observable<Technician> { return this.http.get<Technician>(`${this.base}/me`); }
  create(dto: CreateTechnicianRequest): Observable<Technician> { return this.http.post<Technician>(this.base, dto); }
  update(id: string, dto: UpdateTechnicianRequest): Observable<Technician> { return this.http.put<Technician>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
  updateLocation(id: string, lat: number, lng: number): Observable<void> {
    return this.http.patch<void>(`${this.base}/${id}/location`, { latitude: lat, longitude: lng });
  }
}

@Injectable({ providedIn: 'root' })
export class TaskOrderService {
  private readonly base = `${API}/taskorders`;
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

@Injectable({ providedIn: 'root' })
export class GroupService {
  private readonly base = `${API}/groups`;
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

@Injectable({ providedIn: 'root' })
export class EquipmentService {
  private readonly base = `${API}/equipment`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Equipment[]> { return this.http.get<Equipment[]>(this.base); }
  getById(id: string): Observable<Equipment> { return this.http.get<Equipment>(`${this.base}/${id}`); }
  getDueMaintenance(): Observable<Equipment[]> { return this.http.get<Equipment[]>(`${this.base}/due-maintenance`); }
  create(dto: any): Observable<Equipment> { return this.http.post<Equipment>(this.base, dto); }
  update(id: string, dto: any): Observable<Equipment> { return this.http.put<Equipment>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class HVACService {
  private readonly base = `${API}/hvac`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<HVACSystem[]> { return this.http.get<HVACSystem[]>(this.base); }
  getById(id: string): Observable<HVACSystem> { return this.http.get<HVACSystem>(`${this.base}/${id}`); }
  getDueInspection(): Observable<HVACSystem[]> { return this.http.get<HVACSystem[]>(`${this.base}/due-inspection`); }
  create(dto: any): Observable<HVACSystem> { return this.http.post<HVACSystem>(this.base, dto); }
  update(id: string, dto: any): Observable<HVACSystem> { return this.http.put<HVACSystem>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class ReportService {
  private readonly base = `${API}/reports`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<MaintenanceReport[]> { return this.http.get<MaintenanceReport[]>(this.base); }
  getById(id: string): Observable<MaintenanceReport> { return this.http.get<MaintenanceReport>(`${this.base}/${id}`); }
  getByTaskOrder(id: string): Observable<MaintenanceReport[]> { return this.http.get<MaintenanceReport[]>(`${this.base}/task/${id}`); }
  create(dto: CreateReportRequest): Observable<MaintenanceReport> { return this.http.post<MaintenanceReport>(this.base, dto); }
  update(id: string, dto: CreateReportRequest): Observable<MaintenanceReport> { return this.http.put<MaintenanceReport>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly base = `${API}/invoices`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Invoice[]> { return this.http.get<Invoice[]>(this.base); }
  getById(id: string): Observable<Invoice> { return this.http.get<Invoice>(`${this.base}/${id}`); }
  create(dto: any): Observable<Invoice> { return this.http.post<Invoice>(this.base, dto); }
  update(id: string, dto: any): Observable<Invoice> { return this.http.put<Invoice>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class AvailabilityService {
  private readonly base = `${API}/availabilities`;
  constructor(private http: HttpClient) {}

  getAll(): Observable<Availability[]> { return this.http.get<Availability[]>(this.base); }
  getById(id: string): Observable<Availability> { return this.http.get<Availability>(`${this.base}/${id}`); }
  getByTechnician(id: string): Observable<Availability[]> { return this.http.get<Availability[]>(`${this.base}/technician/${id}`); }
  create(dto: CreateAvailabilityRequest): Observable<Availability> { return this.http.post<Availability>(this.base, dto); }
  update(id: string, dto: CreateAvailabilityRequest): Observable<Availability> { return this.http.put<Availability>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class FileUploadService {
  private readonly base = `${API}/files`;
  constructor(private http: HttpClient) {}

  upload(files: File[]): Observable<any[]> {
    const formData = new FormData();
    files.forEach(f => formData.append('files', f, f.name));
    return this.http.post<any[]>(`${this.base}/upload`, formData);
  }

  getUrl(fileName: string): string {
    return `${API}/files/${fileName}`;
  }

  delete(fileName: string): Observable<void> {
    return this.http.delete<void>(`${this.base}/${fileName}`);
  }
}

  constructor(private http: HttpClient) {}

  getAll(): Observable<Technician[]> { return this.http.get<Technician[]>(this.base); }
  getById(id: string): Observable<Technician> { return this.http.get<Technician>(`${this.base}/${id}`); }
  getMe(): Observable<Technician> { return this.http.get<Technician>(`${this.base}/me`); }
  create(dto: CreateTechnicianRequest): Observable<Technician> { return this.http.post<Technician>(this.base, dto); }
  update(id: string, dto: UpdateTechnicianRequest): Observable<Technician> { return this.http.put<Technician>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
  updateLocation(id: string, lat: number, lng: number): Observable<void> {
    return this.http.patch<void>(`${this.base}/${id}/location`, { latitude: lat, longitude: lng });
  }
}

@Injectable({ providedIn: 'root' })
export class TaskOrderService {
  private readonly base = 'http://localhost:5214/api/taskorders';
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

@Injectable({ providedIn: 'root' })
export class GroupService {
  private readonly base = 'http://localhost:5214/api/groups';
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

@Injectable({ providedIn: 'root' })
export class EquipmentService {
  private readonly base = 'http://localhost:5214/api/equipment';
  constructor(private http: HttpClient) {}

  getAll(): Observable<Equipment[]> { return this.http.get<Equipment[]>(this.base); }
  getById(id: string): Observable<Equipment> { return this.http.get<Equipment>(`${this.base}/${id}`); }
  getDueMaintenance(): Observable<Equipment[]> { return this.http.get<Equipment[]>(`${this.base}/due-maintenance`); }
  create(dto: any): Observable<Equipment> { return this.http.post<Equipment>(this.base, dto); }
  update(id: string, dto: any): Observable<Equipment> { return this.http.put<Equipment>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class HVACService {
  private readonly base = 'http://localhost:5214/api/hvac';
  constructor(private http: HttpClient) {}

  getAll(): Observable<HVACSystem[]> { return this.http.get<HVACSystem[]>(this.base); }
  getById(id: string): Observable<HVACSystem> { return this.http.get<HVACSystem>(`${this.base}/${id}`); }
  getDueInspection(): Observable<HVACSystem[]> { return this.http.get<HVACSystem[]>(`${this.base}/due-inspection`); }
  create(dto: any): Observable<HVACSystem> { return this.http.post<HVACSystem>(this.base, dto); }
  update(id: string, dto: any): Observable<HVACSystem> { return this.http.put<HVACSystem>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class ReportService {
  private readonly base = 'http://localhost:5214/api/reports';
  constructor(private http: HttpClient) {}

  getAll(): Observable<MaintenanceReport[]> { return this.http.get<MaintenanceReport[]>(this.base); }
  getById(id: string): Observable<MaintenanceReport> { return this.http.get<MaintenanceReport>(`${this.base}/${id}`); }
  getByTaskOrder(id: string): Observable<MaintenanceReport[]> { return this.http.get<MaintenanceReport[]>(`${this.base}/task/${id}`); }
  create(dto: CreateReportRequest): Observable<MaintenanceReport> { return this.http.post<MaintenanceReport>(this.base, dto); }
  update(id: string, dto: CreateReportRequest): Observable<MaintenanceReport> { return this.http.put<MaintenanceReport>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class InvoiceService {
  private readonly base = 'http://localhost:5214/api/invoices';
  constructor(private http: HttpClient) {}

  getAll(): Observable<Invoice[]> { return this.http.get<Invoice[]>(this.base); }
  getById(id: string): Observable<Invoice> { return this.http.get<Invoice>(`${this.base}/${id}`); }
  create(dto: any): Observable<Invoice> { return this.http.post<Invoice>(this.base, dto); }
  update(id: string, dto: any): Observable<Invoice> { return this.http.put<Invoice>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}

@Injectable({ providedIn: 'root' })
export class AvailabilityService {
  private readonly base = 'http://localhost:5214/api/availabilities';
  constructor(private http: HttpClient) {}

  getAll(): Observable<Availability[]> { return this.http.get<Availability[]>(this.base); }
  getById(id: string): Observable<Availability> { return this.http.get<Availability>(`${this.base}/${id}`); }
  getByTechnician(id: string): Observable<Availability[]> { return this.http.get<Availability[]>(`${this.base}/technician/${id}`); }
  create(dto: CreateAvailabilityRequest): Observable<Availability> { return this.http.post<Availability>(this.base, dto); }
  update(id: string, dto: CreateAvailabilityRequest): Observable<Availability> { return this.http.put<Availability>(`${this.base}/${id}`, dto); }
  delete(id: string): Observable<void> { return this.http.delete<void>(`${this.base}/${id}`); }
}
