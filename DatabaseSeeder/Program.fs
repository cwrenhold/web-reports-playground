open FSharp.Data.Sql
open Npgsql.FSharp

open DatabaseSeeder.TableCreation
open DatabaseSeeder.StaticDataSeeding
open DatabaseSeeder.RandomisedDataSeeding

let envVars = 
    System.Environment.GetEnvironmentVariables()
    |> Seq.cast<System.Collections.DictionaryEntry>
    |> Seq.map (fun d -> d.Key :?> string, d.Value :?> string)
    |> dict

let connectionString =
    Sql.host envVars.["POSTGRES_SERVER"]
    |> Sql.database envVars.["PROJECT_DB"]
    |> Sql.username envVars.["POSTGRES_USER"]
    |> Sql.password envVars.["POSTGRES_PASSWORD"]
    |> Sql.port (int envVars.["POSTGRES_PORT"])
    |> Sql.includeErrorDetail true
    |> Sql.formatConnectionString

// Select top 1 from database to check connection works
let checkConnection =
    connectionString
    |> Sql.connect
    |> Sql.query "SELECT 1 AS check"
    |> Sql.execute (fun x -> x.int "check")
    |> Seq.head

// Print the result of the check
if checkConnection = 1 then
    printfn "Connection to database successful"
else
    printfn "Connection to database failed"
    exit 1

// Run the table creation queries
createSubjectsTable connectionString |> ignore
createFiltersTable connectionString |> ignore
createFilterValuesTable connectionString |> ignore
createGradesTable connectionString |> ignore
createStudentsTable connectionString |> ignore
createStudentSubjectsTable connectionString |> ignore
createStudentGradesTable connectionString |> ignore
createStudentFiltersTable connectionString |> ignore

printfn "Tables created"

// Run the population scripts
let subjects = populateSubjectsTable connectionString
let filters = populateFiltersTable connectionString
let filterValues = populateFilterValuesTable connectionString filters
let grades = populateGradesTable connectionString

printfn "Static tables populated"

// Run the randomised population scripts
let students = populateStudents connectionString
let studentFilterValues = populateStudentFilter connectionString students filters filterValues
let studetGrades = populateStudentGrades connectionString students subjects grades
let studentSubjects = populateStudentSubjects connectionString students subjects

printfn "Randomised tables populated"
