import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { QRCodeComponent } from 'angularx-qrcode';
import { EventHubApi } from '../../core/api/eventhub-api.service';
import { AdmissionTicket } from '../../core/api/models';

@Component({
  imports: [DatePipe, QRCodeComponent, RouterLink],
  template: `
    <section class="page tickets-page">
      <div class="heading">
        <div>
          <span class="eyebrow">EventHub Pass</span>
          <h1>Meus ingressos</h1>
          <p>Tenha seus QR Codes à mão para entrar sem complicação.</p>
        </div>
        <a class="primary-button" routerLink="/">Explorar eventos</a>
      </div>

      @if (loading()) {
        <div class="ticket-grid">
          @for (item of [1, 2]; track item) { <div class="skeleton"></div> }
        </div>
      } @else if (tickets().length === 0) {
        <div class="card empty-state">
          <h2>Você ainda não possui ingressos</h2>
          <p>Seus QR Codes aparecerão aqui após a confirmação do pagamento.</p>
          <a class="primary-button" routerLink="/">Encontrar eventos</a>
        </div>
      } @else {
        <div class="ticket-grid">
          @for (ticket of tickets(); track ticket.id) {
            <article class="ticket" [class.used]="isUsed(ticket)" [class.cancelled]="isCancelled(ticket)">
              <div class="ticket-info">
                <span class="eyebrow">EventHub Pass</span>
                <span class="ticket-status">{{ statusLabel(ticket) }}</span>
                <h2>{{ ticket.ticketName }}</h2>
                <small>Emitido em {{ ticket.issuedAt | date:'dd/MM/yyyy · HH:mm' }}</small>
                @if (ticket.checkedInAt) {
                  <small>Utilizado em {{ ticket.checkedInAt | date:'dd/MM/yyyy · HH:mm' }}</small>
                }
              </div>
              <div class="ticket-code">
                <div class="qr-frame">
                  <qrcode
                    [qrdata]="ticket.code"
                    [width]="190"
                    [margin]="1"
                    [errorCorrectionLevel]="'M'"
                    aria-label="QR Code do ingresso"
                  />
                </div>
                <span>{{ isActive(ticket) ? 'Apresente este QR Code na entrada' : 'Este ingresso não pode mais ser utilizado' }}</span>
              </div>
            </article>
          }
        </div>
      }
    </section>
  `,
  styles: [`
    .tickets-page { max-width: 1050px; }
    .heading { align-items: end; display: flex; justify-content: space-between; margin-bottom: 42px; }
    .heading h1 { font-size: clamp(2.7rem, 6vw, 5rem); margin: 8px 0; }
    .heading p { color: var(--muted); }
    .ticket-grid { display: grid; gap: 22px; grid-template-columns: repeat(2, 1fr); }
    .ticket { background: #263a34; border-radius: 24px; color: white; display: grid; gap: 24px; grid-template-columns: 1fr auto; overflow: hidden; padding: 28px; position: relative; }
    .ticket.used, .ticket.cancelled { filter: grayscale(.8); opacity: .7; }
    .ticket-info { display: flex; flex-direction: column; }
    .ticket-info h2 { font-size: 1.5rem; margin: 20px 0 8px; }
    .ticket-info small { color: #afc1ba; margin-top: 5px; }
    .ticket-status { align-self: start; background: rgba(255,255,255,.12); border-radius: 999px; font-size: .7rem; font-weight: 800; margin-top: 12px; padding: 6px 9px; text-transform: uppercase; }
    .ticket-code { border-left: 1px dashed #6d8179; display: grid; gap: 10px; justify-items: center; padding-left: 24px; place-content: center; }
    .qr-frame { background: white; border-radius: 13px; line-height: 0; padding: 8px; }
    .ticket-code span { color: #afc1ba; font-size: .7rem; max-width: 190px; text-align: center; }
    @media (max-width: 900px) { .ticket-grid { grid-template-columns: 1fr; } }
    @media (max-width: 600px) { .heading { align-items: start; flex-direction: column; gap: 20px; } .ticket { grid-template-columns: 1fr; } .ticket-code { border-left: 0; border-top: 1px dashed #6d8179; padding: 24px 0 0; } }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class MyTicketsPage {
  private readonly api = inject(EventHubApi);
  readonly tickets = signal<AdmissionTicket[]>([]);
  readonly loading = signal(true);

  constructor() {
    this.api.admissionTickets().subscribe({
      next: tickets => {
        this.tickets.set(tickets);
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
}
