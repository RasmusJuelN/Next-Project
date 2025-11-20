// import { Pipe, PipeTransform } from '@angular/core';

// @Pipe({
//   name: 'groupDisplayName',
//   standalone: true
// })
// export class GroupDisplayNamePipe implements PipeTransform {
//   transform(name: string, createdAt: string | Date): string {
//     const date = new Date(createdAt);
    
//     // Format: dd-MM-yyyy HH:mm
//     const day = date.getDate().toString().padStart(2, '0');
//     const month = (date.getMonth() + 1).toString().padStart(2, '0');
//     const year = date.getFullYear();
//     const hours = date.getHours().toString().padStart(2, '0');
//     const minutes = date.getMinutes().toString().padStart(2, '0');
    
//     const formattedDate = `${day}-${month}-${year} ${hours}:${minutes}`;
    
//     return `${name} (${formattedDate})`;
//   }
// }