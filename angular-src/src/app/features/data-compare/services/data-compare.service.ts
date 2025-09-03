import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class DataCompareService {

}

export function getData() {
  return [
    {
      data: "Sales",
      year: 40,
      rate: 75,
    },
    {
      data: "Engineering",
      year: 45,
      rate: 90,
    },
    {
      data: "HR",
      year: 80,
      rate: 60,
    },
    {
      data: "Marketing",
      year: 80,
      rate: 60,
    },
    {
      data: "Finance",
      year: 85,
      rate: 50,
    },
  ];
}
