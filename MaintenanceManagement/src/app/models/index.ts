// Auth models
export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  userId: string;
  email: string;
  firstName: string;
  lastName: string;
  roles: string[];
}

export interface RefreshTokenRequest {
  accessToken: string;
  refreshToken: string;
}

// Technician models
export interface Technician {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string;
  specialization: string;
  profilePhotoUrl?: string;
  status: TechnicianStatus;
  latitude?: number;
  longitude?: number;
  lastLocationUpdate?: string;
  createdAt: string;
}

export interface CreateTechnicianRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  specialization: string;
  password: string;
}

export interface UpdateTechnicianRequest {
  firstName: string;
  lastName: string;
  phone: string;
  specialization: string;
  status: TechnicianStatus;
}

export enum TechnicianStatus {
  Available = 0,
  Busy = 1,
  OnLeave = 2,
  Inactive = 3
}

// Task Order models
export interface TaskOrder {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  maintenanceType: MaintenanceType;
  scheduledDate?: string;
  dueDate?: string;
  completedDate?: string;
  notes?: string;
  createdByUserId: string;
  technicianId?: string;
  technicianName?: string;
  groupId?: string;
  groupName?: string;
  equipmentId?: string;
  equipmentName?: string;
  createdAt: string;
}

export interface CreateTaskOrderRequest {
  title: string;
  description: string;
  status: TaskStatus;
  priority: TaskPriority;
  maintenanceType: MaintenanceType;
  scheduledDate?: string;
  dueDate?: string;
  notes?: string;
  technicianId?: string;
  groupId?: string;
  equipmentId?: string;
}

export enum TaskStatus {
  Pending = 0,
  InProgress = 1,
  Completed = 2,
  Cancelled = 3,
  OnHold = 4
}

export enum TaskPriority {
  Low = 0,
  Medium = 1,
  High = 2,
  Critical = 3
}

export enum MaintenanceType {
  Preventive = 0,
  Corrective = 1,
  Inspection = 2,
  Emergency = 3
}

// Group models
export interface TechnicianGroup {
  id: string;
  name: string;
  description: string;
  leaderUserId?: string;
  memberCount: number;
  createdAt: string;
}

export interface GroupDetail extends TechnicianGroup {
  members: GroupMember[];
}

export interface GroupMember {
  technicianId: string;
  fullName: string;
  specialization: string;
}

// Equipment models
export interface Equipment {
  id: string;
  name: string;
  serialNumber: string;
  model: string;
  manufacturer: string;
  location: string;
  installationDate?: string;
  lastMaintenanceDate?: string;
  nextMaintenanceDate?: string;
  status: EquipmentStatus;
  notes?: string;
  createdAt: string;
}

export enum EquipmentStatus {
  Operational = 0,
  UnderMaintenance = 1,
  OutOfService = 2,
  Decommissioned = 3
}

// HVAC models
export interface HVACSystem {
  id: string;
  name: string;
  systemType: string;
  brand: string;
  model: string;
  capacity: number;
  capacityUnit: string;
  refrigerantType: string;
  installationDate?: string;
  lastInspectionDate?: string;
  nextInspectionDate?: string;
  location: string;
  notes?: string;
  equipmentId?: string;
  createdAt: string;
}

// Report models
export interface MaintenanceReport {
  id: string;
  title: string;
  content: string;
  technicianName: string;
  createdByUserId: string;
  reportDate: string;
  beforePhotoUrl?: string;
  afterPhotoUrl?: string;
  pdfUrl?: string;
  laborHours?: number;
  materialCost?: number;
  recommendations?: string;
  taskOrderId?: string;
  taskTitle?: string;
  createdAt: string;
}

export interface CreateReportRequest {
  title: string;
  content: string;
  technicianName: string;
  reportDate: string;
  beforePhotoUrl?: string;
  afterPhotoUrl?: string;
  laborHours?: number;
  materialCost?: number;
  recommendations?: string;
  taskOrderId?: string;
}

// Invoice models
export interface Invoice {
  id: string;
  invoiceNumber: string;
  clientName: string;
  clientEmail?: string;
  clientAddress?: string;
  issueDate: string;
  dueDate?: string;
  paidDate?: string;
  subTotal: number;
  taxRate: number;
  taxAmount: number;
  totalAmount: number;
  status: InvoiceStatus;
  notes?: string;
  taskOrderId?: string;
  taskTitle?: string;
  lineItems: InvoiceLineItem[];
  createdAt: string;
}

export interface InvoiceLineItem {
  id: string;
  description: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

export enum InvoiceStatus {
  Draft = 0,
  Sent = 1,
  Paid = 2,
  Overdue = 3,
  Cancelled = 4
}

// Availability models
export interface Availability {
  id: string;
  technicianId: string;
  technicianName: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  notes?: string;
  createdAt: string;
}

export interface CreateAvailabilityRequest {
  technicianId: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
  notes?: string;
}
