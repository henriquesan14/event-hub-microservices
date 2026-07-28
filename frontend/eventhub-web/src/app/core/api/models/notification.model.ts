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
