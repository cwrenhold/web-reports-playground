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

type StudentGrade = {
    Id: int
    StudentId: int
    SubjectId: int
    Points: int
}

type Student = {
    Id: int
    FirstName: string
    LastName: string
}
