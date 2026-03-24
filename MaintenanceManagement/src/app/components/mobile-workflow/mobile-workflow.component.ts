import { Component, OnInit, OnDestroy, signal, ViewChild, ElementRef, AfterViewInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { TaskOrderService } from '../../services/task-order.service';
import { TechnicianService } from '../../services/technician.service';
import { FileUploadService } from '../../services/file-upload.service';
import { AuthService } from '../../services/auth.service';
import { TaskOrder, TaskStatus } from '../../models';

export interface OfflineReport {
  id: string;
  taskId: string;
  taskTitle: string;
  notes: string;
  gpsLat?: number;
  gpsLng?: number;
  photos: string[];
  voiceNoteUrl?: string;
  createdAt: string;
  synced: boolean;
}

const OFFLINE_REPORTS_KEY = 'mw_offline_reports';

@Component({
  selector: 'app-mobile-workflow',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './mobile-workflow.component.html',
  styleUrls: ['./mobile-workflow.component.css']
})
export class MobileWorkflowComponent implements OnInit, OnDestroy, AfterViewInit {
  @ViewChild('signatureCanvas') signatureCanvasRef!: ElementRef<HTMLCanvasElement>;

  myTasks = signal<TaskOrder[]>([]);
  selectedTask = signal<TaskOrder | null>(null);
  isLoading = signal(true);

  // GPS Check-in
  gpsStatus = signal<'idle' | 'capturing' | 'success' | 'denied' | 'unavailable'>('idle');
  gpsCoords = signal<{ lat: number; lng: number } | null>(null);

  // Photo Upload
  uploadedPhotos = signal<string[]>([]);
  isUploadingPhoto = signal(false);

  // Digital Signature
  isDrawing = false;
  signatureDataUrl = signal<string>('');
  private ctx: CanvasRenderingContext2D | null = null;

  // Voice Notes
  isRecording = signal(false);
  voiceNoteUrl = signal<string>('');
  voiceNoteBlob: Blob | null = null;
  private mediaRecorder: MediaRecorder | null = null;
  private audioChunks: BlobPart[] = [];
  private mediaStream: MediaStream | null = null;

  // Offline Reports
  offlineReports = signal<OfflineReport[]>([]);
  workNotes = '';
  isSyncing = signal(false);
  activeTab = signal<'tasks' | 'offline'>('tasks');
  isSubmitting = signal(false);

  TaskStatus = TaskStatus;

  constructor(
    private taskService: TaskOrderService,
    private techService: TechnicianService,
    private fileUploadService: FileUploadService,
    private auth: AuthService
  ) {}

  ngOnInit() {
    this.loadMyTasks();
    this.loadOfflineReports();
  }

  ngAfterViewInit() { /* canvas initialized lazily when task selected */ }

  ngOnDestroy() {
    this.stopRecording();
    if (this.mediaStream) {
      this.mediaStream.getTracks().forEach(t => t.stop());
    }
  }

  private loadMyTasks() {
    const userId = this.auth.currentUser()?.userId;
    if (!userId) { this.isLoading.set(false); return; }
    // Load tasks for the current user by fetching all and filtering by technician
    this.taskService.getAll().subscribe({
      next: (tasks) => {
        const myTasks = tasks.filter(t =>
          t.status === TaskStatus.InProgress || t.status === TaskStatus.Pending
        );
        this.myTasks.set(myTasks);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  selectTask(task: TaskOrder) {
    this.selectedTask.set(task);
    this.gpsStatus.set('idle');
    this.gpsCoords.set(null);
    this.uploadedPhotos.set([]);
    this.signatureDataUrl.set('');
    this.voiceNoteUrl.set('');
    this.workNotes = task.notes ?? '';
    setTimeout(() => this.initCanvas(), 100);
  }

  // ===== GPS CHECK-IN =====
  checkIn() {
    if (!navigator.geolocation) {
      this.gpsStatus.set('unavailable');
      return;
    }
    this.gpsStatus.set('capturing');
    navigator.geolocation.getCurrentPosition(
      (pos) => {
        const coords = { lat: pos.coords.latitude, lng: pos.coords.longitude };
        this.gpsCoords.set(coords);
        this.gpsStatus.set('success');
        const task = this.selectedTask();
        if (task?.technicianId) {
          this.techService.updateLocation(task.technicianId, coords.lat, coords.lng).subscribe();
        }
      },
      () => this.gpsStatus.set('denied'),
      { timeout: 10000, maximumAge: 60000 }
    );
  }

  // ===== PHOTO UPLOAD =====
  onPhotoSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    const files = Array.from(input.files);
    this.isUploadingPhoto.set(true);
    this.fileUploadService.uploadPhoto(files).subscribe({
      next: (results) => {
        const urls = results.map((r: any) => r.url ?? r.fileName ?? '');
        this.uploadedPhotos.update(prev => [...prev, ...urls]);
        this.isUploadingPhoto.set(false);
      },
      error: () => this.isUploadingPhoto.set(false)
    });
  }

  removePhoto(index: number) {
    this.uploadedPhotos.update(prev => prev.filter((_, i) => i !== index));
  }

  // ===== DIGITAL SIGNATURE =====
  private initCanvas() {
    if (!this.signatureCanvasRef) return;
    const canvas = this.signatureCanvasRef.nativeElement;
    canvas.width = canvas.offsetWidth || 400;
    canvas.height = 160;
    this.ctx = canvas.getContext('2d');
    if (this.ctx) {
      this.ctx.strokeStyle = '#1a1a2e';
      this.ctx.lineWidth = 2;
      this.ctx.lineCap = 'round';
      this.ctx.lineJoin = 'round';
    }
  }

  onCanvasMouseDown(event: MouseEvent) {
    this.isDrawing = true;
    this.ctx?.beginPath();
    const rect = (event.target as HTMLCanvasElement).getBoundingClientRect();
    this.ctx?.moveTo(event.clientX - rect.left, event.clientY - rect.top);
  }

  onCanvasMouseMove(event: MouseEvent) {
    if (!this.isDrawing || !this.ctx) return;
    const rect = (event.target as HTMLCanvasElement).getBoundingClientRect();
    this.ctx.lineTo(event.clientX - rect.left, event.clientY - rect.top);
    this.ctx.stroke();
  }

  onCanvasTouchStart(event: TouchEvent) {
    event.preventDefault();
    this.isDrawing = true;
    this.ctx?.beginPath();
    const rect = (event.target as HTMLCanvasElement).getBoundingClientRect();
    const touch = event.touches[0];
    this.ctx?.moveTo(touch.clientX - rect.left, touch.clientY - rect.top);
  }

  onCanvasTouchMove(event: TouchEvent) {
    event.preventDefault();
    if (!this.isDrawing || !this.ctx) return;
    const rect = (event.target as HTMLCanvasElement).getBoundingClientRect();
    const touch = event.touches[0];
    this.ctx.lineTo(touch.clientX - rect.left, touch.clientY - rect.top);
    this.ctx.stroke();
  }

  stopDrawing() {
    if (this.isDrawing) {
      this.isDrawing = false;
      const canvas = this.signatureCanvasRef?.nativeElement;
      if (canvas) this.signatureDataUrl.set(canvas.toDataURL('image/png'));
    }
  }

  clearSignature() {
    const canvas = this.signatureCanvasRef?.nativeElement;
    if (canvas && this.ctx) {
      this.ctx.clearRect(0, 0, canvas.width, canvas.height);
      this.signatureDataUrl.set('');
    }
  }

  // ===== VOICE NOTES =====
  async startRecording() {
    try {
      this.mediaStream = await navigator.mediaDevices.getUserMedia({ audio: true });
      this.audioChunks = [];
      this.mediaRecorder = new MediaRecorder(this.mediaStream);
      this.mediaRecorder.ondataavailable = (e) => this.audioChunks.push(e.data);
      this.mediaRecorder.onstop = () => {
        this.voiceNoteBlob = new Blob(this.audioChunks, { type: 'audio/webm' });
        this.voiceNoteUrl.set(URL.createObjectURL(this.voiceNoteBlob));
      };
      this.mediaRecorder.start();
      this.isRecording.set(true);
    } catch {
      alert('Microphone access denied or not available.');
    }
  }

  stopRecording() {
    if (this.mediaRecorder && this.isRecording()) {
      this.mediaRecorder.stop();
      this.isRecording.set(false);
    }
  }

  clearVoiceNote() {
    this.voiceNoteUrl.set('');
    this.voiceNoteBlob = null;
  }

  // ===== OFFLINE REPORTS =====
  private loadOfflineReports() {
    try {
      const raw = localStorage.getItem(OFFLINE_REPORTS_KEY);
      this.offlineReports.set(raw ? JSON.parse(raw) : []);
    } catch {
      this.offlineReports.set([]);
    }
  }

  private saveOfflineReports() {
    localStorage.setItem(OFFLINE_REPORTS_KEY, JSON.stringify(this.offlineReports()));
  }

  saveOffline() {
    const task = this.selectedTask();
    if (!task) return;
    const id = typeof crypto !== 'undefined' && crypto.randomUUID
      ? crypto.randomUUID()
      : `${Date.now()}-${Math.random().toString(36).slice(2)}`;
    const report: OfflineReport = {
      id,
      taskId: task.id,
      taskTitle: task.title,
      notes: this.workNotes,
      gpsLat: this.gpsCoords()?.lat,
      gpsLng: this.gpsCoords()?.lng,
      photos: [...this.uploadedPhotos()],
      voiceNoteUrl: this.voiceNoteUrl() || undefined,
      createdAt: new Date().toISOString(),
      synced: false
    };
    this.offlineReports.update(prev => [report, ...prev]);
    this.saveOfflineReports();
    alert('Report saved offline. It will sync when connection is available.');
  }

  syncOfflineReports() {
    const pending = this.offlineReports().filter(r => !r.synced);
    if (!pending.length) { alert('No pending offline reports to sync.'); return; }
    this.isSyncing.set(true);
    let done = 0;
    pending.forEach(report => {
      const dto = {
        arrivalLatitude: report.gpsLat,
        arrivalLongitude: report.gpsLng,
        proofPhotoUrl: report.photos[0] ?? undefined,
        customerSignatureUrl: undefined
      };
      this.taskService.submitInterventionProof(report.taskId, dto).subscribe({
        next: () => {
          this.offlineReports.update(prev =>
            prev.map(r => r.id === report.id ? { ...r, synced: true } : r)
          );
          this.saveOfflineReports();
          done++;
          if (done === pending.length) this.isSyncing.set(false);
        },
        error: () => {
          done++;
          if (done === pending.length) this.isSyncing.set(false);
        }
      });
    });
  }

  deleteOfflineReport(id: string) {
    this.offlineReports.update(prev => prev.filter(r => r.id !== id));
    this.saveOfflineReports();
  }

  // ===== SUBMIT PROOF =====
  submitProof() {
    const task = this.selectedTask();
    if (!task) return;
    this.isSubmitting.set(true);
    const coords = this.gpsCoords();
    const dto = {
      arrivalLatitude: coords?.lat,
      arrivalLongitude: coords?.lng,
      proofPhotoUrl: this.uploadedPhotos()[0] ?? undefined,
      customerSignatureUrl: this.signatureDataUrl() || undefined
    };
    this.taskService.submitInterventionProof(task.id, dto).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        alert('Proof submitted successfully!');
        this.loadMyTasks();
        this.selectedTask.set(null);
      },
      error: () => this.isSubmitting.set(false)
    });
  }

  getStatusClass(s: TaskStatus): string {
    return ['bg-warning text-dark', 'bg-info text-dark', 'bg-success', 'bg-danger', 'bg-secondary'][s] ?? 'bg-secondary';
  }

  getStatusLabel(s: TaskStatus): string {
    return ['Pending', 'In Progress', 'Completed', 'Cancelled', 'On Hold'][s] ?? 'Unknown';
  }

  pendingOfflineCount = () => this.offlineReports().filter(r => !r.synced).length;
}
