import { Component, OnInit } from '@angular/core';
import {
  FormBuilder,
  FormGroup,
  FormControl,
  AbstractControl,
  Validators,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { DataService } from '../../../../services/data/data.service';
import { CommonModule } from '@angular/common';
import { SettingsFormComponent } from './settings-form/settings-form.component';
import { Metadata, Settings } from '../../../../models/setting-models';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [FormsModule, ReactiveFormsModule, CommonModule, SettingsFormComponent],
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.css'],
})
export class SettingsComponent implements OnInit {
  settings: Settings = {}; // Holds the actual settings values
  metadata: Metadata = {}; // Holds the metadata for settings
  
  form!: FormGroup;
  selectedSection!: string;
  sectionControl!: FormControl;
  loading: boolean = true; // To track loading state

  constructor(private fb: FormBuilder, private dataService: DataService) {}

  ngOnInit(): void {
    // Fetch settings and metadata from the backend
    this.dataService.getSettings().subscribe({
      next: (response) => {
        this.settings = response.settings;
        this.metadata = response.metadata;

        this.initializeSectionControl();
        this.updateFormForSelectedSection();

        this.loading = false; // Data has been loaded
      },
      error: (err) => {
        console.error('Error fetching settings:', err);
        this.loading = false;
        // Optionally handle the error, e.g., show a message to the user
      },
    });
  }

  private initializeSectionControl(): void {
    const topLevelKeys = Object.keys(this.settings);
    this.selectedSection = topLevelKeys[0]; // Default to the first section
  
    this.sectionControl = new FormControl(this.selectedSection);
    this.sectionControl.valueChanges.subscribe((newSection) => {
      if (newSection !== this.selectedSection) {
        if (this.form.dirty) {
          const confirmDiscard = confirm(
            'You have unsaved changes. Do you really want to switch sections and discard your changes?'
          );
          if (confirmDiscard) {
            this.selectedSection = newSection;
            this.updateFormForSelectedSection();
          } else {
            // Revert the selection to the previous section
            this.sectionControl.setValue(this.selectedSection, { emitEvent: false });
          }
        } else {
          this.selectedSection = newSection;
          this.updateFormForSelectedSection();
        }
      }
    });
  }
  

  private updateFormForSelectedSection(): void {
    if (
      this.settings[this.selectedSection] &&
      this.metadata[this.selectedSection]
    ) {
      this.form = this.buildForm(
        this.settings[this.selectedSection],
        this.metadata[this.selectedSection]
      );
    }
  }

  // Dynamically build a form group from the settings and metadata
  private buildForm(settings: any, metadata: any): FormGroup {
    const group = this.fb.group({});

    if (settings && metadata) {
      Object.keys(settings).forEach((key) => {
        const settingValue = settings[key];
        const settingMetadata = metadata[key] || {};

        if (this.isObject(settingValue) && !Array.isArray(settingValue)) {
          // Nested object - recursively build form group
          group.addControl(key, this.buildForm(settingValue, settingMetadata));
        } else {
          // Leaf node - create form control with validators
          const validators = this.buildValidators(settingMetadata);
          group.addControl(key, this.fb.control(settingValue, validators));
        }
      });
    }

    return group;
  }

  // Helper function to build validators based on metadata
  private buildValidators(metadata: any): any[] {
    const validators = [];

    if (metadata.canBeEmpty === false) {
      validators.push(Validators.required);
    }
    if (typeof metadata.minValue === 'number') {
      validators.push(Validators.min(metadata.minValue));
    }
    if (typeof metadata.maxValue === 'number') {
      validators.push(Validators.max(metadata.maxValue));
    }
    if (Array.isArray(metadata.allowedValues)) {
      validators.push(this.allowedValuesValidator(metadata.allowedValues));
    }

    return validators;
  }

  // Custom validator for allowed values
  private allowedValuesValidator(allowedValues: any[]): any {
    return (control: AbstractControl) => {
      if (allowedValues.includes(control.value)) {
        return null; // Valid value
      }
      return { allowedValues: { valid: false, allowedValues } }; // Invalid value
    };
  }

  private isObject(value: any): boolean {
    return value && typeof value === 'object' && !Array.isArray(value);
  }

  keys(obj: any): string[] {
    return obj ? Object.keys(obj) : [];
  }

  metadataForKey(key: string): any {
    return this.metadata && this.metadata[key] ? this.metadata[key] : {};
  }

  saveSettings(): void {
    const updatedSettings = this.form.value;
    this.settings[this.selectedSection] = updatedSettings;

    // Use the service to save settings
    this.dataService.updateSettings(this.settings).subscribe(
      (response) => {
        console.log('Settings saved successfully:', response);
        // Optionally show a success message to the user
      },
      (error) => {
        console.error('Error saving settings:', error);
        // Optionally handle the error, e.g., show a message to the user
      }
    );
  }
}
