export interface SettingMetadata {
  default?: any;
  minValue?: number;
  maxValue?: number;
  canBeEmpty?: boolean;
  description?: string;
  allowedValues?: any[];
}

export interface Settings {
  [key: string]: any | Settings; // Recursive definition to handle nested settings
}

export interface Metadata {
  [key: string]: SettingMetadata | Metadata; // Recursive definition to handle nested metadata
}