module DatabaseSeeder.StaticDataSeeding

open Npgsql.FSharp
open DataTypes
open DatabaseUtils

let populateSubjectsTable(connectionString): Subject list =
    let alreadyPopulated = 
        isTablePopulated connectionString "subjects"

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

    selectAllAndMap connectionString "subjects" (fun x -> { Id = x.int "id"; Name = x.string "name" }: Subject)

let populateFiltersTable(connectionString): Filter list =
    let alreadyPopulated = 
        isTablePopulated connectionString "filters"

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

    selectAllAndMap connectionString "filters" (fun x -> { Id = x.int "id"; Name = x.string "name" }: Filter)

let populateFilterValuesTable(connectionString) (filters: Filter list): FilterValue list =
    let alreadyPopulated = 
        isTablePopulated connectionString "filter_values"

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

    selectAllAndMap connectionString "filter_values" (fun x -> { Id = x.int "id"; FilterId = x.int "filter_id"; Text = x.string "text" }: FilterValue)

let populateGradesTable(connectionString): Grade list =
    let alreadyPopulated = 
        isTablePopulated connectionString "grades"

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

    selectAllAndMap connectionString "grades" (fun x -> { Id = x.int "id"; Points = x.floatOrNone "points"; Text = x.string "text" }: Grade)
