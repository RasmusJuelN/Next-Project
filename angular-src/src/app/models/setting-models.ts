// Define the types for DatabaseSettings and AuthSettings

export type DatabaseType = 'sqlite' | 'mysql' | 'mssql';

// Database settings interface
export interface DatabaseSettings {
  databaseType: DatabaseType;
  databaseDriver?: string | null;
  dbName: string;
  host?: string | null;
  user?: string | null;
  password?: string | null;
  port?: number | null;
  timeout?: number | null;
  useSsl: boolean;
  sslCertFile?: string | null;
  sslKeyFile?: string | null;
  sslCaCertFile?: string | null;
  maxConnections?: number | null;
  minConnections?: number | null;
}

// Auth settings interface
export interface AuthSettings {
  secretKey?: string | null;
  algorithm: string;
  accessTokenExpireMinutes: number;
  domain: string;
  ldapServer: string;
  ldapBaseDn: string;
  scopes: Record<string, string>; // Using a Record type for key-value pairs
}

// Main app settings interface that includes both AuthSettings and DatabaseSettings
export interface AppSettings {
  auth: AuthSettings;
  database: DatabaseSettings;
}