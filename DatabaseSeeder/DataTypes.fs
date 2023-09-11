module DatabaseSeeder.DataTypes

type FilterValue = {
    Id: int
    FilterId: int
    Text: string
}

type Filter = {
    Id: int
    Name: string
}

type StudentFilter = {
    Id: int
    StudentId: int
    FilterId: int
    FilterValueId: int
}

type Subject = {
    Id: int
    Name: string
}

type Grade = {
    Id: int
    Points: float32 option
    Text: string
}

type StudentGrade = {
    Id: int
    StudentId: int
    SubjectId: int
    GradeId: int
}

type StudentSubject = {
    Id: int
    StudentId: int
    SubjectId: int
}

type Student = {
    Id: int
    FirstName: string
    LastName: string
}
