import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActiveService } from '../../services/active.service';
import { Student } from '../../models/active.models';

@Component({
  selector: 'app-testcomponent',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './testcomponent.component.html',
  styleUrl: './testcomponent.component.css'
})
export class TestcomponentComponent implements OnInit {
  groups: any[] = [];  // your groups
  studentsByGroup: { [groupName: string]: Student[] } = {};
  showStudentsFor: string | null = null; 
  searchStudent: string = '';

  constructor(private groupService: ActiveService) {}

ngOnInit(): void {
//   this.groupService.getAllGroups().subscribe(groups => {
//     console.log('Raw API groups:', groups);    
//     this.groups = groups;

//     // Preload students for each group
//     groups.forEach(group => {
//       this.groupService.getStudentsInGroup(group.name).subscribe(students => {
//         // Normalize the student objects so `name` always exists
//         this.studentsByGroup[group.name] = students.map(s => ({
//           ...s,
//           name: s.fullName || s.userName
//         }));
//       });
//     });
//   });
// }
 // Step 1: Fetch all classes/groups
//  this.groupService.getClasses().subscribe(classes => {
//   const validGroups = ['h1','h2','h3','h4'];
//   this.groups = classes
//     .map(c => c.trim().toLowerCase())
//     .filter(c => validGroups.includes(c));

//   // Fetch students for each group
//   this.groups.forEach(group => {
// this.groupService.getStudentsInGroup(group).subscribe(response => {
//   const classData = response[0]; // take first object
//   this.studentsByGroup[group] = (classData?.students ?? []).map((s: User) => ({
//   // id: s.id,
//   // fullName: s.fullName,
//   // userName: s.userName,
//   // role: 'Student',
//   name: s.fullName || s.userName
// }));

// });


//   });
// });
// }



//   onGroupClick(groupName: string) {
//     this.showStudentsFor = this.showStudentsFor === groupName ? null : groupName;
//   }
// filteredStudents(group: string): Student[] {
//   const students = this.studentsByGroup[group] || [];
//   if (!this.searchStudent.trim()) return students;

//   return students.filter(s =>
//     s.name.toLowerCase().includes(this.searchStudent.toLowerCase())
//   );
// }
// //   filteredStudents(groupName: string): Student[] {
// //   const students = this.studentsByGroup[groupName] || [];

// //   if (!this.searchStudent.trim()) return students;

// //   return students.filter(s =>
// //     s.name.toLowerCase().includes(this.searchStudent.toLowerCase())
// //   );
// // }

//   // Automatically show groups that have matching students
//   get showStudentsForGroups(): string[] {
//   if (!this.searchStudent.trim()) return this.groups;

//   return this.groups.filter(group =>
//     (this.studentsByGroup[group] || []).some(student =>
//       student.name.toLowerCase().includes(this.searchStudent.toLowerCase())
//     )
//   );
// }

//   // get showStudentsForGroups(): string[] {
//   //   if (!this.searchStudent.trim()) return this.groups.map(g => g.name);

//   //   return this.groups
//   //     .filter(group => 
//   //       (this.studentsByGroup[group.name] || []).some(student =>
//   //         student.name.toLowerCase().includes(this.searchStudent.toLowerCase())
//   //       )
//   //     )
//   //     .map(g => g.name);
  }
}


