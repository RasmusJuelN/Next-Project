import { ChartStrategy } from './chart-strategy';
import { ChartBuildInput } from '../models/data-compare.model';

export class PieChartStrategy implements ChartStrategy {
  readonly type = 'pie';

  buildOptions(input: ChartBuildInput): any {
    const { question, year, answers } = input;

    const pieData = Object.entries(answers).map(([answer, obj]) => ({
      answer,
      count: obj.count,
      dates: obj.dates,
    }));

    const theme = {
      baseTheme: 'ag-polychroma',
      overrides: {},
    } as any;

    return {
      data: pieData,
      title: { text: `${question} (${year})` },
      theme,
      series: [
        {
          type: 'pie',
          legend: { enabled: true },
          calloutLabel: false,
          //calloutLabelKey: 'answer',
          sectorLabelKey: 'count',
          angleKey: 'count',
          legendItemKey: 'answer',
          sectorLabel: {
            positionOffset: 30,
            formatter: ({ datum, angleKey }: { datum: any; angleKey: string }) => {
              const value = datum[angleKey];
              const total = pieData.reduce((sum, d) => sum + d.count, 0);
              const pct = total ? (value / total) * 100 : 0;
              return pct >= 5 ? `${pct.toFixed(1)}%` : '';
            },
          },
          strokeWidth: 1,
          tooltip: {
            enabled: true,
            renderer: ({ datum, angleKey }: { datum: any; angleKey: string }) => ({
              title: datum.answer,
              content: `Antal: ${datum[angleKey]}<br>Dato: ${datum.dates.join(', ')}`,
            }),
          },
        },
      ],
      animation: { enabled: true, duration: 800 },
    } as any;
  }
}
