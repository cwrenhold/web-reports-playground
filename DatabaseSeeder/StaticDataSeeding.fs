module DatabaseSeeder.StaticDataSeeding

open Npgsql.FSharp
open DataTypes

// Populate the subjects table using the subject names from Names
let populateSubjectsTable(connectionString): Subject list =
    let alreadyPopulated = 
        connectionString
        |> Sql.connect
        |> Sql.query "SELECT COUNT(*) AS count FROM subjects"
        |> Sql.execute (fun x -> x.int "count")
        |> Seq.head
        |> (<) 0

    if alreadyPopulated then
        printfn "Subjects table already populated"
    else
        Names.subjectNames
        |> Seq.iter (fun name ->
            connectionString
            |> Sql.connect
            |> Sql.query "INSERT INTO subjects (name) VALUES (@name)"
            |> Sql.parameters [ "@name", Sql.string name ]
            |> Sql.executeNonQuery
            |> ignore
        )

    // Return the list of subjects
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM subjects"
    |> Sql.execute (fun x -> { Id = x.int "id"; Name = x.string "name" }: Subject)
    |> Seq.toList

let populateFiltersTable(connectionString): Filter list =
    let alreadyPopulated = 
        connectionString
        |> Sql.connect
        |> Sql.query "SELECT COUNT(*) AS count FROM filters"
        |> Sql.execute (fun x -> x.int "count")
        |> Seq.head
        |> (<) 0

    if alreadyPopulated then
        printfn "Filters table already populated"
    else
        Names.filterNames
        |> Seq.iter (fun name ->
            connectionString
            |> Sql.connect
            |> Sql.query "INSERT INTO filters (name) VALUES (@name)"
            |> Sql.parameters [ "@name", Sql.string name ]
            |> Sql.executeNonQuery
            |> ignore
        )

    // Return the list of filters
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM filters"
    |> Sql.execute (fun x -> { Id = x.int "id"; Name = x.string "name" }: Filter)
    |> Seq.toList

let populateFilterValuesTable(connectionString) (filters: Filter list): FilterValue list =
    let alreadyPopulated = 
        connectionString
        |> Sql.connect
        |> Sql.query "SELECT COUNT(*) AS count FROM filter_values"
        |> Sql.execute (fun x -> x.int "count")
        |> Seq.head
        |> (<) 0

    if alreadyPopulated then
        printfn "Filter values table already populated"
    else
        // Iterate through each filter value in names, for each one, find the ID from the supplied filters, and then insert the record
        Names.filterValues
        |> Seq.iter (fun (filterValue) ->
            let filterName = filterValue.filterName
            let filterValue = filterValue.filterValue

            let filterId = filters |> Seq.find (fun x -> x.Name = filterName) |> (fun x -> x.Id)

            connectionString
            |> Sql.connect
            |> Sql.query "INSERT INTO filter_values (filter_id, text) VALUES (@filterId, @filterValue)"
            |> Sql.parameters [ "@filterId", Sql.int filterId; "@filterValue", Sql.string filterValue ]
            |> Sql.executeNonQuery
            |> ignore
        )

    // Return the list of filter values
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM filter_values"
    |> Sql.execute (fun x -> { Id = x.int "id"; FilterId = x.int "filter_id"; Text = x.string "text" }: FilterValue)
    |> Seq.toList

let populateGradesTable(connectionString): Grade list =
    let alreadyPopulated = 
        connectionString
        |> Sql.connect
        |> Sql.query "SELECT COUNT(*) AS count FROM grades"
        |> Sql.execute (fun x -> x.int "count")
        |> Seq.head
        |> (<) 0

    if alreadyPopulated then
        printfn "Grades table already populated"
    else
        Names.gradeNames
        |> Seq.iteri (fun index grade ->
            let points = if index = 0 then None else Some (decimal index - 1m)
            let text = grade

            connectionString
            |> Sql.connect
            |> Sql.query "INSERT INTO grades (points, text) VALUES (@points, @text)"
            |> Sql.parameters [
                "@points", Sql.decimalOrNone points;
                "@text", Sql.string text
            ]
            |> Sql.executeNonQuery
            |> ignore
        )

    // Return the list of grades
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT * FROM grades"
    |> Sql.execute (fun x -> { Id = x.int "id"; Points = x.floatOrNone "points"; Text = x.string "text" }: Grade)
    |> Seq.toList
