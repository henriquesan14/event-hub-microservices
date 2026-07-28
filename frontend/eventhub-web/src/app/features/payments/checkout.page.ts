import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, required } from '@angular/forms/signals';
import { ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { EventHubApi } from '../../core/api/eventhub-api.service';
import { Payment } from '../../core/api/models';
import { AuthStore } from '../../core/auth/auth.store';

@Component({
  imports: [CurrencyPipe, DatePipe, FormField],
  template: `
    <section class="page checkout-grid">
      <div>
        <span class="eyebrow">Checkout seguro</span>
        <h1>Finalize seu pagamento.</h1>
        <p class="lead">Você será direcionado ao ambiente do Asaas para concluir a cobrança.</p>
        @if (payment()) {
          <div class="summary card">
            <span>Valor total</span>
            <strong>{{ payment()!.amount | currency:payment()!.currency }}</strong>
            <small>Reserva válida até {{ payment()!.expiresAt | date:'dd/MM/yyyy · HH:mm' }}</small>
          </div>
        }
      </div>

      <div class="card form-card checkout-card">
        <h2>Dados do pagador</h2>
        @if (error()) { <div class="alert error">{{ error() }}</div> }
        @if (invoiceUrl()) {
          <div class="alert success">Cobrança criada com sucesso.</div>
          <a class="primary-button full" [href]="invoiceUrl()" target="_blank" rel="noopener">Abrir pagamento no Asaas</a>
        } @else {
          <form (submit)="submit($event)">
            <div class="field"><label for="name">Nome completo</label><input id="name" [formField]="checkoutForm.name" /></div>
            <div class="field"><label for="email">E-mail</label><input id="email" type="email" [formField]="checkoutForm.email" /></div>
            <div class="field"><label for="cpf">CPF ou CNPJ</label><input id="cpf" inputmode="numeric" [formField]="checkoutForm.cpfCnpj" /></div>
            <div class="field"><label for="phone">Celular</label><input id="phone" inputmode="tel" [formField]="checkoutForm.mobilePhone" /></div>
            <div class="field">
              <label for="billing">Forma de pagamento</label>
              <select id="billing" [formField]="checkoutForm.billingType">
                <option value="UNDEFINED">Escolher no Asaas</option>
                <option value="PIX">Pix</option>
                <option value="CREDIT_CARD">Cartão de crédito</option>
                <option value="BOLETO">Boleto</option>
              </select>
            </div>
            <button class="primary-button full" type="submit" [disabled]="loading()">
              {{ loading() ? 'Criando cobrança…' : 'Continuar para o pagamento' }}
            </button>
          </form>
        }
      </div>
    </section>
  `,
  styles: [`
    .checkout-grid { display: grid; gap: clamp(30px, 8vw, 100px); grid-template-columns: 1fr 1fr; }
    h1 { font-size: clamp(2.7rem, 6vw, 5rem); line-height: 1; margin: 12px 0 20px; }
    .summary { display: grid; gap: 8px; margin-top: 38px; max-width: 420px; padding: 24px; }
    .summary span, .summary small { color: var(--muted); }
    .summary strong { font-family: var(--display); font-size: 2rem; }
    .checkout-card { margin: 0; }
    .checkout-card h2 { font-size: 1.8rem; }
    .full { width: 100%; }
    @media (max-width: 780px) { .checkout-grid { grid-template-columns: 1fr; } }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CheckoutPage {
  private readonly api = inject(EventHubApi);
  private readonly auth = inject(AuthStore);
  private readonly paymentId = inject(ActivatedRoute).snapshot.paramMap.get('paymentId')!;
  readonly payment = signal<Payment | null>(null);
  readonly loading = signal(false);
  readonly error = signal('');
  readonly invoiceUrl = signal('');
  readonly model = signal({
    name: this.auth.user()?.name ?? '',
    email: this.auth.user()?.email ?? '',
    cpfCnpj: '',
    mobilePhone: '',
    billingType: 'UNDEFINED',
  });
  readonly checkoutForm = form(this.model, schema => {
    required(schema.name);
    required(schema.email);
    required(schema.cpfCnpj);
  });

  constructor() {
    this.api.payments().subscribe({
      next: values => this.payment.set(values.find(item => item.id === this.paymentId) ?? null),
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }

  submit(event: Event): void {
    event.preventDefault();
    this.loading.set(true);
    this.error.set('');
    this.api.checkout(this.paymentId, this.model()).pipe(
      finalize(() => this.loading.set(false)),
    ).subscribe({
      next: payment => {
        this.payment.set(payment);
        this.invoiceUrl.set(payment.invoiceUrl ?? '');
      },
      error: err => this.error.set(apiErrorMessage(err)),
    });
  }
}
