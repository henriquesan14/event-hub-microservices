import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, computed, inject, signal } from '@angular/core';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { forkJoin } from 'rxjs';
import { apiErrorMessage } from '../../../core/api/api-error';
import { EventHubApi } from '../../../core/api/eventhub-api.service';
import { EventModel, TicketType } from '../../../core/api/models';
import { AuthStore } from '../../../core/auth/auth.store';

@Component({
  imports: [CurrencyPipe, DatePipe, RouterLink],
  templateUrl: './event-detail.page.html',
  styleUrl: './event-detail.page.scss',
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
