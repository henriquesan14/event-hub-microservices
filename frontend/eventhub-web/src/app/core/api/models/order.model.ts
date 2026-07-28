import type { OrderItem } from './order-item.model';

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
