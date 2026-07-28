import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { form, FormField, required } from '@angular/forms/signals';
import { ActivatedRoute } from '@angular/router';
import { finalize } from 'rxjs';
import { apiErrorMessage } from '../../../core/api/api-error';
import { EventHubApi } from '../../../core/api/eventhub-api.service';
import { Payment } from '../../../core/api/models';
import { AuthStore } from '../../../core/auth/auth.store';

@Component({
  imports: [CurrencyPipe, DatePipe, FormField],
  templateUrl: './checkout.page.html',
  styleUrl: './checkout.page.scss',
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
