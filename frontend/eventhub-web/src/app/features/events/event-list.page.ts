import { DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { RouterLink } from '@angular/router';
import { apiErrorMessage } from '../../core/api/api-error';
import { EventModel } from '../../core/api/models';
import { EventHubApi } from '../../core/api/eventhub-api.service';

@Component({
  imports: [DatePipe, RouterLink],
  template: `
    <section class="hero">
      <div class="page">
        <span class="eyebrow">Descubra. Viva. Lembre.</span>
        <h1 class="display-title">A próxima história começa ao vivo.</h1>
        <p class="lead">Encontre experiências marcantes, reserve seu lugar e leve o ingresso no bolso.</p>
        <form class="search" (submit)="search($event)">
          <input aria-label="Pesquisar eventos" placeholder="Busque por nome do evento" [value]="term()"
            (input)="term.set($any($event.target).value)" />
          <button class="primary-button" type="submit">Buscar</button>
        </form>
      </div>
    </section>

    <section class="page events-section">
      <div class="section-heading">
        <div>
          <span class="eyebrow">Agenda</span>
          <h2>Eventos em destaque</h2>
        </div>
        @if (total() > 0) { <span>{{ total() }} eventos encontrados</span> }
      </div>

      @if (loading()) {
        <div class="event-grid">
          @for (item of [1,2,3]; track item) { <div class="skeleton"></div> }
        </div>
      } @else if (error()) {
        <div class="card empty-state">
          <h3>Não foi possível carregar os eventos</h3>
          <p>{{ error() }}</p>
          <a class="primary-button" routerLink="/entrar">Entrar na conta</a>
        </div>
      } @else if (events().length === 0) {
        <div class="card empty-state"><h3>Nenhum evento encontrado</h3><p>Tente buscar por outro termo.</p></div>
      } @else {
        <div class="event-grid">
          @for (event of events(); track event.id; let index = $index) {
            <a class="event-card" [routerLink]="['/eventos', event.id]">
              <div class="event-art" [class.alt]="index % 3 === 1" [class.dark]="index % 3 === 2">
                <span>{{ event.address.city }}</span>
                <strong>{{ event.startsAt | date:'dd' }}</strong>
                <small>{{ event.startsAt | date:'MMM' }}</small>
              </div>
              <div class="event-copy">
                <span class="status">{{ statusLabel(event.status) }}</span>
                <h3>{{ event.title }}</h3>
                <p>{{ event.description }}</p>
                <div class="event-meta">
                  <span>{{ event.startsAt | date:'dd/MM/yyyy · HH:mm' }}</span>
                  <span>{{ event.address.city }} · {{ event.address.state }}</span>
                </div>
              </div>
            </a>
          }
        </div>
      }
    </section>
  `,
  styles: [`
    .hero { background: #dcebdc; border-bottom: 1px solid var(--line); overflow: hidden; position: relative; }
    .hero::after { background: var(--accent); border-radius: 50%; content: ''; height: 360px; opacity: .9; position: absolute; right: -120px; top: -120px; width: 360px; }
    .hero .page { padding-block: clamp(70px, 10vw, 130px); position: relative; z-index: 1; }
    .search { background: white; border: 1px solid var(--line); border-radius: 999px; display: flex; margin-top: 36px; max-width: 610px; padding: 7px; }
    .search input { background: transparent; border: 0; flex: 1; min-width: 0; outline: 0; padding: 0 18px; }
    .section-heading { align-items: end; display: flex; justify-content: space-between; margin-bottom: 28px; }
    .section-heading h2 { font-size: clamp(2rem, 4vw, 3rem); margin: 6px 0 0; }
    .section-heading > span { color: var(--muted); font-size: .9rem; }
    .event-grid { display: grid; gap: 24px; grid-template-columns: repeat(3, 1fr); }
    .event-card { background: var(--card); border: 1px solid var(--line); border-radius: 24px; color: var(--ink); overflow: hidden; transition: .25s ease; }
    .event-card:hover { box-shadow: 0 20px 50px rgba(27, 41, 35, .1); transform: translateY(-5px); }
    .event-art { background: #f0a184; color: #532315; display: grid; height: 210px; padding: 22px; place-content: center; position: relative; text-align: center; }
    .event-art.alt { background: #b9d9c3; color: #194c32; }
    .event-art.dark { background: #263a34; color: #e7f2ea; }
    .event-art > span { font-size: .75rem; font-weight: 800; letter-spacing: .12em; position: absolute; right: 20px; text-transform: uppercase; top: 18px; }
    .event-art strong { font-family: var(--display); font-size: 5rem; line-height: .8; }
    .event-art small { font-size: 1rem; font-weight: 800; text-transform: uppercase; }
    .event-copy { padding: 24px; }
    .event-copy h3 { font-size: 1.35rem; margin: 14px 0 8px; }
    .event-copy p { color: var(--muted); display: -webkit-box; line-height: 1.5; overflow: hidden; -webkit-box-orient: vertical; -webkit-line-clamp: 2; }
    .event-meta { border-top: 1px solid var(--line); color: var(--muted); display: grid; font-size: .82rem; gap: 5px; margin-top: 20px; padding-top: 18px; }
    @media (max-width: 900px) { .event-grid { grid-template-columns: repeat(2, 1fr); } }
    @media (max-width: 600px) { .event-grid { grid-template-columns: 1fr; } .hero::after { display: none; } .section-heading > span { display: none; } }
  `],
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
