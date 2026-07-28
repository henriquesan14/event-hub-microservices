import type { UserRole } from './user-role.type';

export interface AuthResponse {
  userId: string;
  name: string;
  role: UserRole;
}
