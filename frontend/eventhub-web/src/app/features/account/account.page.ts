import { CurrencyPipe, DatePipe } from '@angular/common';
import { ChangeDetectionStrategy, Component, inject, signal } from '@angular/core';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { email, form, FormField, minLength, required } from '@angular/forms/signals';
import { finalize, forkJoin } from 'rxjs';
import { apiErrorMessage } from '../../core/api/api-error';
import { EventHubApi } from '../../core/api/eventhub-api.service';
import { Order, Payment } from '../../core/api/models';
import { AuthStore } from '../../core/auth/auth.store';

@Component({
  imports: [CurrencyPipe, DatePipe, FormField, RouterLink],
  template: `
    <section class="page">
      <div class="account-heading">
        <div>
          <span class="eyebrow">Área do participante</span>
          <h1>Olá, {{ auth.user()?.name }}.</h1>
          <p>Acompanhe seus pedidos, pagamentos e ingressos.</p>
        </div>
        <a class="primary-button" routerLink="/">Explorar eventos</a>
      </div>

      <div class="stats">
        <div class="card"><span>Pedidos</span><strong>{{ orders().length }}</strong></div>
        <div class="card"><span>Pagamentos</span><strong>{{ payments().length }}</strong></div>
      </div>

      <section class="profile-grid">
        <form class="card profile-card" (submit)="saveProfile($event)">
          <span class="eyebrow">Perfil</span>
          <h2>Dados pessoais</h2>
          <p>Atualize o nome e o endereço de e-mail da sua conta.</p>
          @if (profileError()) { <div class="alert error">{{ profileError() }}</div> }
          @if (profileSuccess()) { <div class="alert success">{{ profileSuccess() }}</div> }
          <div class="field">
            <label for="profile-name">Nome</label>
            <input id="profile-name" autocomplete="name" [formField]="profileForm.name" />
          </div>
          <div class="field">
            <label for="profile-email">E-mail</label>
            <input id="profile-email" type="email" autocomplete="email" [formField]="profileForm.email" />
          </div>
          <button class="primary-button" type="submit" [disabled]="savingProfile()">
            {{ savingProfile() ? 'Salvando…' : 'Salvar perfil' }}
          </button>
        </form>

        <form class="card profile-card" (submit)="savePassword($event)">
          <span class="eyebrow">Segurança</span>
          <h2>Alterar senha</h2>
          <p>Confirme sua senha atual antes de escolher uma nova.</p>
          @if (passwordError()) { <div class="alert error">{{ passwordError() }}</div> }
          @if (passwordSuccess()) { <div class="alert success">{{ passwordSuccess() }}</div> }
          <div class="field">
            <label for="current-password">Senha atual</label>
            <input id="current-password" type="password" autocomplete="current-password"
              [formField]="passwordForm.currentPassword" />
          </div>
          <div class="field">
            <label for="new-password">Nova senha</label>
            <input id="new-password" type="password" autocomplete="new-password"
              [formField]="passwordForm.newPassword" />
          </div>
          <div class="field">
            <label for="confirm-password">Confirme a nova senha</label>
            <input id="confirm-password" type="password" autocomplete="new-password"
              [formField]="passwordForm.confirmPassword" />
          </div>
          <button class="primary-button" type="submit" [disabled]="savingPassword()">
            {{ savingPassword() ? 'Alterando…' : 'Alterar senha' }}
          </button>
        </form>
      </section>

      @if (loading()) {
        <div class="skeleton"></div>
      } @else {
        <section class="account-section">
          <div class="section-title"><h2>Pedidos recentes</h2></div>
          @if (orders().length === 0) {
            <div class="card empty-state">Você ainda não possui pedidos.</div>
          } @else {
            <div class="list">
              @for (order of orders(); track order.id) {
                <article class="card list-item">
                  <div>
                    <span class="status">{{ label(order.status, orderLabels) }}</span>
                    <h3>{{ order.items[0]?.name || 'Pedido' }}</h3>
                    <small>{{ order.createdAt | date:'dd/MM/yyyy · HH:mm' }}</small>
                  </div>
                  <div class="item-actions">
                    <strong>{{ order.total | currency:order.currency }}</strong>
                    @if (paymentForOrder(order.id); as payment) {
                      @if (isPending(order.status) && isPaymentPending(payment.status)) {
                        <a class="primary-button compact" [routerLink]="['/checkout', payment.id]">Pagar agora</a>
                      }
                    } @else if (isPending(order.status)) {
                      <small class="preparing">Preparando pagamento…</small>
                    }
                  </div>
                </article>
              }
            </div>
          }
        </section>

      }
    </section>
  `,
  styles: [`
    .account-heading { align-items: end; display: flex; justify-content: space-between; margin-bottom: 42px; }
    .account-heading h1 { font-size: clamp(2.5rem, 6vw, 4.8rem); margin: 8px 0; }
    .account-heading p { color: var(--muted); }
    .stats { display: grid; gap: 18px; grid-template-columns: repeat(2, 1fr); }
    .stats .card { display: grid; gap: 8px; padding: 24px; }
    .stats span { color: var(--muted); font-size: .85rem; }
    .stats strong { font-family: var(--display); font-size: 2rem; }
    .profile-grid { display: grid; gap: 18px; grid-template-columns: 1fr 1fr; margin-top: 34px; }
    .profile-card { padding: clamp(24px, 4vw, 34px); }
    .profile-card h2 { font-size: 1.7rem; margin: 8px 0; }
    .profile-card > p { color: var(--muted); margin-bottom: 24px; }
    .account-section { margin-top: 52px; }
    .section-title h2 { font-size: 2rem; }
    .list { display: grid; gap: 12px; }
    .list-item { align-items: center; display: flex; justify-content: space-between; padding: 22px 24px; }
    .list-item h3 { margin: 10px 0 4px; }
    .list-item small { color: var(--muted); }
    .item-actions { align-items: end; display: grid; gap: 10px; justify-items: end; }
    .preparing { color: var(--warning) !important; }
    @media (max-width: 700px) { .account-heading { align-items: start; flex-direction: column; gap: 20px; } .stats, .profile-grid { grid-template-columns: 1fr; } .list-item { align-items: start; gap: 20px; } }
  `],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class AccountPage {
  private readonly api = inject(EventHubApi);
  private readonly reservationId = inject(ActivatedRoute).snapshot.queryParamMap.get('reservation');
  protected readonly auth = inject(AuthStore);
  readonly orders = signal<Order[]>([]);
  readonly payments = signal<Payment[]>([]);
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
  private refreshAttempts = 0;

  constructor() {
    const user = this.auth.user();
    if (user) {
      this.profileModel.set({ name: user.name, email: user.email });
    }
    this.load();
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

  label(status: string | number, labels: string[]): string {
    return labels[Number(status)] ?? String(status);
  }

  isPending(status: string | number): boolean {
    return status === 0 || status === 'PendingPayment';
  }

  private load(): void {
    forkJoin({
      orders: this.api.orders(),
      payments: this.api.payments(),
    }).subscribe({
      next: data => {
        this.orders.set(data.orders);
        this.payments.set(data.payments);
        this.loading.set(false);
        this.scheduleRefreshIfPaymentIsBeingPrepared();
      },
      error: () => this.loading.set(false),
    });
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
