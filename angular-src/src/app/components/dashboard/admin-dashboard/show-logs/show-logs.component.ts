import { Component, OnInit } from '@angular/core';
import { DataService } from '../../../../services/data/data.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LogEntry } from '../../../../models/log-models';

type LogFileType = 'sql' | 'backend' | 'settingsManager';
type SeverityLevel = 'DEBUG' | 'INFO' | 'WARN' | 'ERROR' | 'CRITICAL'; // Changed WARNING to WARN

@Component({
  selector: 'app-show-logs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './show-logs.component.html',
  styleUrls: ['./show-logs.component.css']
})
export class ShowLogsComponent implements OnInit {
  logs: LogEntry[] = []; // Logs stored as LogEntry objects
  filteredLogs: LogEntry[] = []; // Logs after applying checkbox filters
  logFileTypes: LogFileType[] = ['sql', 'backend', 'settingsManager'];
  selectedLogFileType: LogFileType = 'backend';

  severityLevels: SeverityLevel[] = ['DEBUG', 'INFO', 'WARN', 'ERROR', 'CRITICAL']; // Updated severity level names
  selectedFetchSeverityLevel: SeverityLevel = 'DEBUG'; // Dropdown for fetching logs
  selectedFilterSeverityLevels: SeverityLevel[] = []; // Checkboxes for filtering fetched logs

  startLine: number = 1;
  lineCount: number = 5;
  reverseLogs: boolean = false;
  isLoading: boolean = false;
  errorMessage: string | null = null;

  constructor(private dataService: DataService) {}

  ngOnInit(): void {
    // Initialize with generated test logs
    //const testLogs = this.generateTestLogs(50); // Generate 50 test logs
    //this.logs = testLogs.map(this.parseLog).filter(log => log !== null) as LogEntry[];
    //this.applySeverityFilter(); // Apply the severity filter after generating logs
  }

    // Generate test logs for testing purposes
    private generateTestLogs(count: number): string[] {
      const logLevels = ['INFO', 'DEBUG', 'WARN', 'ERROR', 'CRITICAL']; // Changed WARNING to WARN in log generation
      const entities = ['Option|options', 'Question|questions', 'QuestionTemplate|question_templates'];
      const actions = [
        'constructed',
        '_configure_property(template, _RelationshipDeclared)',
        '_configure_property(options, _RelationshipDeclared)',
        '_configure_property(id, Column)',
        'Identified primary key columns',
        'Created new connection',
        'BEGIN (implicit)',
        'PRAGMA main.table_info("options")',
        'Col (\'cid\', \'name\', \'type\', \'notnull\', \'dflt_value\', \'pk\')'
      ];
    
      const logs: string[] = [];
    
      const generateTimestamp = (): string => {
        const date = new Date();
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        const hours = String(date.getHours()).padStart(2, '0');
        const minutes = String(date.getMinutes()).padStart(2, '0');
        const seconds = String(date.getSeconds()).padStart(2, '0');
    
        return `[${year}-${month}-${day} ${hours}:${minutes}:${seconds}]`;
      };
    
      for (let i = 0; i < count; i++) {
        const timestamp = generateTimestamp();
        const logLevel = logLevels[Math.floor(Math.random() * logLevels.length)];
        const entity = entities[Math.floor(Math.random() * entities.length)];
        const action = actions[Math.floor(Math.random() * actions.length)];
    
        const logMessage = `${timestamp} [${logLevel}] sqlalchemy.orm.mapper.Mapper: (${entity}) ${action}`;
        logs.push(logMessage);
      }
    
      return logs;
    }

  // Fetch logs based on the selected severity level and other params
  fetchLogs(): void {
    this.isLoading = true;
    this.errorMessage = null;

    // Fetch logs using the selected severity level from the dropdown
    this.dataService.getLogs(this.selectedFetchSeverityLevel, this.selectedLogFileType, this.startLine, this.lineCount, this.reverseLogs).subscribe({
      next: (data: string[]) => {
        // Convert the raw log strings into LogEntry objects
        this.logs = data.map(this.parseLog).filter(log => log !== null) as LogEntry[];
        this.applySeverityFilter(); // Apply checkbox filters after fetching
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Error fetching logs';
        this.isLoading = false;
      },
    });
  }

  // Parse a log string into a LogEntry object
  parseLog(logLine: string): LogEntry | null {
    const logRegex = /^\[([^\]]+)\] \[([A-Z]+)\] ([^:]+): (.+)$/;
    const match = logLine.match(logRegex);

    if (match) {
      const [, timestamp, severity, source, message] = match;
      return {
        timestamp,
        severity: severity as SeverityLevel,  // Cast severity to the SeverityLevel type
        source,
        message
      };
    }

    return null;  // Return null if the log entry doesn't match the expected format
  }

  // Apply the filter using the selected severity levels from checkboxes
  applySeverityFilter(): void {
    if (this.selectedFilterSeverityLevels.length === 0) {
      this.filteredLogs = this.logs; // No filter applied if no checkboxes are selected
    } else {
      this.filteredLogs = this.logs.filter(log => {
        return this.selectedFilterSeverityLevels.includes(log.severity);
      });
    }
  }

  // Handle checkbox change for severity level filtering
  onSeverityFilterChange(event: Event, severity: SeverityLevel): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      if (!this.selectedFilterSeverityLevels.includes(severity)) {
        this.selectedFilterSeverityLevels.push(severity);
      }
    } else {
      this.selectedFilterSeverityLevels = this.selectedFilterSeverityLevels.filter(level => level !== severity);
    }
    this.applySeverityFilter(); // Apply the filter whenever a checkbox is changed
  }

  onUpdateLines(): void {
    if (this.startLine < 1) {
      this.startLine = 1;
    }

    if (this.lineCount > 0) {
      this.fetchLogs();
    } else {
      this.errorMessage = 'Limit must be greater than 0.';
    }
  }

  onPageChange(isNext: boolean): void {
    if (isNext) {
      this.startLine += this.lineCount;
    } else if (this.startLine > 1) {
      this.startLine = Math.max(1, this.startLine - this.lineCount);
    }

    this.fetchLogs();
  }
}
