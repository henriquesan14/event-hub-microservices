export type UserRole = 'Admin' | 'User' | 'Organizer' | 0 | 1 | 2;
export type EventStatus = 'Draft' | 'Published' | 'Cancelled' | 'Finished' | 0 | 1 | 2 | 3;

export interface ApiProblem {
  title?: string;
  detail?: string;
  errors?: Record<string, string[]>;
  status?: number;
}

export interface AuthResponse {
  userId: string;
  name: string;
  role: UserRole;
}

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  emailConfirmed: boolean;
  createdAt?: string;
  createdByName?: string;
}

export interface Address {
  street: string;
  number: string;
  district: string;
  city: string;
  state: string;
  country: string;
  zipCode: string;
}

export interface EventModel {
  id: string;
  title: string;
  description: string;
  address: Address;
  startsAt: string;
  endsAt: string;
  status: EventStatus;
  organizerId: string;
  createdAt?: string;
  createdByName?: string;
}

export interface PaginatedResult<T> {
  pageIndex: number;
  pageSize: number;
  count: number;
  data: T[];
}

export interface TicketType {
  id: string;
  eventId: string;
  name: string;
  description: string;
  price: number;
  currency: string;
  totalQuantity: number;
  availableQuantity: number;
  salesStart: string;
  salesEnd: string;
  status: string | number;
}

export interface Reservation {
  id: string;
  userId: string;
  ticketTypeId: string;
  eventId: string;
  ticketName: string;
  unitPrice: number;
  currency: string;
  quantity: number;
  expiresAt: string;
  status: string | number;
}

export interface OrderItem {
  id: string;
  ticketTypeId: string;
  eventId: string;
  name: string;
  unitPrice: number;
  currency: string;
  quantity: number;
  total: number;
}

export interface Order {
  id: string;
  userId: string;
  reservationId: string;
  status: string | number;
  total: number;
  currency: string;
  expiresAt: string;
  paymentId?: string;
  items: OrderItem[];
  createdAt?: string;
}

export interface Payment {
  id: string;
  orderId: string;
  reservationId: string;
  userId: string;
  amount: number;
  currency: string;
  status: string | number;
  expiresAt: string;
  approvedAt?: string;
  providerReference?: string;
  billingType?: string;
  invoiceUrl?: string;
  failureReason?: string;
  createdAt?: string;
}

export interface AdmissionTicket {
  id: string;
  orderId: string;
  reservationId: string;
  eventId: string;
  ticketTypeId: string;
  ticketName: string;
  code: string;
  status: string | number;
  issuedAt: string;
  checkedInAt?: string;
}

export interface Notification {
  id: string;
  type: string | number;
  title: string;
  message: string;
  resourceId: string;
  isRead: boolean;
  readAt?: string;
  createdAt?: string;
}
