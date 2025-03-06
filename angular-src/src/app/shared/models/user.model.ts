export interface User {
    id: string;
    userName: string;
    fullName: string
    role: string;
  }


  export enum Role {
    Student = "student",
    Teacher = "teacher",
    Admin = "admin"
  }