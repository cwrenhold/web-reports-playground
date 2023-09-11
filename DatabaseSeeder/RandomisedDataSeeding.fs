module DatabaseSeeder.RandomisedDataSeeding

open FSharp.Data.Sql
open Npgsql.FSharp

open DatabaseSeeder.Names
open DatabaseSeeder.DataTypes
open DatabaseSeeder.StaticDataSeeding

let populateStudents(connectionString) =
    let alreadyPopulated = 
        isTablePopulated connectionString "students"

    if alreadyPopulated then
        printfn "Students table already populated"
    else
        let randomSeed = 1
        let randomiser = System.Random(randomSeed)
        let totalStudentCount = 1000

        [0..totalStudentCount]
        |> Seq.iter (fun i ->
            let firstName = firstNames.[randomiser.Next(0, firstNames.Length)]
            let lastName = lastNames.[randomiser.Next(0, lastNames.Length)]

            connectionString
            |> Sql.connect
            |> Sql.query "INSERT INTO students (first_name, last_name) VALUES (@firstName, @lastName)"
            |> Sql.parameters [ "@firstName", Sql.string firstName; "@lastName", Sql.string lastName ]
            |> Sql.executeNonQuery
            |> ignore
        )

    // Return the list of students
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM students"
    |> Sql.execute (fun x -> { Id = x.int "id"; FirstName = x.string "first_name"; LastName = x.string "last_name" }: Student)
    |> Seq.toList

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
        let unweightedFilterValueRandomiser student filterId =
            let randomSeed = student.Id
            let randomiser = System.Random(randomSeed)
            let filterValues = filterValuesByFilterId |> Seq.find (fun (id, _) -> id = filterId) |> snd
            let filterValue = filterValues.[randomiser.Next(0, filterValues.Length)]
            filterValue

        // Picks the first filter value 90% of the time, random chance of the rest
        let weightedFilterValueRandomiser student filterId =
            let randomSeed = student.Id
            let randomiser = System.Random(randomSeed)
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
            filterValuesByFilterId
            |> Seq.iter (fun (filterId, _) ->
                let filterValueFinder = getRandomFilterValueFinder filterId
                let filterValue = filterValueFinder student filterId

                connectionString
                |> Sql.connect
                |> Sql.query "INSERT INTO student_filters (student_id, filter_id, filter_value_id) VALUES (@studentId, @filterId, @filterValueId)"
                |> Sql.parameters [ "@studentId", Sql.int student.Id; "@filterId", Sql.int filterId; "@filterValueId", Sql.int filterValue.Id ]
                |> Sql.executeNonQuery
                |> ignore
            )
        )

    // Return the list of student filters
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM student_filters"
    |> Sql.execute (fun x -> { Id = x.int "id"; StudentId = x.int "student_id"; FilterId = x.int "filter_id"; FilterValueId = x.int "filter_value_id" }: StudentFilter)
    |> Seq.toList
