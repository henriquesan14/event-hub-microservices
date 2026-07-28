import type { UserRole } from './user-role.type';

export interface User {
  id: string;
  name: string;
  email: string;
  role: UserRole;
  emailConfirmed: boolean;
  createdAt?: string;
  createdByName?: string;
}
