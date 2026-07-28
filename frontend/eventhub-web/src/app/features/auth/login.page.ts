import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { email, form, FormField, minLength, required } from '@angular/forms/signals';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { AuthStore } from '../../core/auth/auth.store';

@Component({
  imports: [FormField, RouterLink],
  template: `
    <section class="page">
      <div class="card form-card">
        <span class="eyebrow">Bem-vindo de volta</span>
        <h1>Entre na sua conta</h1>
        <p>Seus eventos, pedidos e ingressos em um só lugar.</p>

        @if (error()) { <div class="alert error">{{ error() }}</div> }

        <form (submit)="submit($event)">
          <div class="field">
            <label for="email">E-mail</label>
            <input id="email" type="email" autocomplete="email" [formField]="loginForm.email" />
          </div>
          <div class="field">
            <label for="password">Senha</label>
            <input id="password" type="password" autocomplete="current-password" [formField]="loginForm.password" />
          </div>
          <div class="form-actions">
            <a class="form-link" routerLink="/esqueci-senha">Esqueci minha senha</a>
            <button class="primary-button" type="submit" [disabled]="loading()">
              {{ loading() ? 'Entrando…' : 'Entrar' }}
            </button>
          </div>
        </form>
      </div>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class LoginPage {
  private readonly auth = inject(AuthStore);
  private readonly router = inject(Router);
  private readonly route = inject(ActivatedRoute);

  readonly loading = signal(false);
  readonly error = signal('');
  readonly model = signal({ email: '', password: '' });
  readonly loginForm = form(this.model, schema => {
    required(schema.email, { message: 'Informe o e-mail.' });
    email(schema.email, { message: 'Informe um e-mail válido.' });
    required(schema.password, { message: 'Informe a senha.' });
    minLength(schema.password, 8, { message: 'A senha deve ter ao menos 8 caracteres.' });
  });

  submit(event: Event): void {
    event.preventDefault();
    this.error.set('');
    this.loading.set(true);
    const value = this.model();

    this.auth.login(value.email, value.password).pipe(
      finalize(() => this.loading.set(false)),
    ).subscribe({
      next: () => void this.router.navigateByUrl(
        this.route.snapshot.queryParamMap.get('returnUrl') ?? '/minha-conta',
      ),
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }
}
