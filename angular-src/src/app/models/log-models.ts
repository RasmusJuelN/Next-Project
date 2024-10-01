export interface LogEntry {
    timestamp: string;    // The timestamp of the log, e.g., "2024-09-20 12:34:23"
    severity: 'DEBUG' | 'INFO' | 'WARNING' | 'ERROR' | 'CRITICAL';  // The severity level of the log
    source: string;       // The source of the log, e.g., "sqlalchemy.orm.mapper.Mapper"
    message: string;      // The log message, e.g., "Col ('cid', 'name', 'type', 'notnull', 'dflt_value', 'pk')"
  }
  