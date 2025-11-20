import { ChartStrategy } from './chart-strategy';
import { ChartBuildInput } from '../models/data-compare.model';

export class BarChartStrategy implements ChartStrategy {
  readonly type = 'bar';

  buildOptions(input: ChartBuildInput): any {
    const { question, year, answers } = input;

    const row: Record<string, any> = { question };
    Object.entries(answers).forEach(([answer, obj]) => {
      row[answer] = obj.count;
      row[`${answer}_dates`] = obj.dates;
    });

    const data = [row];
    const series = Object.keys(answers).map((answer) => ({
      type: 'bar',
      xKey: 'question',
      yKey: answer,
      yName: `Svar: ${answer}`,
      stacked: true,
      label: { enabled: true },
      tooltip: {
        enabled: true,
        renderer: ({ datum }: { datum: any }) => ({
          title: answer,
          content: `Antal: ${datum[answer]}<br>Dato: ${(datum[`${answer}_dates`] || []).join(', ')}`,
        }),
      },
    }));

    return {
      data,
      theme: { baseTheme: 'ag-polychroma', overrides: { bar: { series: { label: { enabled: true } } } } },
      title: { text: `${question} (${year})`, fontSize: 18 },
      series,
      axes: [
        { type: 'category', position: 'bottom', title: { text: 'Spørgsmål' } },
        { type: 'number', position: 'left', title: { text: 'Antal' } },
      ],
      legend: { enabled: true },
      animation: { enabled: true, duration: 800 },
    } as any;
  }
}
