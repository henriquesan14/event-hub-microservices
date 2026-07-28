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
