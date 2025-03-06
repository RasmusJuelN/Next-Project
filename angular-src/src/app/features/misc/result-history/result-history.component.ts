import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { User } from '../../../shared/models/user.model';

export enum Rating {
  A = 'A',
  B = 'B',
  C = 'C',
  D = 'D',
  E = 'E'
}

export interface Result {
  id: string;
  templateName: string;
  student: {
    user: User;
    answeredWhen: Date;
  };
  teacher: {
    user: User;
    answeredWhen: Date;
  };
  answers: {
    question: string;
    studentAnswer: string;
    isStudentCustomAnswer: boolean;
    studentRating?: Rating | null;
    teacherAnswer: string;
    isTeacherCustomAnswer: boolean;
    teacherRating?: Rating | null;
  }[];
}

@Component({
  selector: 'app-result-history',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './result-history.component.html',
  styleUrls: ['./result-history.component.css']
})
export class ResultHistoryComponent {

  // Sample data
  results: Result[] = [
    {
      id: 'res-1',
      templateName: 'Math Quiz',
      student: {
        user: { id: 's1', userName: 'Student One', fullName: "Jake", role:"student" },
        answeredWhen: new Date('2025-03-01T10:00:00')
      },
      teacher: {
        user: { id: 't1', userName: 'Teacher One', fullName: "Jake", role:"teacher" },
        answeredWhen: new Date('2025-03-01T12:00:00')
      },
      answers: [
        {
          question: 'Q1: Solve 2+2',
          studentAnswer: '4',
          isStudentCustomAnswer: false,
          studentRating: Rating.A,
          teacherAnswer: '4 is correct',
          isTeacherCustomAnswer: false,
          teacherRating: Rating.A
        },
        {
          question: 'Q2: Explain your method',
          studentAnswer: 'Custom explanation text',
          isStudentCustomAnswer: true,
          studentRating: null,
          teacherAnswer: 'Good reasoning!',
          isTeacherCustomAnswer: false,
          teacherRating: Rating.B
        }
      ]
    },
    {
      id: 'res-2',
      templateName: 'Science Test',
      student: {
        user: { id: 's2', userName: 'Student One', fullName: "Jake", role:"student" },
        answeredWhen: new Date('2025-03-01T10:00:00')
      },
      teacher: {
        user: { id: 't2', userName: 'Teacher One', fullName: "Jake", role:"teacher" },
        answeredWhen: new Date('2025-03-01T12:00:00')
      },
      answers: [
        {
          question: 'Q1: What is H₂O?',
          studentAnswer: 'Water',
          isStudentCustomAnswer: false,
          studentRating: Rating.A,
          teacherAnswer: 'Correct',
          isTeacherCustomAnswer: false,
          teacherRating: Rating.A
        },
        {
          question: 'Q2: Explain states of matter',
          studentAnswer: 'Custom: In the realm of physics and chemistry, matter is commonly recognized to exist in several distinct forms, traditionally referred to as states or phases. The most familiar states of matter—solid, liquid, and gas—exhibit unique characteristics that can be observed in everyday life. A solid, for instance, retains a fixed shape and volume because the particles that comprise it are closely packed and vibrate around fixed positions. This arrangement of particles grants solids both structural rigidity and a relatively consistent form, such as a rock or a wooden block. Liquids, on the other hand, maintain a definite volume but not a definite shape, taking on the shape of their container. The particles in a liquid are more loosely bonded than those in a solid, allowing them to flow and move past each other with relative ease. This property makes liquids versatile in processes like pouring, mixing, and flowing through channels, as seen with water in a glass or juice in a carton. Gases, in contrast, have neither a fixed shape nor a fixed volume, expanding to fill any available space. Gas particles are in rapid, random motion, colliding frequently yet occupying a far larger volume relative to their size. Examples of gases include the air we breathe, the helium in balloons, and the carbon dioxide bubbles that effervesce in soda',
          isStudentCustomAnswer: true,
          studentRating: null,
          teacherAnswer: 'Answer is acceptable',
          isTeacherCustomAnswer: false,
          teacherRating: Rating.C
        }
      ]
    }
  ];

  // Extend the detail object to store userName, answeredWhen, and rating
  selectedDetail: {
    role: 'teacher' | 'student';
    question: string;
    answer: string;
    userName: string;
    answeredWhen: Date;
    rating?: Rating | null;
  } | null = null;

  /**
   * Convert a rating (A–E) into a number for filling columns (1–5).
   * Return 0 if rating is null/undefined or not recognized.
   */
  ratingToNumber(rating?: Rating | null): number {
    switch (rating) {
      case Rating.A: return 1;
      case Rating.B: return 2;
      case Rating.C: return 3;
      case Rating.D: return 4;
      case Rating.E: return 5;
      default:       return 0;
    }
  }

  /**
   * Called when a rating cell or question mark is clicked to show the detail view.
   * We now accept userName, answeredWhen, and rating.
   */
  showDetails(
    role: 'teacher' | 'student',
    question: string,
    answer: string,
    userName: string,
    answeredWhen: Date,
    rating?: Rating | null
  ) {
    this.selectedDetail = {
      role,
      question,
      answer,
      userName,
      answeredWhen,
      rating
    };
  }

  /**
   * Close/hide the detail view.
   */
  closeDetails() {
    this.selectedDetail = null;
  }
}
