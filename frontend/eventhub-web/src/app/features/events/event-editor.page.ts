import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { EventHubApi } from '../../core/api/eventhub-api.service';
import { AuthStore } from '../../core/auth/auth.store';

@Component({
  imports: [FormField, RouterLink],
  template: `
    <section class="page editor-page">
      <a class="back" [routerLink]="eventId ? ['/eventos', eventId] : ['/']">← Voltar</a>
      <div class="editor-heading">
        <span class="eyebrow">Painel do organizador</span>
        <h1>{{ eventId ? 'Editar evento' : 'Criar evento' }}</h1>
        <p>Preencha os dados principais. Você poderá configurar os ingressos em seguida.</p>
      </div>

      @if (loadingEvent()) {
        <div class="skeleton"></div>
      } @else {
        <form class="card editor-form" (submit)="submit($event)">
          @if (error()) { <div class="alert error">{{ error() }}</div> }

          <div class="field wide">
            <label for="title">Nome do evento</label>
            <input id="title" [formField]="eventForm.title" />
          </div>
          <div class="field wide">
            <label for="description">Descrição</label>
            <textarea id="description" [formField]="eventForm.description"></textarea>
          </div>
          <div class="field">
            <label for="startsAt">Início</label>
            <input id="startsAt" type="datetime-local" [formField]="eventForm.startsAt" />
          </div>
          <div class="field">
            <label for="endsAt">Término</label>
            <input id="endsAt" type="datetime-local" [formField]="eventForm.endsAt" />
          </div>

          <h2 class="wide">Local do evento</h2>
          <div class="field wide">
            <label for="street">Rua</label>
            <input id="street" [formField]="eventForm.address.street" />
          </div>
          <div class="field">
            <label for="number">Número</label>
            <input id="number" [formField]="eventForm.address.number" />
          </div>
          <div class="field">
            <label for="district">Bairro</label>
            <input id="district" [formField]="eventForm.address.district" />
          </div>
          <div class="field">
            <label for="city">Cidade</label>
            <input id="city" [formField]="eventForm.address.city" />
          </div>
          <div class="field">
            <label for="state">Estado</label>
            <input id="state" [formField]="eventForm.address.state" />
          </div>
          <div class="field">
            <label for="zipCode">CEP</label>
            <input id="zipCode" [formField]="eventForm.address.zipCode" />
          </div>
          <div class="field">
            <label for="country">País</label>
            <input id="country" [formField]="eventForm.address.country" />
          </div>

          <div class="wide submit-row">
            <a class="ghost-button" [routerLink]="eventId ? ['/eventos', eventId] : ['/']">Cancelar</a>
            <button class="primary-button" type="submit" [disabled]="saving()">
              {{ saving() ? 'Salvando…' : eventId ? 'Salvar alterações' : 'Criar e configurar ingressos' }}
            </button>
          </div>
        </form>
      }
    </section>
  `,
  styles: [`
    .editor-page { max-width: 980px; }
    .back { color: var(--muted); font-weight: 700; }
    .editor-heading { margin: 36px 0; }
    .editor-heading h1 { font-size: clamp(2.5rem, 6vw, 4.6rem); margin: 8px 0; }
    .editor-heading p { color: var(--muted); }
    .editor-form { display: grid; gap: 4px 20px; grid-template-columns: 1fr 1fr; padding: clamp(24px, 5vw, 46px); }
    .wide { grid-column: 1 / -1; }
    .editor-form h2 { border-top: 1px solid var(--line); font-size: 1.5rem; margin: 20px 0; padding-top: 30px; }
    .submit-row { display: flex; gap: 12px; justify-content: flex-end; margin-top: 22px; }
    @media (max-width: 620px) { .editor-form { grid-template-columns: 1fr; } .wide { grid-column: auto; } .submit-row { align-items: stretch; flex-direction: column; } }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EventEditorPage {
  private readonly api = inject(EventHubApi);
  private readonly auth = inject(AuthStore);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);
  readonly eventId = this.route.snapshot.paramMap.get('id');
  readonly loadingEvent = signal(Boolean(this.eventId));
  readonly saving = signal(false);
  readonly error = signal('');
  readonly model = signal({
    title: '',
    description: '',
    startsAt: '',
    endsAt: '',
    address: {
      street: '',
      number: '',
      district: '',
      city: '',
      state: '',
      country: 'Brasil',
      zipCode: '',
    },
  });
  readonly eventForm = form(this.model, schema => {
    required(schema.title);
    minLength(schema.title, 3);
    required(schema.description);
    required(schema.startsAt);
    required(schema.endsAt);
    required(schema.address.street);
    required(schema.address.number);
    required(schema.address.city);
    required(schema.address.state);
    maxLength(schema.address.state, 2);
    required(schema.address.country);
    required(schema.address.zipCode);
  });

  constructor() {
    if (!this.auth.isAdmin() && !this.auth.isOrganizer()) {
      void this.router.navigateByUrl('/');
      return;
    }
    if (!this.eventId) return;

    this.api.event(this.eventId).subscribe({
      next: event => {
        if (!this.auth.isAdmin() && event.organizerId !== this.auth.user()?.id) {
          void this.router.navigateByUrl('/');
          return;
        }
        this.model.set({
          title: event.title,
          description: event.description,
          startsAt: this.toLocalInput(event.startsAt),
          endsAt: this.toLocalInput(event.endsAt),
          address: { ...event.address },
        });
        this.loadingEvent.set(false);
      },
      error: err => {
        this.error.set(apiErrorMessage(err));
        this.loadingEvent.set(false);
      },
    });
  }

  submit(event: Event): void {
    event.preventDefault();
    this.saving.set(true);
    this.error.set('');
    const request = this.eventId
      ? this.api.updateEvent(this.eventId, this.model())
      : this.api.createEvent(this.model());

    request.pipe(finalize(() => this.saving.set(false))).subscribe({
      next: saved => void this.router.navigate(['/eventos', saved.id, 'ingressos']),
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }

  private toLocalInput(value: string): string {
    return value.length >= 16 ? value.slice(0, 16) : value;
  }
}
