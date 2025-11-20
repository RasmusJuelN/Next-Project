import { ChartStrategy } from './chart-strategy';
import { PieChartStrategy } from './pie-chart.strategy';
import { BarChartStrategy } from './bar-chart.strategy';

/**
 * Lightweight registry/factory for chart strategies.
 * If you prefer Angular DI, you can convert this to an @Injectable service.
 */
export class ChartRegistry {
  private strategies = new Map<string, ChartStrategy>();

  constructor() {
    this.register(new PieChartStrategy());
    this.register(new BarChartStrategy());
  }

  register(strategy: ChartStrategy) {
    this.strategies.set(strategy.type, strategy);
  }

  has(type: string): boolean {
    return this.strategies.has(type);
  }

  get(type: string): ChartStrategy {
    const strat = this.strategies.get(type);
    if (!strat) throw new Error(`Unknown chart type: ${type}`);
    return strat;
  }

  list(): string[] {
    return Array.from(this.strategies.keys());
  }
}
