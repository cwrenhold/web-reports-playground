export interface Filter {
  id: number | null;
  name: string;
}
export interface FilterValue {
  id: number;
  text: string;
  filterId: number;
}
export interface Grade {
  id: number;
  text: string;
  points: number | null;
}
export interface Student {
  id: number;
  firstName: string;
  lastName: string;
}
export interface StudentFilter {
  id: number;
  studentId: number;
  filterId: number;
  filterValueId: number;
}
export interface StudentGrade {
  id: number;
  studentId: number;
  subjectId: number;
  gradeId: number;
}
export interface StudentSubject {
  id: number;
  studentId: number;
  subjectId: number;
}
export interface Subject {
  id: number | null;
  name: string;
}
export interface ApiResponse {
  filters: Filter[];
  filterValues: FilterValue[];
  grades: Grade[];
  students: Student[];
  studentFilters: StudentFilter[];
  studentGrades: StudentGrade[];
  studentSubjects: StudentSubject[];
  subjects: Subject[];
}
