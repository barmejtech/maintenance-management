import { Component, OnInit, OnDestroy, signal, ViewChild, ElementRef, AfterViewChecked } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { ChatService } from '../../services/chat.service';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="page-container">
      <div class="page-header">
        <h2>💬 Team Chat</h2>
        <a routerLink="/dashboard" class="btn-back">← Dashboard</a>
      </div>

      <div class="chat-layout">
        <div class="chat-box">
          <div class="chat-status" [class.connected]="chatService.isConnected()">
            <span class="status-dot"></span>
            {{ chatService.isConnected() ? 'Connected' : 'Connecting...' }}
          </div>

          <div class="messages-area" #messagesContainer>
            @if (chatService.messages().length === 0) {
              <div class="no-messages">No messages yet. Say hello! 👋</div>
            }
            @for (msg of chatService.messages(); track msg.id) {
              <div class="message-row" [class.own]="msg.isOwn">
                <div class="message-bubble" [class.own]="msg.isOwn">
                  @if (!msg.isOwn) {
                    <div class="msg-sender">{{ msg.senderName }}</div>
                  }
                  <div class="msg-content">{{ msg.content }}</div>
                  <div class="msg-time">{{ msg.sentAt | date:'shortTime' }}</div>
                </div>
              </div>
            }
          </div>

          <div class="chat-input-area">
            <input
              #messageInput
              type="text"
              [(ngModel)]="newMessage"
              (keydown.enter)="sendMessage()"
              placeholder="Type a message..."
              class="chat-input"
              [disabled]="!chatService.isConnected()"
            />
            <button
              (click)="sendMessage()"
              class="btn-send"
              [disabled]="!newMessage.trim() || !chatService.isConnected()"
            >
              Send ➤
            </button>
          </div>
        </div>
      </div>
    </div>
  `,
  styles: [`
    .page-container { padding: 2rem; height: calc(100vh - 64px); display: flex; flex-direction: column; box-sizing: border-box; }
    .page-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; flex-shrink: 0; }
    .page-header h2 { font-size: 1.5rem; color: #333; }
    .btn-back { color: #0f3460; text-decoration: none; font-weight: 500; }
    .chat-layout { flex: 1; display: flex; min-height: 0; }
    .chat-box {
      flex: 1;
      background: white;
      border-radius: 0.75rem;
      box-shadow: 0 2px 8px rgba(0,0,0,0.06);
      display: flex;
      flex-direction: column;
      overflow: hidden;
    }
    .chat-status {
      display: flex;
      align-items: center;
      gap: 0.5rem;
      padding: 0.6rem 1rem;
      font-size: 0.8rem;
      color: #888;
      border-bottom: 1px solid #f0f0f0;
      background: #fafafa;
    }
    .chat-status.connected { color: #27ae60; }
    .status-dot {
      width: 8px;
      height: 8px;
      border-radius: 50%;
      background: #ccc;
      flex-shrink: 0;
    }
    .chat-status.connected .status-dot { background: #27ae60; }
    .messages-area {
      flex: 1;
      overflow-y: auto;
      padding: 1rem;
      display: flex;
      flex-direction: column;
      gap: 0.5rem;
    }
    .no-messages { text-align: center; color: #aaa; padding: 2rem; font-size: 0.9rem; }
    .message-row { display: flex; }
    .message-row.own { justify-content: flex-end; }
    .message-bubble {
      max-width: 65%;
      background: #f0f0f0;
      border-radius: 1rem;
      padding: 0.5rem 0.875rem;
      position: relative;
    }
    .message-bubble.own { background: #0f3460; color: white; }
    .msg-sender { font-size: 0.72rem; font-weight: 600; color: #888; margin-bottom: 0.2rem; }
    .msg-content { font-size: 0.9rem; word-break: break-word; line-height: 1.4; }
    .msg-time { font-size: 0.7rem; color: #aaa; margin-top: 0.25rem; text-align: right; }
    .message-bubble.own .msg-time { color: rgba(255,255,255,0.6); }
    .chat-input-area {
      display: flex;
      gap: 0.75rem;
      padding: 0.875rem 1rem;
      border-top: 1px solid #f0f0f0;
      background: white;
      flex-shrink: 0;
    }
    .chat-input {
      flex: 1;
      border: 2px solid #e0e0e0;
      border-radius: 2rem;
      padding: 0.5rem 1rem;
      font-size: 0.9rem;
      outline: none;
      transition: border-color 0.2s;
    }
    .chat-input:focus { border-color: #0f3460; }
    .chat-input:disabled { background: #f8f8f8; cursor: not-allowed; }
    .btn-send {
      background: #0f3460;
      color: white;
      border: none;
      border-radius: 2rem;
      padding: 0.5rem 1.25rem;
      font-size: 0.85rem;
      font-weight: 600;
      cursor: pointer;
      transition: background 0.2s;
      white-space: nowrap;
    }
    .btn-send:hover:not(:disabled) { background: #1a4a7a; }
    .btn-send:disabled { background: #ccc; cursor: not-allowed; }
  `]
})
export class ChatComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild('messagesContainer') private messagesContainer!: ElementRef;
  newMessage = '';
  private shouldScrollToBottom = false;

  constructor(
    public chatService: ChatService,
    private auth: AuthService
  ) {}

  ngOnInit() {
    const token = this.auth.getAccessToken() ?? '';
    const userId = this.auth.currentUser()?.userId ?? '';
    this.chatService.startConnection(token, userId);
    this.chatService.loadHistory().subscribe();
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

  sendMessage(): void {
    const content = this.newMessage.trim();
    if (!content) return;
    this.newMessage = '';
    this.chatService.sendViaHub(content);
    this.shouldScrollToBottom = true;
  }

  private scrollToBottom(): void {
    try {
      const el = this.messagesContainer.nativeElement;
      el.scrollTop = el.scrollHeight;
    } catch {}
  }
}
