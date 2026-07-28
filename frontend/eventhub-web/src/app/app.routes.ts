import { Routes } from '@angular/router';
import { authGuard } from './core/auth/auth.guard';
import { guestGuard } from './core/auth/guest.guard';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/events/event-list/event-list.page').then(m => m.EventListPage),
    title: 'Eventos | EventHub',
  },
  {
    path: 'eventos/novo',
    canActivate: [authGuard],
    loadComponent: () => import('./features/events/event-editor/event-editor.page').then(m => m.EventEditorPage),
    title: 'Criar evento | EventHub',
  },
  {
    path: 'eventos/:id/editar',
    canActivate: [authGuard],
    loadComponent: () => import('./features/events/event-editor/event-editor.page').then(m => m.EventEditorPage),
    title: 'Editar evento | EventHub',
  },
  {
    path: 'eventos/:id/ingressos',
    canActivate: [authGuard],
    loadComponent: () => import('./features/events/ticket-manager/ticket-manager.page').then(m => m.TicketManagerPage),
    title: 'Gerenciar ingressos | EventHub',
  },
  {
    path: 'eventos/:id',
    loadComponent: () => import('./features/events/event-detail/event-detail.page').then(m => m.EventDetailPage),
    title: 'Detalhes do evento | EventHub',
  },
  {
    path: 'entrar',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/login/login.page').then(m => m.LoginPage),
    title: 'Entrar | EventHub',
  },
  {
    path: 'cadastro',
    canActivate: [guestGuard],
    loadComponent: () => import('./features/auth/register/register.page').then(m => m.RegisterPage),
    title: 'Criar conta | EventHub',
  },
  {
    path: 'esqueci-senha',
    loadComponent: () => import('./features/auth/forgot-password/forgot-password.page').then(m => m.ForgotPasswordPage),
    title: 'Recuperar senha | EventHub',
  },
  {
    path: 'reset-password',
    loadComponent: () => import('./features/auth/reset-password/reset-password.page').then(m => m.ResetPasswordPage),
    title: 'Redefinir senha | EventHub',
  },
  {
    path: 'confirm-email',
    loadComponent: () => import('./features/auth/confirm-email/confirm-email.page').then(m => m.ConfirmEmailPage),
    title: 'Confirmar e-mail | EventHub',
  },
  {
    path: 'minha-conta',
    canActivate: [authGuard],
    loadComponent: () => import('./features/account/account/account.page').then(m => m.AccountPage),
    title: 'Minha conta | EventHub',
  },
  {
    path: 'meus-ingressos',
    canActivate: [authGuard],
    loadComponent: () => import('./features/account/my-tickets/my-tickets.page').then(m => m.MyTicketsPage),
    title: 'Meus ingressos | EventHub',
  },
  {
    path: 'checkout/:paymentId',
    canActivate: [authGuard],
    loadComponent: () => import('./features/payments/checkout/checkout.page').then(m => m.CheckoutPage),
    title: 'Pagamento | EventHub',
  },
  {
    path: 'notificacoes',
    canActivate: [authGuard],
    loadComponent: () => import('./features/notifications/notifications/notifications.page').then(m => m.NotificationsPage),
    title: 'Notificações | EventHub',
  },
  { path: '**', redirectTo: '' },
];
