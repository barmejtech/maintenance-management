import { Component, OnInit, OnDestroy, signal, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ChatService } from '../../services/chat.service';
import { FileUploadService } from '../../services/file-upload.service';
import { ChatUser, MessageType } from '../../models';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  @ViewChild('fileInput') private fileInput!: ElementRef<HTMLInputElement>;
  newMessage = '';
  isUploading = signal(false);
  MessageType = MessageType;
  private shouldScrollToBottom = false;
  private currentUserId = '';

  constructor(
    public chatService: ChatService,
    private auth: AuthService,
    private fileUploadService: FileUploadService
  ) {}

  ngOnInit() {
    const token = this.auth.getAccessToken() ?? '';
    this.currentUserId = this.auth.currentUser()?.userId ?? '';
    this.chatService.startConnection(token, this.currentUserId);
    this.chatService.loadHistory().subscribe();
    this.chatService.loadUsers();
    this.shouldScrollToBottom = true;
  }

  ngOnDestroy() {
    this.chatService.stopConnection();
  }

  ngAfterViewChecked() {
    if (this.shouldScrollToBottom) {
      this.scrollToBottom();
      this.shouldScrollToBottom = false;
    }
  }

  get selectedRecipient(): ChatUser | null {
    return this.chatService.selectedRecipient();
  }

  get channelLabel(): string {
    const r = this.selectedRecipient;
    return r ? `🔒 ${r.fullName}` : '🌐 Public Channel';
  }

  selectUser(user: ChatUser | null): void {
    this.chatService.selectRecipient(user);
    this.shouldScrollToBottom = true;
  }

  isOwnUser(userId: string): boolean {
    return userId === this.currentUserId;
  }

  sendMessage(): void {
    const content = this.newMessage.trim();
    if (!content) return;
    this.newMessage = '';
    const recipient = this.selectedRecipient;
    if (recipient) {
      this.chatService.sendPrivateViaHub(recipient.id, content);
    } else {
      this.chatService.sendViaHub(content);
    }
    this.shouldScrollToBottom = true;
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
    const file = input.files[0];
    this.isUploading.set(true);
    this.fileUploadService.upload([file]).subscribe({
      next: (results) => {
        const result = results[0];
        const isPhoto = file.type.startsWith('image/');
        const recipient = this.selectedRecipient;
        if (recipient) {
          this.chatService.sendPrivateFileViaHub(recipient.id, result.url, result.originalName, result.contentType, isPhoto);
        } else {
          this.chatService.sendFileViaHub(result.url, result.originalName, result.contentType, isPhoto);
        }
        this.shouldScrollToBottom = true;
        this.isUploading.set(false);
        if (this.fileInput) this.fileInput.nativeElement.value = '';
      },
      error: () => this.isUploading.set(false)
    });
  }

  isImage(contentType?: string): boolean {
    return !!contentType?.startsWith('image/');
  }

  getFileUrl(fileUrl?: string): string {
    if (!fileUrl) return '';
    if (fileUrl.startsWith('http')) return fileUrl;
    const base = (window as any).__env?.apiUrl?.replace('/api', '') ?? 'https://localhost:44352';
    return `${base}${fileUrl}`;
  }

  private scrollToBottom(): void {
    try {
      const el = this.messagesContainer.nativeElement;
      el.scrollTop = el.scrollHeight;
    } catch {}
  }
}
