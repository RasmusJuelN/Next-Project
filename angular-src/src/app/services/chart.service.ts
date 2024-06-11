import { Injectable } from '@angular/core';
import { Question, StudentTeacherAnwser } from '../../models/questionare';
import {Chart, ChartConfiguration, ChartData, ChartItem} from 'chart.js';

@Injectable({
  providedIn: 'root'
})
export class ChartService {

  initializeChart(questions: Question[], answers: StudentTeacherAnwser[]): Chart {
    const labels = questions.map(q => q.text);

    const convertToRange = (rating: number): [number, number] => {
      const converted = rating - 3; // Convert 1-5 to -2 to +2
      return converted === 0 ? [-0.3, 0.3] : converted > 0 ? [0, converted] : [converted, 0];
    };

    const studentRatings = answers.map(a => convertToRange(a.student.rating));
    const teacherRatings = answers.map(a => convertToRange(a.teacher.rating));

    const LabelsForXAxis: { [key: number]: string } = {
      '-2': 'Does not understand at all',
      '-1': 'Understands poorly',
      '0': 'Understands somewhat',
      '1': 'Understands well',
      '2': 'Understands very well'
    };

    const getBackgroundColor = (range: [number, number]): string => 
      range[0] === -0.3 && range[1] === 0.3 ? 'rgba(255, 165, 0, 0.5)' :
      range[0] >= 0 && range[1] > 0 ? 'rgba(75, 192, 192, 0.5)' : 
      'rgba(255, 99, 132, 0.5)';

    const createDataset = (label: string, ratings: [number, number][]) => ({
      label,
      data: ratings,
      backgroundColor: ratings.map(getBackgroundColor),
      borderColor: ratings.map(getBackgroundColor),
      borderWidth: 1
    });

    const data: ChartData<'bar', (number | [number, number] | null)[]> = {
      labels,
      datasets: [
        createDataset('Student Ratings', studentRatings),
        createDataset('Teacher Ratings', teacherRatings)
      ]
    };

    const config: ChartConfiguration<'bar'> = {
      type: 'bar',
      data,
      options: {
        scales: {
          x: {
            beginAtZero: true,
            min: -2,
            max: 2,
            ticks: {
              autoSkip: false,
              callback: value => LabelsForXAxis[value as number] || ''
            }
          },
          y: {
            beginAtZero: true,
            stacked: false
          }
        },
        indexAxis: 'y',
        elements: {
          bar: {
            borderWidth: 2,
            borderRadius: 4,
            borderSkipped: false
          }
        },
        plugins: {
          tooltip: {
            callbacks: {
              label: context => {
                const dataIndex = context.dataIndex;
                const datasetIndex = context.datasetIndex;
                const dataset = data.datasets[datasetIndex];
                const dataValue = dataset.data[dataIndex] as [number, number];

                const question = questions[dataIndex];
                const studentRating = answers[dataIndex].student.rating;
                const teacherRating = answers[dataIndex].teacher.rating;

                const optionLabel = (rating: number) =>
                  question.options.find(option => option.value === rating)?.label || 'No label';

                const rangeLabel = dataValue ? `${dataValue[0]} to ${dataValue[1]}` : 'No data';

                return dataset.label === 'Student Ratings'
                  ? `Student: ${rangeLabel}, Rating: ${studentRating}, Option: ${optionLabel(studentRating)}`
                  : `Teacher: ${rangeLabel}, Rating: ${teacherRating}, Option: ${optionLabel(teacherRating)}`;
              }
            }
          }
        }
      }
    };

    const canvas = document.getElementById('myChart') as HTMLCanvasElement;
    return new Chart(canvas as ChartItem, config);
  }

}
