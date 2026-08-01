export interface TicketType {
  id: string;
  eventId: string;
  eventName: string;
  eventStartsAt?: string;
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
