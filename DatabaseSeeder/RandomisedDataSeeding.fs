module DatabaseSeeder.RandomisedDataSeeding

open FSharp.Data.Sql
open Npgsql.FSharp

open DatabaseSeeder.Names
open DatabaseSeeder.DataTypes
open DatabaseUtils

let populateStudents(connectionString) =
    let alreadyPopulated = 
        isTablePopulated connectionString "students"

    if alreadyPopulated then
        printfn "Students table already populated"
    else
        let totalStudentCount = 1000

        [0..totalStudentCount]
        |> Seq.iter (fun i ->
            let randomiser = System.Random(i)
            let firstName = firstNames.[randomiser.Next(0, firstNames.Length)]
            let lastName = lastNames.[randomiser.Next(0, lastNames.Length)]

            connectionString
            |> Sql.connect
            |> Sql.query "INSERT INTO students (first_name, last_name) VALUES (@firstName, @lastName)"
            |> Sql.parameters [ "@firstName", Sql.string firstName; "@lastName", Sql.string lastName ]
            |> Sql.executeNonQuery
            |> ignore
        )
        printfn "Students table populated"

    selectAllAndMap connectionString "students" (fun x -> { Id = x.int "id"; FirstName = x.string "first_name"; LastName = x.string "last_name" }: Student)

let populateStudentFilter connectionString students (filters: Filter list) (filtervalues: FilterValue list) =
    let alreadyPopulated = 
        isTablePopulated connectionString "student_filters"

    if alreadyPopulated then
        printfn "Student filter values table already populated"
    else
        let filterValuesByFilterId =
            filtervalues
            |> Seq.groupBy (fun fv -> fv.FilterId)
            |> Seq.map (fun (filterId, filterValues) -> (filterId, filterValues |> Seq.toArray))
            |> Seq.toArray

        // Picks any filter value at random
        let unweightedFilterValueRandomiser student filterId (randomiser: System.Random) =
            let filterValues = filterValuesByFilterId |> Seq.find (fun (id, _) -> id = filterId) |> snd
            let filterValue = filterValues.[randomiser.Next(0, filterValues.Length)]
            filterValue

        // Picks the first filter value 90% of the time, random chance of the rest
        let weightedFilterValueRandomiser student filterId (randomiser: System.Random) =
            let filterValues = filterValuesByFilterId |> Seq.find (fun (id, _) -> id = filterId) |> snd
            let filterValue = 
                if randomiser.Next(0, 10) = 0 then
                    filterValues.[randomiser.Next(0, filterValues.Length)]
                else
                    filterValues.[0]
            filterValue

        let getRandomFilterValueFinder filterId =
            let filter = filters |> Seq.find (fun x -> x.Id = filterId)
            match filter.Name with
            | "Gender" -> unweightedFilterValueRandomiser
            | "Class" -> unweightedFilterValueRandomiser
            | _ -> weightedFilterValueRandomiser

        students
        |> Seq.iter (fun student ->
            let randomSeed = student.Id
            let randomiser = System.Random(randomSeed)

            filterValuesByFilterId
            |> Seq.iter (fun (filterId, _) ->
                let filterValueFinder = getRandomFilterValueFinder filterId
                let filterValue = filterValueFinder student filterId randomiser

                connectionString
                |> Sql.connect
                |> Sql.query "INSERT INTO student_filters (student_id, filter_id, filter_value_id) VALUES (@studentId, @filterId, @filterValueId)"
                |> Sql.parameters [ "@studentId", Sql.int student.Id; "@filterId", Sql.int filterId; "@filterValueId", Sql.int filterValue.Id ]
                |> Sql.executeNonQuery
                |> ignore
            )
        )
        printfn "Student filter values table populated"

    selectAllAndMap connectionString "student_filters" (fun x -> { Id = x.int "id"; StudentId = x.int "student_id"; FilterId = x.int "filter_id"; FilterValueId = x.int "filter_value_id" }: StudentFilter)


let populateStudentGrades connectionString students (subjects: Subject list) (grades: Grade list) =
    let alreadyPopulated = 
        isTablePopulated connectionString "student_grades"

    if alreadyPopulated then
        printfn "Student grades table already populated"
    else
        students
        |> Seq.iter (fun student ->
            let randomSeed = student.Id
            let randomiser = System.Random(randomSeed)

            subjects
            |> Seq.iter (fun subject ->
                // Skip this subject 2% of the time
                if randomiser.Next(0, 50) = 0 then
                    ()
                else
                    let grade = grades.[randomiser.Next(0, grades.Length)]

                    connectionString
                    |> Sql.connect
                    |> Sql.query "INSERT INTO student_grades (student_id, subject_id, grade_id) VALUES (@studentId, @subjectId, @gradeId)"
                    |> Sql.parameters [ "@studentId", Sql.int student.Id; "@subjectId", Sql.int subject.Id; "@gradeId", Sql.int grade.Id ]
                    |> Sql.executeNonQuery
                    |> ignore
            )
        )
        printfn "Student grades table populated"

    selectAllAndMap connectionString "student_grades" (fun x -> { Id = x.int "id"; StudentId = x.int "student_id"; SubjectId = x.int "subject_id"; GradeId = x.int "grade_id" }: StudentGrade)

let populateStudentSubjects connectionString students (subjects: Subject list) =
    let alreadyPopulated = 
        isTablePopulated connectionString "student_subjects"

    if alreadyPopulated then
        printfn "Student subjects table already populated"
    else
        students
        |> Seq.iter (fun student ->
            subjects
            |> Seq.iter (fun subject ->
                connectionString
                |> Sql.connect
                |> Sql.query "INSERT INTO student_subjects (student_id, subject_id) VALUES (@studentId, @subjectId)"
                |> Sql.parameters [ "@studentId", Sql.int student.Id; "@subjectId", Sql.int subject.Id ]
                |> Sql.executeNonQuery
                |> ignore
            )
        )
        printfn "Student subjects table populated"

    selectAllAndMap connectionString "student_subjects" (fun x -> { Id = x.int "id"; StudentId = x.int "student_id"; SubjectId = x.int "subject_id" }: StudentSubject)
