import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { apiErrorMessage } from '../../../core/api/api-error';
import { EventModel } from '../../../core/api/models';
import { EventHubApi } from '../../../core/api/eventhub-api.service';

@Component({
  imports: [DatePipe, RouterLink],
  templateUrl: './event-list.page.html',
  styleUrl: './event-list.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class EventListPage {
  private readonly api = inject(EventHubApi);
  readonly events = signal<EventModel[]>([]);
  readonly loading = signal(true);
  readonly error = signal('');
  readonly term = signal('');
  readonly total = signal(0);
  constructor() { this.load(); }

  search(event: Event): void {
    event.preventDefault();
    this.load();
  }

  statusLabel(status: EventModel['status']): string {
    return ['Rascunho', 'Publicado', 'Cancelado', 'Encerrado'][Number(status)] ?? String(status);
  }

  private load(): void {
    this.loading.set(true);
    this.error.set('');
    this.api.events(this.term()).subscribe({
      next: result => {
        this.events.set(result.data ?? []);
        this.total.set(result.count ?? 0);
        this.loading.set(false);
      },
      error: err => {
        this.error.set(apiErrorMessage(err));
        this.loading.set(false);
      },
    });
  }
}
