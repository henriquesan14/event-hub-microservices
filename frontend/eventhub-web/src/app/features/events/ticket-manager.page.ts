import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, maxLength, min, required } from '@angular/forms/signals';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize, forkJoin } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { EventHubApi } from '../../core/api/eventhub-api.service';
import { EventModel, TicketType } from '../../core/api/models';
import { AuthStore } from '../../core/auth/auth.store';

@Component({
  imports: [CurrencyPipe, DatePipe, FormField, RouterLink],
  template: `
    <section class="page manager-page">
      <a class="back" [routerLink]="['/eventos', eventId]">← Voltar ao evento</a>
      <div class="heading">
        <div>
          <span class="eyebrow">Configuração</span>
          <h1>Ingressos</h1>
          @if (event()) { <p>{{ event()!.title }}</p> }
        </div>
      </div>

      @if (error()) { <div class="alert error">{{ error() }}</div> }

      <div class="manager-grid">
        <form class="card ticket-form" (submit)="submit($event)">
          <h2>{{ editingTicketId() ? 'Editar ingresso' : 'Novo tipo de ingresso' }}</h2>
          <div class="field"><label for="name">Nome</label><input id="name" placeholder="Ex.: Inteira" [formField]="ticketForm.name" /></div>
          <div class="field"><label for="description">Descrição</label><textarea id="description" [formField]="ticketForm.description"></textarea></div>
          <div class="form-grid">
            <div class="field"><label for="price">Preço</label><input id="price" type="number" step="0.01" [formField]="ticketForm.price" /></div>
            <div class="field"><label for="currency">Moeda</label><input id="currency" [formField]="ticketForm.currency" /></div>
            <div class="field"><label for="quantity">Quantidade</label><input id="quantity" type="number" [formField]="ticketForm.totalQuantity" /></div>
          </div>
          <div class="field"><label for="salesStart">Início das vendas</label><input id="salesStart" type="datetime-local" [formField]="ticketForm.salesStart" /></div>
          <div class="field"><label for="salesEnd">Fim das vendas</label><input id="salesEnd" type="datetime-local" [formField]="ticketForm.salesEnd" /></div>
          @if (editingTicketId()) {
            <div class="active-field">
              <input id="active" type="checkbox" [formField]="ticketForm.active" />
              <label for="active">Ingresso ativo para venda</label>
            </div>
          }
          <button class="primary-button full" type="submit" [disabled]="saving()">
            {{ saving() ? 'Salvando…' : editingTicketId() ? 'Salvar alterações' : 'Criar ingresso' }}
          </button>
          @if (editingTicketId()) {
            <button class="ghost-button full cancel-edit" type="button" (click)="resetForm()">Cancelar edição</button>
          }
        </form>

        <div>
          <h2>Ingressos configurados</h2>
          @if (loading()) {
            <div class="skeleton"></div>
          } @else if (tickets().length === 0) {
            <div class="card empty-state">Nenhum ingresso criado.</div>
          } @else {
            <div class="ticket-list">
              @for (ticket of tickets(); track ticket.id) {
                <article class="card ticket-item">
                  <div>
                    <span class="status">{{ ticket.status }}</span>
                    <h3>{{ ticket.name }}</h3>
                    <p>{{ ticket.availableQuantity }} de {{ ticket.totalQuantity }} disponíveis</p>
                    <small>Vendas: {{ ticket.salesStart | date:'dd/MM/yy HH:mm' }} – {{ ticket.salesEnd | date:'dd/MM/yy HH:mm' }}</small>
                  </div>
                  <div class="ticket-actions">
                    <strong>{{ ticket.price | currency:ticket.currency }}</strong>
                    <div>
                      <button class="edit-link" type="button" (click)="edit(ticket)">Editar</button>
                      <button class="danger-link" type="button" (click)="remove(ticket)">Excluir</button>
                    </div>
                  </div>
                </article>
              }
            </div>
          }
        </div>
      </div>
    </section>
  `,
  styles: [`
    .manager-page { max-width: 1100px; }
    .back { color: var(--muted); font-weight: 700; }
    .heading { margin: 36px 0; }
    .heading h1 { font-size: clamp(2.5rem, 6vw, 4.6rem); margin: 8px 0; }
    .heading p { color: var(--muted); }
    .manager-grid { display: grid; gap: 40px; grid-template-columns: .8fr 1.2fr; }
    .ticket-form { padding: 28px; }
    .form-grid { display: grid; gap: 12px; grid-template-columns: 1.4fr .8fr 1fr; }
    .full { width: 100%; }
    .cancel-edit { margin-top: 10px; }
    .active-field { align-items: center; display: flex; gap: 10px; margin: 8px 0 20px; }
    .active-field input { height: 18px; width: 18px; }
    .ticket-list { display: grid; gap: 12px; }
    .ticket-item { display: flex; justify-content: space-between; padding: 22px; }
    .ticket-item h3 { margin: 10px 0 5px; }
    .ticket-item p, .ticket-item small { color: var(--muted); }
    .ticket-actions { align-items: end; display: flex; flex-direction: column; justify-content: space-between; }
    .danger-link { background: none; border: 0; color: var(--danger); font-weight: 700; padding: 4px; }
    .edit-link { background: none; border: 0; color: var(--accent); font-weight: 700; margin-right: 8px; padding: 4px; }
    @media (max-width: 800px) { .manager-grid { grid-template-columns: 1fr; } .form-grid { grid-template-columns: 1fr; } }
  `],
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
