import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import * as signalR from '@microsoft/signalr';
import { ChatMessage, MessageType, SendMessageRequest } from '../models';
import { environment } from '../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ChatService {
  private readonly base = `${environment.apiUrl}/chat`;
  private hubConnection: signalR.HubConnection | null = null;
  private currentUserId = '';

  private messagesSignal = signal<ChatMessage[]>([]);
  readonly messages = computed(() => this.messagesSignal());
  readonly isConnected = signal(false);

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
