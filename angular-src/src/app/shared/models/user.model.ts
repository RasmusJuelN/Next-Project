export interface User {
    
    id: string;
    userName: string;
    fullName: string
    role: string;
  }


  // Key values of role in config in backend
  export enum Role {
    Student = "student",
    Teacher = "teacher",
    Admin = "admin"
  }