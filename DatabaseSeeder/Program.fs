open FSharp.Data.Sql
open Npgsql.FSharp

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
