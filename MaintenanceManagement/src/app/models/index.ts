// Auth models
export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: string;
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
  profilePhotoUrl?: string;
  roles: string[];
}

export interface RefreshTokenRequest {
  accessToken: string;
  refreshToken: string;
}

// User models (all users with roles)
export interface User {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  fullName: string;
  profilePhotoUrl?: string;
  isActive: boolean;
  createdAt: string;
  roles: string[];
}

// Manager models
export interface Manager {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string;
  department: string;
  profilePhotoUrl?: string;
  createdAt: string;
}

export interface CreateManagerRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  department: string;
  password: string;
}

export interface UpdateManagerRequest {
  firstName: string;
  lastName: string;
  phone: string;
  department: string;
  profilePhotoUrl?: string;
}

// DataEntry models
export interface DataEntry {
  id: string;
  userId: string;
  firstName: string;
  lastName: string;
  fullName: string;
  email: string;
  phone: string;
  section: string;
  profilePhotoUrl?: string;
  createdAt: string;
}

export interface CreateDataEntryRequest {
  firstName: string;
  lastName: string;
  email: string;
  phone: string;
  section: string;
  password: string;
}

export interface UpdateDataEntryRequest {
  firstName: string;
  lastName: string;
  phone: string;
  section: string;
  profilePhotoUrl?: string;
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
  profilePhotoUrl?: string;
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
  arrivalLatitude?: number;
  arrivalLongitude?: number;
  arrivalTime?: string;
  proofPhotoUrl?: string;
  customerSignatureUrl?: string;
  isGpsValidated: boolean;
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
  beforeMaintenancePhotoUrl?: string;
  afterMaintenancePhotoUrl?: string;
  photo3Url?: string;
  photo4Url?: string;
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
  clientId?: string;
  clientName: string;
  clientEmail?: string;
  clientAddress?: string;
  clientCompany?: string;
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

// Notification models
export interface AppNotification {
  id: string;
  title: string;
  message: string;
  type: NotificationType;
  isRead: boolean;
  createdAt: string;
  relatedEntityId?: string;
  relatedEntityType?: string;
}

export enum NotificationType {
  Info = 0,
  Success = 1,
  Warning = 2,
  Error = 3
}

// Chat models
export enum MessageType {
  Text = 0,
  File = 1,
  Photo = 2
}

export interface ChatMessage {
  id: string;
  senderId: string;
  senderName: string;
  receiverId?: string;
  content?: string;
  messageType: MessageType;
  fileUrl?: string;
  fileName?: string;
  contentType?: string;
  sentAt: string;
  isOwn?: boolean;
}

export interface SendMessageRequest {
  content?: string;
  messageType?: MessageType;
  fileUrl?: string;
  fileName?: string;
  contentType?: string;
  receiverId?: string;
}

export interface ChatUser {
  id: string;
  fullName: string;
  email: string;
  isOnline: boolean;
}

// Predictive Maintenance models
export interface EquipmentHealthPrediction {
  id: string;
  equipmentId: string;
  equipmentName: string;
  predictedFailureDate: string;
  failureProbability: number;
  recommendation: string;
  totalInterventions: number;
  averageDaysBetweenFailures: number;
  averageDaysBetweenMaintenance: number;
  lastAnalyzedAt?: string;
  createdAt: string;
}

// Digital Twin models
export interface EquipmentDigitalTwin {
  id: string;
  equipmentId: string;
  equipmentName: string;
  equipmentLocation: string;
  currentStatus: EquipmentStatus;
  wearPercentage: number;
  performanceScore: number;
  temperatureCelsius?: number;
  usageHours?: number;
  lastKnownIssue?: string;
  lastSyncedAt?: string;
  simulationNotes?: string;
  createdAt: string;
}

export interface UpsertDigitalTwinRequest {
  equipmentId: string;
  currentStatus: EquipmentStatus;
  wearPercentage: number;
  performanceScore: number;
  temperatureCelsius?: number;
  usageHours?: number;
  lastKnownIssue?: string;
  simulationNotes?: string;
}

// Performance models
export interface TechnicianPerformanceScore {
  id: string;
  technicianId: string;
  technicianName: string;
  averageInterventionTimeMinutes: number;
  successRate: number;
  customerSatisfactionScore: number;
  onTimeRate: number;
  totalTasksCompleted: number;
  totalTasksDelayed: number;
  lastCalculatedAt: string;
  createdAt: string;
}

export interface SmartDispatchResult {
  technicianId: string;
  technicianName: string;
  specialization: string;
  latitude?: number;
  longitude?: number;
  distanceScore: number;
  performanceScore: number;
  overallScore: number;
  assignmentReason: string;
}

export interface SubmitInterventionProofRequest {
  arrivalLatitude?: number;
  arrivalLongitude?: number;
  proofPhotoUrl?: string;
  customerSignatureUrl?: string;
}

// Spare Part models
export interface SparePart {
  id: string;
  name: string;
  partNumber: string;
  description: string;
  unit: string;
  quantityInStock: number;
  minimumStockLevel: number;
  unitPrice: number;
  supplier?: string;
  storageLocation?: string;
  notes?: string;
  photo1Url?: string;
  photo2Url?: string;
  photo3Url?: string;
  photo4Url?: string;
  isLowStock: boolean;
  createdAt: string;
}

export interface CreateSparePartRequest {
  name: string;
  partNumber: string;
  description: string;
  unit: string;
  quantityInStock: number;
  minimumStockLevel: number;
  unitPrice: number;
  supplier?: string;
  storageLocation?: string;
  notes?: string;
  photo1Url?: string;
  photo2Url?: string;
  photo3Url?: string;
  photo4Url?: string;
}

export interface UpdateSparePartRequest {
  name: string;
  partNumber: string;
  description: string;
  unit: string;
  quantityInStock: number;
  minimumStockLevel: number;
  unitPrice: number;
  supplier?: string;
  storageLocation?: string;
  notes?: string;
  photo1Url?: string;
  photo2Url?: string;
  photo3Url?: string;
  photo4Url?: string;
}

export interface SparePartUsage {
  id: string;
  sparePartId: string;
  sparePartName: string;
  taskOrderId?: string;
  taskOrderTitle?: string;
  quantityUsed: number;
  notes?: string;
  usedAt: string;
  usedByUserId: string;
  createdAt: string;
}

export interface CreateSparePartUsageRequest {
  sparePartId: string;
  taskOrderId?: string;
  quantityUsed: number;
  notes?: string;
}

// Maintenance Schedule models
export enum ScheduleFrequency {
  Daily = 0,
  Weekly = 1,
  BiWeekly = 2,
  Monthly = 3,
  Quarterly = 4,
  SemiAnnual = 5,
  Annual = 6
}

export interface MaintenanceSchedule {
  id: string;
  name: string;
  description: string;
  maintenanceType: MaintenanceType;
  frequency: ScheduleFrequency;
  frequencyValue: number;
  lastExecutedAt?: string;
  nextDueAt?: string;
  isActive: boolean;
  notes?: string;
  createdByUserId: string;
  equipmentId?: string;
  equipmentName?: string;
  assignedTechnicianId?: string;
  assignedTechnicianName?: string;
  assignedGroupId?: string;
  assignedGroupName?: string;
  createdAt: string;
}

export interface CreateMaintenanceScheduleRequest {
  name: string;
  description: string;
  maintenanceType: MaintenanceType;
  frequency: ScheduleFrequency;
  frequencyValue: number;
  nextDueAt?: string;
  isActive: boolean;
  notes?: string;
  equipmentId?: string;
  assignedTechnicianId?: string;
  assignedGroupId?: string;
}

export interface UpdateMaintenanceScheduleRequest {
  name: string;
  description: string;
  maintenanceType: MaintenanceType;
  frequency: ScheduleFrequency;
  frequencyValue: number;
  lastExecutedAt?: string;
  nextDueAt?: string;
  isActive: boolean;
  notes?: string;
  equipmentId?: string;
  assignedTechnicianId?: string;
  assignedGroupId?: string;
}


// Account models
export interface AccountProfile {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  phoneNumber?: string;
  profilePhotoUrl?: string;
  createdAt: string;
  roles: string[];
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  profilePhotoUrl?: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

// Client models
export interface Client {
  id: string;
  name: string;
  companyName?: string;
  email: string;
  phone?: string;
  address?: string;
  notes?: string;
  maintenanceRequestCount: number;
  createdAt: string;
}

export interface CreateClientRequest {
  name: string;
  companyName?: string;
  email: string;
  phone?: string;
  address?: string;
  notes?: string;
}

export interface UpdateClientRequest {
  name: string;
  companyName?: string;
  email: string;
  phone?: string;
  address?: string;
  notes?: string;
}

// Maintenance Request models
export interface MaintenanceRequest {
  id: string;
  title: string;
  description?: string;
  equipmentDescription?: string;
  requestDate: string;
  status: MaintenanceRequestStatus;
  notes?: string;
  clientId: string;
  clientName: string;
  taskOrderId?: string;
  taskTitle?: string;
  invoiceId?: string;
  invoiceNumber?: string;
  createdAt: string;
}

export interface CreateMaintenanceRequestRequest {
  title: string;
  description?: string;
  equipmentDescription?: string;
  requestDate?: string;
  clientId: string;
  notes?: string;
}

export interface UpdateMaintenanceRequestRequest {
  title: string;
  description?: string;
  equipmentDescription?: string;
  requestDate?: string;
  status: MaintenanceRequestStatus;
  notes?: string;
  taskOrderId?: string;
  invoiceId?: string;
}

export enum MaintenanceRequestStatus {
  Pending = 0,
  UnderReview = 1,
  Approved = 2,
  InProgress = 3,
  Completed = 4,
  Rejected = 5,
  Cancelled = 6
}
