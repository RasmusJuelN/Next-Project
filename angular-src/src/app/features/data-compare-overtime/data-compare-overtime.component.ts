import { CommonModule } from '@angular/common';
import type { AgChartOptions } from 'ag-charts-community';
import {
  Component,
  ElementRef,
  EventEmitter,
  HostListener,
  OnDestroy,
  OnInit,
  ViewChild,
  inject,
  Output,
} from "@angular/core";
import { AgCharts } from "ag-charts-angular";

@Component({
  selector: 'app-data-compare-overtime',
  standalone: true,
  imports: [CommonModule, AgCharts,DataCompareOvertimeComponent],
   template: `
    <ag-charts-angular [options]="chartOptions"></ag-charts-angular>
  `,
  templateUrl: './data-compare-overtime.component.html',
  styleUrl: './data-compare-overtime.component.css'
})

export class DataCompareOvertimeComponent {
  public options: AgChartOptions = {
    title: { text: 'Annual Fuel Expenditure' },
    data: getData(),
    series: [
      { type: 'line', xKey: 'quarter', yKey: 'petrol', yName: 'Teacher' },
      { type: 'line', xKey: 'quarter', yKey: 'diesel', yName: 'Student' },
    ],
  };
}

function getData() {
  return [
    { quarter: '02-02-2020', petrol: 2, diesel: 3 },
    { quarter: '05-03-2020', petrol: 1, diesel: 3 },
    { quarter: '23-04-2020', petrol: 4, diesel: 4 },
    { quarter: '02-05-2020', petrol: 4, diesel: 5 },
  ];
}
