import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';
import { ChatMessage, ChatUser, MessageType, SendMessageRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly base = `${environment.apiUrl}/chat`;
  private hubConnection: signalR.HubConnection | null = null;
  private currentUserId = '';

  private messagesSignal = signal<ChatMessage[]>([]);
  readonly messages = computed(() => this.messagesSignal());
  readonly isConnected = signal(false);
  readonly onlineUsers = signal<string[]>([]);
  readonly users = signal<ChatUser[]>([]);

  constructor(private http: HttpClient) {}

  startConnection(token: string, currentUserId: string): void {
    this.currentUserId = currentUserId;
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl(`${environment.hubsUrl}/hubs/chat`, {
        accessTokenFactory: () => token
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection.on('ReceiveMessage', (message: ChatMessage) => {
      const msg = { ...message, isOwn: message.senderId === this.currentUserId };
      this.messagesSignal.update(list => [...list, msg]);
    });

    this.hubConnection.on('UserConnected', (userId: string) => {
      this.onlineUsers.update(ids => ids.includes(userId) ? ids : [...ids, userId]);
      this.users.update(list => list.map(u => u.id === userId ? { ...u, isOnline: true } : u));
    });

    this.hubConnection.on('UserDisconnected', (userId: string) => {
      this.onlineUsers.update(ids => ids.filter(id => id !== userId));
      this.users.update(list => list.map(u => u.id === userId ? { ...u, isOnline: false } : u));
    });

    this.hubConnection.onreconnected(() => this.isConnected.set(true));
    this.hubConnection.onclose(() => this.isConnected.set(false));

    this.hubConnection.start()
      .then(() => this.isConnected.set(true))
      .catch(err => console.error('Chat hub error:', err));
  }

  stopConnection(): void {
    this.hubConnection?.stop();
    this.isConnected.set(false);
  }

  loadHistory(): Observable<ChatMessage[]> {
    return this.http.get<ChatMessage[]>(`${this.base}/history`).pipe(
      tap(data => {
        const mapped = data.map(m => ({ ...m, isOwn: m.senderId === this.currentUserId }));
        this.messagesSignal.set(mapped);
      })
    );
  }

  getUsers(): Observable<ChatUser[]> {
    return this.http.get<ChatUser[]>(`${this.base}/users`);
  }

  loadUsers(): void {
    this.getUsers().pipe(
      tap(data => {
        const onlineIds = this.onlineUsers();
        const merged = data.map(u => ({ ...u, isOnline: u.isOnline || onlineIds.includes(u.id) }));
        this.users.set(merged);
        this.onlineUsers.set(merged.filter(u => u.isOnline).map(u => u.id));
      })
    ).subscribe({ error: err => console.error('Failed to load users:', err) });
  }

  sendMessage(dto: SendMessageRequest): Observable<ChatMessage> {
    return this.http.post<ChatMessage>(`${this.base}/messages`, dto).pipe(
      tap(msg => this.messagesSignal.update(list => [...list, { ...msg, isOwn: true }]))
    );
  }

  sendViaHub(content: string): void {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendMessage', content).catch(err =>
        console.error('Send message error:', err)
      );
    }
  }

  sendFileViaHub(fileUrl: string, fileName: string, contentType: string, isPhoto: boolean): void {
    if (this.hubConnection?.state === signalR.HubConnectionState.Connected) {
      this.hubConnection.invoke('SendFileMessage', fileUrl, fileName, contentType, isPhoto).catch(err =>
        console.error('Send file message error:', err)
      );
    }
  }
}
