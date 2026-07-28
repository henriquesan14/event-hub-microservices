import type { Address } from './address.model';
import type { EventStatus } from './event-status.type';

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
