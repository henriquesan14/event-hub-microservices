import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { apiErrorMessage } from '../../core/api/api-error';

@Component({
  imports: [RouterLink],
  template: `
    <section class="page"><div class="card form-card">
      <span class="eyebrow">Confirmação</span>
      <h1>Confirme seu e-mail</h1>
      @if (loading()) {
        <p>Validando seu link…</p>
      } @else if (success()) {
        <div class="alert success">E-mail confirmado. Sua conta está pronta.</div>
        <a class="primary-button" routerLink="/entrar">Entrar</a>
      } @else {
        <div class="alert error">{{ error() }}</div>
      }
    </div></section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ConfirmEmailPage {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);
  readonly loading = signal(true);
  readonly success = signal(false);
  readonly error = signal('');

  constructor() {
    const token = this.route.snapshot.queryParamMap.get('token');
    if (!token) {
      this.loading.set(false);
      this.error.set('O token de confirmação não foi informado.');
      return;
    }
    this.http.post('/api/auth/confirm-email', { token }).subscribe({
      next: () => {
        this.success.set(true);
        this.loading.set(false);
      },
      error: err => {
        this.error.set(apiErrorMessage(err));
        this.loading.set(false);
      },
    });
  }
}
