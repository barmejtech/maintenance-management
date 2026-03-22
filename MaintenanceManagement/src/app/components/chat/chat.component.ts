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
  templateUrl: './chat.component.html',
  styleUrls: ['./chat.component.css']
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
