import { HttpClient } from '@angular/common/http';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { email, form, FormField, required } from '@angular/forms/signals';
import { finalize } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';

@Component({
  imports: [FormField],
  template: `
    <section class="page"><div class="card form-card">
      <span class="eyebrow">Recuperação</span>
      <h1>Esqueceu a senha?</h1>
      <p>Informe seu e-mail e enviaremos um link seguro.</p>
      @if (message()) { <div class="alert success">{{ message() }}</div> }
      @if (error()) { <div class="alert error">{{ error() }}</div> }
      <form (submit)="submit($event)">
        <div class="field">
          <label for="email">E-mail</label>
          <input id="email" type="email" [formField]="forgotForm.email" />
        </div>
        <button class="primary-button" type="submit" [disabled]="loading()">Enviar link</button>
      </form>
    </div></section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class ForgotPasswordPage {
  private readonly http = inject(HttpClient);
  readonly model = signal({ email: '' });
  readonly forgotForm = form(this.model, schema => {
    required(schema.email);
    email(schema.email);
  });
  readonly loading = signal(false);
  readonly message = signal('');
  readonly error = signal('');

  submit(event: Event): void {
    event.preventDefault();
    this.loading.set(true);
    this.error.set('');
    this.http.post('/api/auth/forgot-password', this.model()).pipe(
      finalize(() => this.loading.set(false)),
    ).subscribe({
      next: () => this.message.set('Se o e-mail estiver cadastrado, o link chegará em instantes.'),
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }
}
