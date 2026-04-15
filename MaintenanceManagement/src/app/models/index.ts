// Auth models
export enum ClientType {
  Person = 0,
  Company = 1
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: string;
}

export interface ClientRegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  password: string;
  confirmPassword: string;
  clientType: ClientType;
  companyName?: string;
  phone?: string;
  address?: string;
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
  clientType?: ClientType;
  companyName?: string;
  clientRecordId?: string;
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
  qrCode?: string;
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
  maintenanceReportId?: string;
  maintenanceReportTitle?: string;
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
  qrCode?: string;
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
  reviewedByUserId?: string;
  reviewedAt?: string;
  reviewNotes?: string;
  assignedTechnicians: AssignedTechnician[];
}

export interface AssignedTechnician {
  technicianId: string;
  fullName: string;
  specialization: string;
  email: string;
  assignedAt: string;
}

export interface AuditLogEntry {
  id: string;
  action: string;
  performedByName: string;
  details?: string;
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

export interface SubmitMaintenanceRequestRequest {
  title: string;
  description?: string;
  equipmentDescription?: string;
  requestDate?: string;
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

// Premium Service models
export enum PremiumServiceType {
  Preventive = 0,
  Emergency = 1,
  Inspection = 2,
  FullOverhaul = 3,
  Corrective = 4,
  Consultation = 5
}

export enum PremiumMaintenanceStatus {
  Draft = 0,
  PaymentPending = 1,
  Paid = 2,
  InProgress = 3,
  Completed = 4,
  Cancelled = 5,
  Refunded = 6
}

export enum PaymentStatus {
  Pending = 0,
  Processing = 1,
  Completed = 2,
  Failed = 3,
  Refunded = 4,
  Cancelled = 5
}

export enum PaymentMethod {
  CreditCard = 0,
  DebitCard = 1,
  BankTransfer = 2,
  Cash = 3,
  OnlinePayment = 4
}

export interface PremiumService {
  id: string;
  name: string;
  description?: string;
  serviceType: PremiumServiceType;
  price: number;
  durationHours: number;
  priorityLevel: number;
  isActive: boolean;
  features?: string;
  createdAt: string;
}

export interface CreatePremiumServiceRequest {
  name: string;
  description?: string;
  serviceType: PremiumServiceType;
  price: number;
  durationHours: number;
  priorityLevel: number;
  isActive: boolean;
  features?: string;
}

export interface UpdatePremiumServiceRequest {
  name: string;
  description?: string;
  serviceType: PremiumServiceType;
  price: number;
  durationHours: number;
  priorityLevel: number;
  isActive: boolean;
  features?: string;
}

export interface PremiumMaintenanceRequest {
  id: string;
  clientId: string;
  clientName: string;
  premiumServiceId: string;
  serviceName: string;
  servicePrice: number;
  serviceType: PremiumServiceType;
  status: PremiumMaintenanceStatus;
  requestDate: string;
  scheduledDate?: string;
  notes?: string;
  address?: string;
  paymentStatus?: PaymentStatus;
  transactionId?: string;
  createdAt: string;
}

export interface CreatePremiumMaintenanceRequestRequest {
  clientId: string;
  premiumServiceId: string;
  scheduledDate?: string;
  notes?: string;
  address?: string;
}

export interface UpdatePremiumMaintenanceRequestRequest {
  status: PremiumMaintenanceStatus;
  scheduledDate?: string;
  notes?: string;
  address?: string;
}

export interface Payment {
  id: string;
  premiumMaintenanceRequestId: string;
  amount: number;
  status: PaymentStatus;
  paymentMethod: PaymentMethod;
  transactionId?: string;
  paymentDate?: string;
  notes?: string;
  failureReason?: string;
  createdAt: string;
}

export interface PaymentCheckout {
  paymentId: string;
  checkoutUrl: string;
  clientSecret: string;
  amount: number;
  currency: string;
}

// GPS Tracking models
export interface TechnicianGpsLog {
  id: string;
  technicianId: string;
  latitude: number;
  longitude: number;
  recordedAt: string;
}

export interface TechnicianDistance {
  technicianId: string;
  technicianName: string;
  technicianAddress?: string;
  technicianLatitude: number;
  technicianLongitude: number;
  serviceLatitude: number;
  serviceLongitude: number;
  distanceKm: number;
}

// Quotation models
export enum QuotationStatus {
  Draft = 0,
  Sent = 1,
  Accepted = 2,
  Rejected = 3,
  Expired = 4,
  Cancelled = 5
}

export interface Quotation {
  id: string;
  quotationNumber: string;
  clientName: string;
  clientEmail?: string;
  clientAddress?: string;
  clientPhone?: string;
  issueDate: string;
  validUntil: string;
  estimatedDurationDays: number;
  subTotal: number;
  taxRate: number;
  taxAmount: number;
  totalAmount: number;
  status: QuotationStatus;
  notes?: string;
  termsAndConditions?: string;
  maintenanceRequestId?: string;
  maintenanceRequestTitle?: string;
  clientId?: string;
  clientCompany?: string;
  lineItems: QuotationLineItem[];
  createdAt: string;
}

export interface QuotationLineItem {
  id: string;
  description: string;
  quantity: number;
  unitPrice: number;
  total: number;
}

export interface CreateQuotationRequest {
  clientName: string;
  clientEmail?: string;
  clientAddress?: string;
  clientPhone?: string;
  validUntil?: string;
  estimatedDurationDays: number;
  taxRate: number;
  notes?: string;
  termsAndConditions?: string;
  maintenanceRequestId?: string;
  clientId?: string;
  lineItems: CreateQuotationLineItemRequest[];
}

export interface CreateQuotationLineItemRequest {
  description: string;
  quantity: number;
  unitPrice: number;
}

export interface UpdateQuotationRequest extends CreateQuotationRequest {
  status: QuotationStatus;
}
