import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, maxLength, minLength, required } from '@angular/forms/signals';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorMessage } from '../../../core/api/api-error';
import { EventHubApi } from '../../../core/api/eventhub-api.service';
import { AuthStore } from '../../../core/auth/auth.store';

@Component({
  imports: [FormField, RouterLink],
  templateUrl: './event-editor.page.html',
  styleUrl: './event-editor.page.scss',
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
