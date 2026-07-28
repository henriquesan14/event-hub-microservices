import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { EventHubApi } from '../../core/api/eventhub-api.service';
import { EventModel, TicketType } from '../../core/api/models';
import { AuthStore } from '../../core/auth/auth.store';

@Component({
  imports: [CurrencyPipe, DatePipe, RouterLink],
  template: `
    <section class="detail-hero">
      <div class="page">
        <a class="back" routerLink="/">← Todos os eventos</a>
        @if (event()) {
          <span class="eyebrow">{{ event()!.address.city }} · {{ event()!.address.state }}</span>
          <h1 class="display-title">{{ event()!.title }}</h1>
          <div class="date-line">{{ event()!.startsAt | date:'EEEE, dd MMMM · HH:mm' }}</div>
          @if (canManage()) {
            <div class="manage-actions">
              <a class="light-button" [routerLink]="['/eventos', event()!.id, 'editar']">Editar evento</a>
              <a class="light-button" [routerLink]="['/eventos', event()!.id, 'ingressos']">Gerenciar ingressos</a>
              @if (isDraft()) {
                <button class="publish-button" type="button" (click)="publish()">Publicar</button>
              }
              @if (!isCancelled()) {
                <button class="cancel-button" type="button" (click)="cancel()">Cancelar evento</button>
              }
            </div>
          }
        }
      </div>
    </section>

    <section class="page detail-grid">
      @if (loading()) {
        <div class="skeleton"></div>
      } @else if (error()) {
        <div class="alert error">{{ error() }}</div>
      } @else if (event()) {
        <article>
          <span class="eyebrow">Sobre</span>
          <h2>Uma experiência para guardar.</h2>
          <p class="description">{{ event()!.description }}</p>
          <div class="location card">
            <strong>Onde acontece</strong>
            <span>{{ fullAddress(event()!) }}</span>
          </div>
        </article>

        <aside class="card tickets">
          <span class="eyebrow">Ingressos</span>
          <h2>Escolha seu lugar</h2>
          @if (tickets().length === 0) {
            <p class="muted">Ainda não há ingressos disponíveis.</p>
          }
          @for (ticket of tickets(); track ticket.id) {
            <div class="ticket-row" [class.sold-out]="isSoldOut(ticket)">
              <div>
                <strong>{{ ticket.name }}</strong>
                @if (isSoldOut(ticket)) {
                  <small class="sold-out-label">Ingresso esgotado</small>
                } @else {
                  <small>{{ ticket.availableQuantity }} disponíveis</small>
                }
              </div>
              <span>{{ ticket.price | currency:ticket.currency }}</span>
            </div>
            @if (isSoldOut(ticket)) {
              <div class="sold-out-button">Esgotado</div>
            } @else {
              <div class="reserve-row">
                <select aria-label="Quantidade" [value]="quantity(ticket.id)"
                  (change)="setQuantity(ticket.id, +$any($event.target).value)">
                  @for (number of quantities(ticket); track number) { <option [value]="number">{{ number }}</option> }
                </select>
                <button class="primary-button" type="button" [disabled]="reserving() === ticket.id"
                  (click)="reserve(ticket)">
                  {{ reserving() === ticket.id ? 'Reservando…' : 'Reservar' }}
                </button>
              </div>
            }
          }
          @if (reservationMessage()) { <div class="alert success">{{ reservationMessage() }}</div> }
        </aside>
      }
    </section>
  `,
  styles: [`
    .detail-hero { background: #263a34; color: #f4f0e5; }
    .detail-hero .page { padding-block: clamp(60px, 9vw, 110px); }
    .detail-hero .eyebrow { display: block; margin-top: 50px; }
    .back { color: #b9d9c3; font-weight: 700; }
    .date-line { color: #c8d1cd; font-size: 1.06rem; margin-top: 28px; }
    .manage-actions { display: flex; flex-wrap: wrap; gap: 10px; margin-top: 28px; }
    .manage-actions a, .manage-actions button { border-radius: 999px; font-weight: 800; min-height: 42px; padding: 0 17px; }
    .light-button { align-items: center; background: #f4f0e5; color: #263a34; display: inline-flex; }
    .publish-button { background: #b9d9c3; border: 0; color: #194c32; }
    .cancel-button { background: transparent; border: 1px solid #87958f; color: #f4f0e5; }
    .detail-grid { display: grid; gap: clamp(30px, 7vw, 90px); grid-template-columns: 1.2fr .8fr; }
    article h2, .tickets h2 { font-size: clamp(1.8rem, 4vw, 3rem); margin: 10px 0 20px; }
    .description { color: var(--muted); font-size: 1.08rem; line-height: 1.8; white-space: pre-line; }
    .location { display: grid; gap: 8px; margin-top: 34px; padding: 24px; }
    .location span, .muted { color: var(--muted); }
    .tickets { align-self: start; padding: 30px; position: sticky; top: 100px; }
    .ticket-row { align-items: center; border-top: 1px solid var(--line); display: flex; justify-content: space-between; padding: 22px 0 10px; }
    .ticket-row > div { display: grid; gap: 5px; }
    .ticket-row small { color: var(--muted); }
    .ticket-row > span { font-weight: 800; }
    .ticket-row.sold-out { opacity: .62; }
    .sold-out-label { color: var(--danger) !important; font-weight: 800; text-transform: uppercase; }
    .sold-out-button { background: #efefeb; border-radius: 999px; color: var(--muted); font-weight: 800; margin-bottom: 22px; padding: 13px; text-align: center; }
    .reserve-row { display: flex; gap: 10px; margin-bottom: 22px; }
    .reserve-row select { border: 1px solid var(--line); border-radius: 999px; padding: 0 13px; }
    .reserve-row .primary-button { flex: 1; }
    @media (max-width: 760px) { .detail-grid { grid-template-columns: 1fr; } .tickets { position: static; } }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EventDetailPage {
  private readonly api = inject(EventHubApi);
  private readonly auth = inject(AuthStore);
  private readonly router = inject(Router);
  private readonly id = inject(ActivatedRoute).snapshot.paramMap.get('id')!;
  readonly event = signal<EventModel | null>(null);
  readonly tickets = signal<TicketType[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly reserving = signal('');
  readonly reservationMessage = signal('');
  readonly selectedQuantities = signal<Record<string, number>>({});
  readonly canManage = computed(() => {
    const event = this.event();
    return event !== null && (
      this.auth.isAdmin() ||
      this.auth.isOrganizer() && event.organizerId === this.auth.user()?.id
    );
  });

  constructor() {
    forkJoin({ event: this.api.event(this.id), tickets: this.api.ticketTypes(this.id) }).subscribe({
      next: result => {
        this.event.set(result.event);
        this.tickets.set(result.tickets);
        this.loading.set(false);
      },
      error: err => {
        this.error.set(apiErrorMessage(err));
        this.loading.set(false);
      },
    });
  }

  quantity(id: string): number { return this.selectedQuantities()[id] ?? 1; }
  setQuantity(id: string, quantity: number): void {
    this.selectedQuantities.update(value => ({ ...value, [id]: quantity }));
  }
  quantities(ticket: TicketType): number[] {
    return Array.from({ length: Math.min(ticket.availableQuantity, 5) }, (_, index) => index + 1);
  }

  isSoldOut(ticket: TicketType): boolean {
    return ticket.availableQuantity <= 0
      || ticket.status === 'SoldOut'
      || ticket.status === 2;
  }
  fullAddress(event: EventModel): string {
    const a = event.address;
    return `${a.street}, ${a.number} · ${a.district} · ${a.city}/${a.state}`;
  }

  reserve(ticket: TicketType): void {
    if (!this.auth.isAuthenticated()) {
      void this.router.navigate(['/entrar'], { queryParams: { returnUrl: this.router.url } });
      return;
    }
    this.reserving.set(ticket.id);
    this.api.reserve(ticket.id, this.quantity(ticket.id)).subscribe({
      next: reservation => {
        this.reserving.set('');
        this.reservationMessage.set('Reserva criada. Seu pedido e pagamento serão preparados automaticamente.');
        setTimeout(() => void this.router.navigate(['/minha-conta'], {
          queryParams: { reservation: reservation.id },
        }), 900);
      },
      error: err => {
        this.reserving.set('');
        this.error.set(apiErrorMessage(err));
      },
    });
  }

  isDraft(): boolean {
    const status = this.event()?.status;
    return status === 0 || status === 'Draft';
  }

  isCancelled(): boolean {
    const status = this.event()?.status;
    return status === 2 || status === 'Cancelled';
  }

  publish(): void {
    this.api.publishEvent(this.id).subscribe({
      next: event => this.event.set(event),
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }

  cancel(): void {
    if (!confirm('Deseja realmente cancelar este evento?')) return;
    this.api.cancelEvent(this.id).subscribe({
      next: event => this.event.set(event),
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }
}
