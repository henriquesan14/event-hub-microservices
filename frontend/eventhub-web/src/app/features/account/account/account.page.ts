import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { email, form, FormField, minLength, required } from '@angular/forms/signals';
import { finalize, forkJoin } from 'rxjs';
import { apiErrorMessage } from '../../../core/api/api-error';
import { EventHubApi } from '../../../core/api/eventhub-api.service';
import { Order, Payment } from '../../../core/api/models';
import { AuthStore } from '../../../core/auth/auth.store';

@Component({
  imports: [CurrencyPipe, DatePipe, FormField, RouterLink],
  templateUrl: './account.page.html',
  styleUrl: './account.page.scss',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccountPage {
  private readonly api = inject(EventHubApi);
  private readonly reservationId = inject(ActivatedRoute).snapshot.queryParamMap.get('reservation');
  protected readonly auth = inject(AuthStore);
  readonly orders = signal<Order[]>([]);
  readonly payments = signal<Payment[]>([]);
  readonly adminPayments = signal<Payment[]>([]);
  readonly refundReasons = signal<Record<string, string>>({});
  readonly refundingPaymentId = signal<string | null>(null);
  readonly refundError = signal('');
  readonly refundSuccess = signal('');
  private readonly eventNames = signal<Record<string, string>>({});
  private readonly eventDates = signal<Record<string, string>>({});
  readonly loading = signal(true);
  readonly savingProfile = signal(false);
  readonly savingPassword = signal(false);
  readonly profileError = signal('');
  readonly profileSuccess = signal('');
  readonly passwordError = signal('');
  readonly passwordSuccess = signal('');
  readonly profileModel = signal({
    name: this.auth.user()?.name ?? '',
    email: this.auth.user()?.email ?? '',
  });
  readonly profileForm = form(this.profileModel, schema => {
    required(schema.name);
    minLength(schema.name, 2);
    required(schema.email);
    email(schema.email);
  });
  readonly passwordModel = signal({
    currentPassword: '',
    newPassword: '',
    confirmPassword: '',
  });
  readonly passwordForm = form(this.passwordModel, schema => {
    required(schema.currentPassword);
    required(schema.newPassword);
    minLength(schema.newPassword, 8);
    required(schema.confirmPassword);
  });
  readonly orderLabels = ['Aguardando pagamento', 'Pago', 'Falha no pagamento', 'Cancelado', 'Expirado', 'Reembolsado'];
  readonly paymentLabels = ['Pendente', 'Aprovado', 'Falhou', 'Cancelado', 'Expirado', 'Reembolsado', 'Estorno em processamento'];
  private refreshAttempts = 0;

  constructor() {
    const user = this.auth.user();
    if (user) {
      this.profileModel.set({ name: user.name, email: user.email });
    }
    this.load();
    if (this.auth.isAdmin()) this.loadAdminPayments();
  }

  saveProfile(event: Event): void {
    event.preventDefault();
    this.profileError.set('');
    this.profileSuccess.set('');
    this.savingProfile.set(true);
    const previousEmail = this.auth.user()?.email;
    const value = this.profileModel();

    this.api.updateProfile(value.name, value.email).pipe(
      finalize(() => this.savingProfile.set(false)),
    ).subscribe({
      next: user => {
        this.auth.setUser(user);
        this.profileSuccess.set(previousEmail !== user.email
          ? 'Perfil atualizado. Confirme o novo endereço pelo e-mail enviado.'
          : 'Perfil atualizado com sucesso.');
      },
      error: err => this.profileError.set(apiErrorMessage(err)),
    });
  }

  savePassword(event: Event): void {
    event.preventDefault();
    this.passwordError.set('');
    this.passwordSuccess.set('');
    const value = this.passwordModel();
    if (value.newPassword !== value.confirmPassword) {
      this.passwordError.set('A confirmação não corresponde à nova senha.');
      return;
    }

    this.savingPassword.set(true);
    this.api.changePassword(value.currentPassword, value.newPassword).pipe(
      finalize(() => this.savingPassword.set(false)),
    ).subscribe({
      next: () => {
        this.passwordSuccess.set('Senha alterada com sucesso.');
        this.passwordModel.set({
          currentPassword: '',
          newPassword: '',
          confirmPassword: '',
        });
      },
      error: err => this.passwordError.set(apiErrorMessage(err)),
    });
  }

  paymentForOrder(orderId: string): Payment | undefined {
    return this.payments().find(payment => payment.orderId === orderId);
  }

  isPaymentPending(status: string | number): boolean {
    return status === 0 || status === 'Pending';
  }

  isApprovedPayment(status: string | number): boolean {
    return status === 1 || status === 'Approved';
  }

  billingTypeLabel(billingType?: string): string {
    switch (billingType?.toUpperCase()) {
      case 'PIX': return 'PIX';
      case 'BOLETO': return 'Boleto';
      case 'CREDIT_CARD': return 'Cartão de crédito';
      case 'DEBIT_CARD': return 'Cartão de débito';
      case 'UNDEFINED': return 'Escolhido no Asaas';
      default: return 'Não informado';
    }
  }

  setRefundReason(paymentId: string, event: Event): void {
    const reason = (event.target as HTMLInputElement).value;
    this.refundReasons.update(reasons => ({ ...reasons, [paymentId]: reason }));
  }

  refund(payment: Payment): void {
    const reason = this.refundReasons()[payment.id]?.trim() ?? '';
    this.refundError.set('');
    this.refundSuccess.set('');
    if (!reason) {
      this.refundError.set('Informe o motivo do estorno.');
      return;
    }
    if (!window.confirm(`Confirma o estorno de ${payment.amount.toFixed(2)} ${payment.currency}?`)) return;

    this.refundingPaymentId.set(payment.id);
    this.api.refundPayment(payment.id, reason).pipe(
      finalize(() => this.refundingPaymentId.set(null)),
    ).subscribe({
      next: updated => {
        this.adminPayments.update(items => items.map(item => item.id === updated.id ? updated : item));
        this.refundSuccess.set('Estorno solicitado. Aguardando a confirmação do Asaas.');
        this.refundReasons.update(reasons => ({ ...reasons, [payment.id]: '' }));
        this.pollRefund(payment.id);
      },
      error: err => this.refundError.set(apiErrorMessage(err)),
    });
  }

  label(status: string | number, labels: string[]): string {
    return labels[Number(status)] ?? String(status);
  }

  isPending(status: string | number): boolean {
    return status === 0 || status === 'PendingPayment';
  }

  eventName(order: Order): string {
    const item = order.items[0];
    return item?.eventName || this.eventNames()[item?.eventId ?? ''] || 'Pedido';
  }

  eventStartsAt(order: Order): string | undefined {
    const item = order.items[0];
    return item?.eventStartsAt || this.eventDates()[item?.eventId ?? ''];
  }

  private load(): void {
    forkJoin({
      orders: this.api.orders(),
      payments: this.api.payments(),
    }).subscribe({
      next: data => {
        this.orders.set(data.orders);
        this.loadMissingEventNames(data.orders);
        this.payments.set(data.payments);
        this.loading.set(false);
        this.scheduleRefreshIfPaymentIsBeingPrepared();
      },
      error: () => this.loading.set(false),
    });
  }

  private loadMissingEventNames(orders: Order[]): void {
    const ids = [...new Set(orders
      .map(order => order.items[0])
      .filter(item => item && (!item.eventName || !item.eventStartsAt))
      .map(item => item.eventId))];
    for (const id of ids) {
      this.api.event(id).subscribe({
        next: event => {
          this.eventNames.update(names => ({ ...names, [id]: event.title }));
          this.eventDates.update(dates => ({ ...dates, [id]: event.startsAt }));
        },
      });
    }
  }

  private loadAdminPayments(): void {
    this.api.adminPayments().subscribe({
      next: payments => this.adminPayments.set(payments),
      error: err => this.refundError.set(apiErrorMessage(err)),
    });
  }

  private pollRefund(paymentId: string, attempt = 0): void {
    if (attempt >= 15) return;
    setTimeout(() => this.api.adminPayments().subscribe({
      next: payments => {
        this.adminPayments.set(payments);
        const payment = payments.find(item => item.id === paymentId);
        if (payment?.status === 5 || payment?.status === 'Refunded') {
          this.refundSuccess.set('Estorno confirmado pelo Asaas.');
          this.load();
          return;
        }
        this.pollRefund(paymentId, attempt + 1);
      },
    }), 2000);
  }

  private scheduleRefreshIfPaymentIsBeingPrepared(): void {
    const expectedOrderHasNotArrived = this.reservationId !== null
      && !this.orders().some(order => order.reservationId === this.reservationId);
    const hasPaymentBeingPrepared = this.orders().some(order =>
      this.isPending(order.status) && !this.paymentForOrder(order.id));

    if ((!expectedOrderHasNotArrived && !hasPaymentBeingPrepared) || this.refreshAttempts >= 10) return;

    this.refreshAttempts++;
    setTimeout(() => this.load(), 1500);
  }
}
