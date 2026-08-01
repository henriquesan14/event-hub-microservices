export interface AdmissionTicket {
  id: string;
  orderId: string;
  reservationId: string;
  eventId: string;
  eventName: string;
  eventStartsAt?: string;
  ticketTypeId: string;
  ticketName: string;
  code: string;
  status: string | number;
  issuedAt: string;
  checkedInAt?: string;
}
