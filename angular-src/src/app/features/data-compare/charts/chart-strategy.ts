import { ChartBuildInput } from '../models/data-compare.model';

export interface ChartStrategy {
  /** Unique key used to pick the strategy (e.g. 'pie', 'bar') */
  readonly type: string;

  /** Build ag-charts options from the unified input */
  buildOptions(input: ChartBuildInput): any; // keep as any to avoid coupling to ag-charts types
}
