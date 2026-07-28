import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, minLength, required } from '@angular/forms/signals';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';

@Component({
  imports: [FormField, RouterLink],
  template: `
    <section class="page"><div class="card form-card">
      <span class="eyebrow">Nova senha</span>
      <h1>Redefina sua senha</h1>
      <p>Escolha uma senha forte que você ainda não tenha usado.</p>
      @if (success()) {
        <div class="alert success">Senha alterada com sucesso.</div>
        <a class="primary-button" routerLink="/entrar">Entrar</a>
      } @else {
        @if (error()) { <div class="alert error">{{ error() }}</div> }
        <form (submit)="submit($event)">
          <div class="field">
            <label for="password">Nova senha</label>
            <input id="password" type="password" [formField]="resetForm.newPassword" />
          </div>
          <button class="primary-button" type="submit" [disabled]="loading() || !token">Salvar nova senha</button>
        </form>
      }
    </div></section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ResetPasswordPage {
  private readonly http = inject(HttpClient);
  private readonly route = inject(ActivatedRoute);
  readonly token = this.route.snapshot.queryParamMap.get('token') ?? '';
  readonly model = signal({ newPassword: '' });
  readonly resetForm = form(this.model, schema => {
    required(schema.newPassword);
    minLength(schema.newPassword, 8);
  });
  readonly loading = signal(false);
  readonly success = signal(false);
  readonly error = signal(this.token ? '' : 'O token de redefinição não foi informado.');

  submit(event: Event): void {
    event.preventDefault();
    this.loading.set(true);
    this.http.post('/api/auth/reset-password', { token: this.token, ...this.model() }).pipe(
      finalize(() => this.loading.set(false)),
    ).subscribe({
      next: () => this.success.set(true),
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }
}
