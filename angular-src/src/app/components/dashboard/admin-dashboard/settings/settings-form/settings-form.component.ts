import { CommonModule } from '@angular/common';
import { Component, Input } from '@angular/core';
import {
  FormControl,
  FormGroup,
  FormsModule,
  ReactiveFormsModule,
} from '@angular/forms';
import { Metadata, Settings } from '../../../../../models/setting-models';

@Component({
  selector: 'app-settings-form',
  standalone: true,
  imports: [CommonModule, FormsModule, ReactiveFormsModule],
  templateUrl: './settings-form.component.html',
  styleUrls: ['../settings.component.css','./settings-form.component.css'],
})
export class SettingsFormComponent {
  @Input() group!: FormGroup;
  @Input() settings!: Settings;
  @Input() metadata!: Metadata;

  // Helper methods
  keys(obj: any): string[] {
    return obj ? Object.keys(obj) : [];
  }

  isFormGroup(control: any): control is FormGroup {
    return control instanceof FormGroup;
  }

  isFormControl(control: any): control is FormControl {
    return control instanceof FormControl;
  }

  getInputType(key: string): string {
    const value = this.settings ? this.settings[key] : undefined;
    const meta = this.metadata ? this.metadata[key] : undefined;

    if (meta && meta.allowedValues) {
      return 'select';
    } else if (typeof value === 'boolean') {
      return 'checkbox';
    } else if (typeof value === 'number') {
      return 'number';
    } else {
      return 'text';
    }
  }

  metadataForKey(key: string): any {
    return this.metadata && this.metadata[key] ? this.metadata[key] : {};
  }
}
