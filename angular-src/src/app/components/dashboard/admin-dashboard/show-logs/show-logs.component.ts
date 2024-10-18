import { Component, inject, OnInit, OnDestroy } from '@angular/core'; 
import { DataService } from '../../../../services/data/data.service';
import { LogEntry, LogFileType } from '../../../../models/log-models';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

type SeverityLevel = 'DEBUG' | 'INFO' | 'WARNING' | 'ERROR' | 'CRITICAL';

@Component({
  selector: 'app-show-logs',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './show-logs.component.html',
  styleUrls: ['./show-logs.component.css']
})
export class ShowLogsComponent implements OnInit, OnDestroy {
  private dataService = inject(DataService);
  logs: LogEntry[] = [];
  filteredLogs: LogEntry[] = [];

  logFileTypes: LogFileType = {};
  logFileTypeKeys: string[] = [];
  selectedLogFileType: string = '';

  severityLevels: SeverityLevel[] = ['DEBUG', 'INFO', 'WARNING', 'ERROR', 'CRITICAL'];
  selectedFetchSeverityLevel: SeverityLevel = 'DEBUG';
  selectedFilterSeverityLevels: SeverityLevel[] = [];

  lineCount: number | null = 10;  // Default value
  loadAllLines: boolean = false;
  reverseLogs: boolean = false;
  isLoading: boolean = false;
  errorMessage: string | null = null;

  private refreshIntervalId: any;

  ngOnInit(): void {
    this.fetchLogFileTypes();

    // Auto-refresh log files every 10 seconds
    this.refreshIntervalId = setInterval(() => {
      this.fetchLogFileTypes();
    }, 10000);
  }

  ngOnDestroy(): void {
    if (this.refreshIntervalId) {
      clearInterval(this.refreshIntervalId);
    }
  }

  fetchLogFileTypes(): void {
    this.dataService.getLogFileTypes().subscribe({
      next: (fileTypes: LogFileType) => {
        this.logFileTypes = fileTypes;
        this.logFileTypeKeys = Object.keys(fileTypes);

        // If the selected log file type is no longer available, reset it
        if (!this.logFileTypeKeys.includes(this.selectedLogFileType)) {
          this.selectedLogFileType = this.logFileTypeKeys[0] || '';
        }
      },
      error: () => {
        this.errorMessage = 'Failed to load log file types';
      },
    });
  }

  fetchLogs(): void {
    this.isLoading = true;
    this.errorMessage = null;

    this.dataService
      .getLogs(
        this.selectedFetchSeverityLevel,
        this.selectedLogFileType,
        this.lineCount,
        this.reverseLogs
      )
      .subscribe({
        next: (data: LogEntry[]) => {
          this.logs = data;
          this.applySeverityFilter();
          this.isLoading = false;
        },
        error: () => {
          this.errorMessage = 'Error fetching logs';
          this.isLoading = false;
        },
      });
  }

  applySeverityFilter(): void {
    if (this.selectedFilterSeverityLevels.length === 0) {
      this.filteredLogs = this.logs;
    } else {
      this.filteredLogs = this.logs.filter((log) =>
        this.selectedFilterSeverityLevels.includes(log.severity)
      );
    }
  }

  onSeverityFilterChange(event: Event, severity: SeverityLevel): void {
    const checkbox = event.target as HTMLInputElement;
    if (checkbox.checked) {
      if (!this.selectedFilterSeverityLevels.includes(severity)) {
        this.selectedFilterSeverityLevels.push(severity);
      }
    } else {
      this.selectedFilterSeverityLevels = this.selectedFilterSeverityLevels.filter(
        (level) => level !== severity
      );
    }
    this.applySeverityFilter();
  }

  onUpdateLines(): void {
    if (this.loadAllLines) {
      this.lineCount = null;
    } else {
      if (this.lineCount === null || isNaN(this.lineCount) || this.lineCount <= 0) {
        this.errorMessage = 'Number of Lines must be greater than 0 or select Load All Lines.';
        return;
      }
    }
    this.fetchLogs();
  }
}
