import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { email, form, FormField, minLength, required } from '@angular/forms/signals';
import { RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { AuthStore } from '../../core/auth/auth.store';

@Component({
  imports: [FormField, RouterLink],
  template: `
    <section class="page">
      <div class="card form-card">
        <span class="eyebrow">Sua próxima experiência</span>
        <h1>Crie sua conta</h1>
        <p>Reserve ingressos e acompanhe tudo sem complicação.</p>

        @if (error()) { <div class="alert error">{{ error() }}</div> }
        @if (success()) {
          <div class="alert success">
            Conta criada. Enviamos um link de confirmação para seu e-mail.
          </div>
        }

        <form (submit)="submit($event)">
          <div class="field">
            <label for="name">Nome</label>
            <input id="name" autocomplete="name" [formField]="registerForm.name" />
          </div>
          <div class="field">
            <label for="email">E-mail</label>
            <input id="email" type="email" autocomplete="email" [formField]="registerForm.email" />
          </div>
          <div class="field">
            <label for="password">Senha</label>
            <input id="password" type="password" autocomplete="new-password" [formField]="registerForm.password" />
          </div>
          <div class="form-actions">
            <a class="form-link" routerLink="/entrar">Já tenho uma conta</a>
            <button class="primary-button" type="submit" [disabled]="loading() || success()">
              {{ loading() ? 'Criando…' : 'Criar conta' }}
            </button>
          </div>
        </form>
      </div>
    </section>
  `,
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class RegisterPage {
  private readonly auth = inject(AuthStore);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly success = signal(false);
  readonly model = signal({ name: '', email: '', password: '' });
  readonly registerForm = form(this.model, schema => {
    required(schema.name, { message: 'Informe seu nome.' });
    minLength(schema.name, 2, { message: 'Informe ao menos 2 caracteres.' });
    required(schema.email, { message: 'Informe o e-mail.' });
    email(schema.email, { message: 'Informe um e-mail válido.' });
    required(schema.password, { message: 'Informe a senha.' });
    minLength(schema.password, 8, { message: 'Use ao menos 8 caracteres.' });
  });

  submit(event: Event): void {
    event.preventDefault();
    this.error.set('');
    this.loading.set(true);
    const value = this.model();
    this.auth.register(value.name, value.email, value.password).pipe(
      finalize(() => this.loading.set(false)),
    ).subscribe({
      next: () => this.success.set(true),
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }
}
