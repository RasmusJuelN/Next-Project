import { Component, OnInit } from '@angular/core';
import { DataService } from '../../../../services/data/data.service';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

type LogFileType = 'sql' | 'backend' | 'settingsManager';
type LogType = 'info' | 'warn' | 'debug';
@Component({
  selector: 'app-show-logs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './show-logs.component.html',
  styleUrls: ['./show-logs.component.css']
})
export class ShowLogsComponent implements OnInit {
  logs: string[] = [];
  logFileTypes: LogFileType[] = ['sql', 'backend', 'settingsManager'];
  selectedLogFileType: LogFileType = 'backend'; 
  logSeverityTypes: LogType[] = ['info', 'warn', 'debug'];
  selectedSeverityTypes: LogType[] = [];
  startLine: number = 1;
  lineCount: number = 25;
  reverseLogs: boolean = false;
  isLoading: boolean = false;
  errorMessage: string | null = null;

  constructor(private dataService: DataService) {}

  ngOnInit(): void {
    this.logs = this.generateTestLogs(255)
  }

  // Generate test logs for testing purposes
  generateTestLogs(count: number): string[] {
    const logLevels = ['INFO', 'DEBUG'];
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
  
    // Function to generate a random timestamp
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
  
  // Fetch logs based on the selected log type, start line, and limit
  fetchLogs(): void {
    this.isLoading = true;
    this.errorMessage = null;
  
    // Fetch logs using the selected log type, startLine, and lineCount
    this.dataService.getLogs(this.selectedSeverityTypes,this.selectedLogFileType, this.startLine, this.lineCount, this.reverseLogs).subscribe({
      next: (data: string[]) => {
        this.logs = data;
        this.isLoading = false;
      },
      error: (err) => {
        this.errorMessage = 'Error fetching logs';
        this.isLoading = false;
      },
    });
  }
  

  // Update pagination to fetch the next or previous page
  onUpdateLines(): void {
    // Ensure startLine is at least 1, and limit is positive
    if (this.startLine < 1) {
      this.startLine = 1;  // Force it to be 1 if user enters 0 or less
    }
  
    if (this.lineCount > 0) {
      this.fetchLogs();
    } else {
      this.errorMessage = 'Limit must be greater than 0.';
    }
  }

  onSeverityChange(event: Event, severity: LogType): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      // Add the severity type to the selectedSeverityTypes array
      if (!this.selectedSeverityTypes.includes(severity)) {
        this.selectedSeverityTypes.push(severity);
      }
    } else {
      // Remove the severity type from the selectedSeverityTypes array
      this.selectedSeverityTypes = this.selectedSeverityTypes.filter(type => type !== severity);
    }
  }
  
  // Update pagination to fetch the next or previous page
  onPageChange(isNext: boolean): void {
    if (isNext) {
      this.startLine += this.lineCount; // Move to the next page
    } else if (this.startLine > 1) {
      this.startLine = Math.max(1, this.startLine - this.lineCount); // Move to the previous page but don't go below 1
    }
  
    // Fetch the logs for the new page
    this.fetchLogs();
  }
}
