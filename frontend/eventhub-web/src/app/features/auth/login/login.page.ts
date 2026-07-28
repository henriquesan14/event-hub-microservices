import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { email, form, FormField, minLength, required } from '@angular/forms/signals';
import { ActivatedRoute, Router, RouterLink } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorMessage } from '../../../core/api/api-error';
import { AuthStore } from '../../../core/auth/auth.store';

@Component({
  imports: [FormField, RouterLink],
  templateUrl: './login.page.html',
  styleUrl: './login.page.scss',
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
