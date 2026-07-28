export interface RealtimeNotificationMessage {
  type: string;
  resourceId: string;
  title: string;
  message: string;
  actionUrl?: string;
}
