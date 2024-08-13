import { Component, inject } from '@angular/core';
import { Chart } from 'chart.js';

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
}
