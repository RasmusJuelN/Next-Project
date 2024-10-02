import { Component, inject } from '@angular/core';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpClient } from '@angular/common/http'; // Future use for sending requests
import { AppSettings, DatabaseType } from '../../../../models/setting-models';
import { DataService } from '../../../../services/data/data.service';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CommonModule],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css']
})
export class SettingsComponent {
  private dataService = inject(DataService)
  private http = inject(HttpClient)
  private fb = inject(FormBuilder)
  settingsForm: FormGroup;

  databaseTypes: DatabaseType[] = ['sqlite', 'mysql', 'mssql'];
  
  constructor() {
    this.settingsForm = this.fb.group({
      auth: this.fb.group({
        accessTokenExpireMinutes: [null, Validators.required],
        algorithm: [null, Validators.required],
        domain: [null, Validators.required],
        ldapBaseDn: [null],
        ldapServer: [null, Validators.required],
        secretKey: [null]
      }),
      database: this.fb.group({
        databaseDriver: [null],
        databaseType: [null, Validators.required],
        dbName: [null, Validators.required],
        host: [null],
        maxConnections: [null, [Validators.min(1)]],
        minConnections: [null, [Validators.min(1)]],
        password: [null],
        port: [null],
        sslCaCertFile: [null],
        sslCertFile: [null],
        sslKeyFile: [null],
        timeout: [null],
        useSsl: [null],
        user: [null]
      })
    });
  }

  ngOnInit(): void {
    // Fetch the initial settings from the mock service
    this.dataService.getSettings().subscribe((settings: AppSettings) => {
      this.settingsForm.patchValue(settings); // Populate the form with fetched settings
    });
  }

  onSubmit(): void {
    if (this.settingsForm.valid) {
      const updatedSettings: AppSettings = this.settingsForm.value;
  
      // Show a confirmation dialog
      const confirmed = window.confirm('Are you sure you want to save these settings?');
  
      if (confirmed) {
        // If user confirms, proceed with saving
        this.dataService.updateSettings(updatedSettings).subscribe({
          next: (response) => {
            console.log('Settings updated successfully:', response);
          },
          error: (error) => {
            console.error('Error updating settings:', error);
          }
        });
      } else {
        // User cancelled, log or do nothing
        console.log('User cancelled the save operation.');
      }
    } else {
      console.error('Form is invalid');
    }
  }
  
}

