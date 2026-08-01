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
  refundRequestedAt?: string;
  refundedAt?: string;
  refundReason?: string;
  createdAt?: string;
}
