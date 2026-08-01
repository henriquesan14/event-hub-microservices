import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { QRCodeComponent } from 'angularx-qrcode';
import { EventHubApi } from '../../../core/api/eventhub-api.service';
import { AdmissionTicket } from '../../../core/api/models';

@Component({
  imports: [DatePipe, QRCodeComponent, RouterLink],
  templateUrl: './my-tickets.page.html',
  styleUrl: './my-tickets.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyTicketsPage {
  private readonly api = inject(EventHubApi);
  readonly tickets = signal<AdmissionTicket[]>([]);
  private readonly eventNames = signal<Record<string, string>>({});
  private readonly eventDates = signal<Record<string, string>>({});
  readonly loading = signal(true);

  constructor() {
    this.api.admissionTickets().subscribe({
      next: tickets => {
        this.tickets.set(tickets);
        this.loadMissingEventNames(tickets);
        this.loading.set(false);
      },
      error: () => this.loading.set(false),
    });
  }

  isActive(ticket: AdmissionTicket): boolean {
    return ticket.status === 'Active' || ticket.status === 0;
  }

  isUsed(ticket: AdmissionTicket): boolean {
    return ticket.status === 'Used' || ticket.status === 1;
  }

  isCancelled(ticket: AdmissionTicket): boolean {
    return ticket.status === 'Cancelled' || ticket.status === 2;
  }

  statusLabel(ticket: AdmissionTicket): string {
    if (this.isUsed(ticket)) return 'Utilizado';
    if (this.isCancelled(ticket)) return 'Cancelado';
    return 'Ativo';
  }

  eventName(ticket: AdmissionTicket): string {
    return ticket.eventName || this.eventNames()[ticket.eventId] || 'Evento';
  }

  eventStartsAt(ticket: AdmissionTicket): string | undefined {
    return ticket.eventStartsAt || this.eventDates()[ticket.eventId];
  }

  private loadMissingEventNames(tickets: AdmissionTicket[]): void {
    const ids = [...new Set(tickets
      .filter(ticket => !ticket.eventName || !ticket.eventStartsAt)
      .map(ticket => ticket.eventId))];
    for (const id of ids) {
      this.api.event(id).subscribe({
        next: event => {
          this.eventNames.update(names => ({ ...names, [id]: event.title }));
          this.eventDates.update(dates => ({ ...dates, [id]: event.startsAt }));
        },
      });
    }
  }
}
