/**
 * Represents an user.
 */
export interface User {
  /** Unique identifier of the user. */
  id: string;

  /** Username used for login. */
  userName: string;

  /** User's full display name. */
  fullName: string;

  /** Role string (should match one of the `Role` enum values). */
  role: Role;
}

/**
 * User roles available in the application.
 *
 * These values map directly to the backend role configuration.
 *
 * @example
 * ```ts
 * if (user.role === Role.Admin) {
 *   // grant admin access
 * }
 * ```
 */

export enum Role {
  Student = 'student',
  Teacher = 'teacher',
  Admin = 'admin',
}
