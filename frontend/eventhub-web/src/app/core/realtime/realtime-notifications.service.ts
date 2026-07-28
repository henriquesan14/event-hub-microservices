import { effect, inject, Injectable, signal } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  HubConnectionState,
  LogLevel,
} from '@microsoft/signalr';
import { AuthStore } from '../auth/auth.store';
import { EventHubApi } from '../api/eventhub-api.service';

export interface RealtimeNotificationMessage {
  type: string;
  resourceId: string;
  title: string;
  message: string;
  actionUrl?: string;
}

@Injectable({ providedIn: 'root' })
export class RealtimeNotifications {
  private readonly auth = inject(AuthStore);
  private readonly api = inject(EventHubApi);
  private connection: HubConnection | null = null;
  private readonly currentToast = signal<RealtimeNotificationMessage | null>(null);
  private readonly currentUnreadCount = signal(0);

  readonly toast = this.currentToast.asReadonly();
  readonly unreadCount = this.currentUnreadCount.asReadonly();

  constructor() {
    effect(() => {
      if (this.auth.isAuthenticated()) {
        void this.connect();
      } else {
        void this.disconnect();
      }
    });
  }

  dismiss(): void {
    this.currentToast.set(null);
  }

  markOneRead(): void {
    this.currentUnreadCount.update(count => Math.max(0, count - 1));
  }

  markAllRead(): void {
    this.currentUnreadCount.set(0);
  }

  private async connect(): Promise<void> {
    if (this.connection &&
        this.connection.state !== HubConnectionState.Disconnected) {
      return;
    }

    this.connection = new HubConnectionBuilder()
      .withUrl('/hubs/notifications', { withCredentials: true })
      .withAutomaticReconnect()
      .configureLogging(LogLevel.Warning)
      .build();

    this.connection.on(
      'notificationReceived',
      (message: RealtimeNotificationMessage) => {
        this.currentToast.set(message);
        this.currentUnreadCount.update(count => count + 1);
        window.setTimeout(() => {
          if (this.currentToast()?.resourceId === message.resourceId &&
              this.currentToast()?.type === message.type) {
            this.currentToast.set(null);
          }
        }, 10000);
      },
    );

    try {
      await this.connection.start();
      this.api.unreadCount().subscribe({
        next: count => this.currentUnreadCount.set(count),
      });
    } catch {
      // Automatic reconnect only applies after a successful connection.
      window.setTimeout(() => void this.connect(), 5000);
    }
  }

  private async disconnect(): Promise<void> {
    if (!this.connection) return;
    await this.connection.stop();
    this.connection = null;
    this.currentToast.set(null);
    this.currentUnreadCount.set(0);
  }
}
