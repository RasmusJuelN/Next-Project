import { Component, inject } from '@angular/core';
import { Chart } from 'chart.js';
import { MockDataService } from '../../../services/mock-data.service';
import { ChartService } from '../../../services/chart.service';

/**
 * Represents the ChartComponent class that showcases charts. Currently in testing.
 */
@Component({
  selector: 'app-chart',
  standalone: true,
  imports: [],
  templateUrl: './chart.component.html',
  styleUrl: './chart.component.css'
})
export class ChartComponent {
  chart: Chart | null = null;
  dataService = inject(MockDataService);
  ChartService = inject(ChartService);
}
