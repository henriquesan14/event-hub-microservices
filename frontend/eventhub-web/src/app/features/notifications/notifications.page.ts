import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { EventHubApi } from '../../core/api/eventhub-api.service';
import { Notification } from '../../core/api/models';

@Component({
  imports: [DatePipe],
  template: `
    <section class="page">
      <div class="heading">
        <div><span class="eyebrow">Atualizações</span><h1>Notificações</h1></div>
        @if (notifications().length > 0) {
          <button class="ghost-button" type="button" (click)="readAll()">Marcar todas como lidas</button>
        }
      </div>

      @if (loading()) {
        <div class="skeleton"></div>
      } @else if (notifications().length === 0) {
        <div class="card empty-state">Nenhuma notificação por enquanto.</div>
      } @else {
        <div class="notification-list">
          @for (notification of notifications(); track notification.id) {
            <button class="notification card" [class.unread]="!notification.isRead" type="button"
              (click)="read(notification)">
              <span class="dot"></span>
              <span class="content">
                <strong>{{ notification.title }}</strong>
                <span>{{ notification.message }}</span>
                <small>{{ notification.createdAt | date:'dd/MM/yyyy · HH:mm' }}</small>
              </span>
            </button>
          }
        </div>
      }
    </section>
  `,
  styles: [`
    .heading { align-items: end; display: flex; justify-content: space-between; margin-bottom: 38px; }
    h1 { font-size: clamp(2.7rem, 6vw, 5rem); margin: 8px 0 0; }
    .notification-list { display: grid; gap: 10px; }
    .notification { align-items: start; color: var(--ink); display: grid; gap: 16px; grid-template-columns: 10px 1fr; padding: 22px; text-align: left; width: 100%; }
    .notification.unread { border-color: #e1a08e; background: #fffaf7; }
    .dot { background: var(--line); border-radius: 50%; height: 8px; margin-top: 6px; width: 8px; }
    .unread .dot { background: var(--accent); }
    .content { display: grid; gap: 5px; }
    .content > span { color: var(--muted); line-height: 1.5; }
    .content small { color: #929793; margin-top: 5px; }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class NotificationsPage {
  private readonly api = inject(EventHubApi);
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
  }

  readAll(): void {
    this.api.markAllNotificationsRead().subscribe({
      next: () => this.notifications.update(values => values.map(item => ({ ...item, isRead: true }))),
    });
  }
}
