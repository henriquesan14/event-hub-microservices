export interface OrderItem {
  id: string;
  ticketTypeId: string;
  eventId: string;
  eventName: string;
  eventStartsAt?: string;
  name: string;
  unitPrice: number;
  currency: string;
  quantity: number;
  total: number;
}
