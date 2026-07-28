import { HttpClient, HttpParams } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import {
  AdmissionTicket,
  EventModel,
  Notification,
  Order,
  PaginatedResult,
  Payment,
  Reservation,
  TicketType,
  User,
} from './models';

@Injectable({ providedIn: 'root' })
export class EventHubApi {
  private readonly http = inject(HttpClient);

  events(search = '', pageNumber = 1) {
    let params = new HttpParams().set('pageNumber', pageNumber).set('pageSize', 12);
    if (search.trim()) params = params.set('title', search.trim());
    return this.http.get<PaginatedResult<EventModel>>('/api/events', { params });
  }

  event(id: string) {
    return this.http.get<EventModel>(`/api/events/${id}`);
  }

  createEvent(body: Omit<EventModel, 'id' | 'status' | 'organizerId' | 'createdAt' | 'createdByName'>) {
    return this.http.post<EventModel>('/api/events', body);
  }

  updateEvent(id: string, body: Omit<EventModel, 'id' | 'status' | 'organizerId' | 'createdAt' | 'createdByName'>) {
    return this.http.put<EventModel>(`/api/events/${id}`, body);
  }

  publishEvent(id: string) {
    return this.http.post<EventModel>(`/api/events/${id}/publish`, {});
  }

  cancelEvent(id: string) {
    return this.http.post<EventModel>(`/api/events/${id}/cancel`, {});
  }

  ticketTypes(eventId: string) {
    return this.http.get<TicketType[]>(`/api/events/${eventId}/tickets`);
  }

  createTicketType(eventId: string, body: {
    name: string;
    description: string;
    price: number;
    currency: string;
    totalQuantity: number;
    salesStart: string;
    salesEnd: string;
  }) {
    return this.http.post<TicketType>(`/api/events/${eventId}/tickets`, body);
  }

  updateTicketType(id: string, body: {
    name: string;
    description: string;
    price: number;
    currency: string;
    totalQuantity: number;
    salesStart: string;
    salesEnd: string;
    active: boolean;
  }) {
    return this.http.put<TicketType>(`/api/tickets/${id}`, body);
  }

  deleteTicketType(id: string) {
    return this.http.delete<void>(`/api/tickets/${id}`);
  }

  reserve(ticketTypeId: string, quantity: number) {
    return this.http.post<Reservation>(`/api/tickets/${ticketTypeId}/reservations`, { quantity });
  }

  reservation(id: string) {
    return this.http.get<Reservation>(`/api/reservations/${id}`);
  }

  orders() {
    return this.http.get<Order[]>('/api/orders/me');
  }

  orderByReservation(reservationId: string) {
    return this.http.get<Order>(`/api/orders/by-reservation/${reservationId}`);
  }

  cancelOrder(id: string) {
    return this.http.post<void>(`/api/orders/${id}/cancel`, {});
  }

  payments() {
    return this.http.get<Payment[]>('/api/payments/me');
  }

  checkout(paymentId: string, body: {
    name: string;
    email: string;
    cpfCnpj: string;
    mobilePhone: string;
    billingType: string;
  }) {
    return this.http.post<Payment>(`/api/payments/${paymentId}/checkout`, body);
  }

  admissionTickets() {
    return this.http.get<AdmissionTicket[]>('/api/admission/tickets/me');
  }

  notifications() {
    return this.http.get<Notification[]>('/api/notifications/me');
  }

  unreadCount() {
    return this.http.get<number>('/api/notifications/unread-count');
  }

  markNotificationRead(id: string) {
    return this.http.post<void>(`/api/notifications/${id}/read`, {});
  }

  markAllNotificationsRead() {
    return this.http.post<void>('/api/notifications/read-all', {});
  }

  updateProfile(name: string, email: string) {
    return this.http.put<User>('/api/users/me', { name, email });
  }

  changePassword(currentPassword: string, newPassword: string) {
    return this.http.put<void>('/api/users/me/password', {
      currentPassword,
      newPassword,
    });
  }
}
