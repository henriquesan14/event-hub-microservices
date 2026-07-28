import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, maxLength, min, required } from '@angular/forms/signals';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { apiErrorMessage } from '../../../core/api/api-error';
import { EventHubApi } from '../../../core/api/eventhub-api.service';
import { EventModel, TicketType } from '../../../core/api/models';
import { AuthStore } from '../../../core/auth/auth.store';

@Component({
  imports: [CurrencyPipe, DatePipe, FormField, RouterLink],
  templateUrl: './ticket-manager.page.html',
  styleUrl: './ticket-manager.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class TicketManagerPage {
  private readonly api = inject(EventHubApi);
  private readonly auth = inject(AuthStore);
  private readonly router = inject(Router);
  readonly eventId = inject(ActivatedRoute).snapshot.paramMap.get('id')!;
  readonly event = signal<EventModel | null>(null);
  readonly tickets = signal<TicketType[]>([]);
  readonly loading = signal(true);
  readonly saving = signal(false);
  readonly error = signal('');
  readonly editingTicketId = signal<string | null>(null);
  readonly model = signal({
    name: '',
    description: '',
    price: 0,
    currency: 'BRL',
    totalQuantity: 1,
    salesStart: '',
    salesEnd: '',
    active: true,
  });
  readonly ticketForm = form(this.model, schema => {
    required(schema.name);
    required(schema.description);
    min(schema.price, 0);
    required(schema.currency);
    maxLength(schema.currency, 3);
    min(schema.totalQuantity, 1);
    required(schema.salesStart);
    required(schema.salesEnd);
  });

  constructor() {
    forkJoin({
      event: this.api.event(this.eventId),
      tickets: this.api.ticketTypes(this.eventId),
    }).subscribe({
      next: result => {
        if (!this.auth.isAdmin() && result.event.organizerId !== this.auth.user()?.id) {
          void this.router.navigateByUrl('/');
          return;
        }
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

  submit(event: Event): void {
    event.preventDefault();
    this.saving.set(true);
    this.error.set('');
    const ticketId = this.editingTicketId();
    const request = ticketId
      ? this.api.updateTicketType(ticketId, this.model())
      : this.api.createTicketType(this.eventId, this.model());

    request.pipe(
      finalize(() => this.saving.set(false)),
    ).subscribe({
      next: ticket => {
        this.tickets.update(values => ticketId
          ? values.map(item => item.id === ticket.id ? ticket : item)
          : [...values, ticket]);
        this.resetForm();
      },
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }

  edit(ticket: TicketType): void {
    this.editingTicketId.set(ticket.id);
    this.model.set({
      name: ticket.name,
      description: ticket.description,
      price: ticket.price,
      currency: ticket.currency,
      totalQuantity: ticket.totalQuantity,
      salesStart: this.toLocalInput(ticket.salesStart),
      salesEnd: this.toLocalInput(ticket.salesEnd),
      active: ticket.status === 'Active' || ticket.status === 0,
    });
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  resetForm(): void {
    this.editingTicketId.set(null);
    this.model.set({
      name: '',
      description: '',
      price: 0,
      currency: 'BRL',
      totalQuantity: 1,
      salesStart: '',
      salesEnd: '',
      active: true,
    });
  }

  remove(ticket: TicketType): void {
    if (!confirm(`Excluir o ingresso "${ticket.name}"?`)) return;
    this.api.deleteTicketType(ticket.id).subscribe({
      next: () => this.tickets.update(values => values.filter(item => item.id !== ticket.id)),
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }

  private toLocalInput(value: string): string {
    return value.length >= 16 ? value.slice(0, 16) : value;
  }
}
