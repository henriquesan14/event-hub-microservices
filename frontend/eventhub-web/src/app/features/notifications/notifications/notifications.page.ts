import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { EventHubApi } from '../../../core/api/eventhub-api.service';
import { Notification } from '../../../core/api/models';
import { RealtimeNotifications } from '../../../core/realtime/realtime-notifications.service';

@Component({
  imports: [DatePipe],
  templateUrl: './notifications.page.html',
  styleUrl: './notifications.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NotificationsPage {
  private readonly api = inject(EventHubApi);
  private readonly realtime = inject(RealtimeNotifications);
  readonly notifications = signal<Notification[]>([]);
  readonly loading = signal(true);

  constructor() {
    this.api.notifications().subscribe({
      next: value => {
        this.notifications.set(value);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  read(notification: Notification): void {
    if (notification.isRead) return;
    this.api.markNotificationRead(notification.id).subscribe({
      next: () => this.notifications.update(values =>
        values.map(item => item.id === notification.id ? { ...item, isRead: true } : item)),
    });
    this.realtime.markOneRead();
  }

  readAll(): void {
    this.api.markAllNotificationsRead().subscribe({
      next: () => this.notifications.update(values => values.map(item => ({ ...item, isRead: true }))),
    });
    this.realtime.markAllRead();
  }
}
