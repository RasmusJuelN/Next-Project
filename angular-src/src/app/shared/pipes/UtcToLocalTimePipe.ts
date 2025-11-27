import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'utcToLocalTime',
  standalone: true
})
export class UtcToLocalTimePipe implements PipeTransform {
  transform(utcDate: string | Date, format: string = 'dd-MM-yyyy HH:mm'): string {
    if (!utcDate) return '';
    
    let date: Date;
    
    if (typeof utcDate === 'string') {
      // Force UTC interpretation if no Z suffix
      const utcString = utcDate.endsWith('Z') ? utcDate : utcDate + 'Z';
      date = new Date(utcString);
    } else {
      date = utcDate;
    }
    
    // Check if date is valid
    if (isNaN(date.getTime())) {
      return 'Invalid Date';
    }
    
    // Format as DD-MM-YYYY HH:mm in local timezone
    const day = date.getDate().toString().padStart(2, '0');
    const month = (date.getMonth() + 1).toString().padStart(2, '0');
    const year = date.getFullYear();
    const hours = date.getHours().toString().padStart(2, '0');
    const minutes = date.getMinutes().toString().padStart(2, '0');
    
    return `${day}-${month}-${year} ${hours}:${minutes}`;
  }
}