module DatabaseSeeder.DatabaseUtils

open Npgsql.FSharp

let executeQuery connectionString query =
    connectionString
    |> Sql.connect
    |> Sql.query query
    |> Sql.executeNonQuery

let isTablePopulated connectionString tableName =
    connectionString
    |> Sql.connect
    |> Sql.query ("SELECT COUNT(*) AS count FROM " + tableName)
    |> Sql.execute (fun x -> x.int "count")
    |> Seq.head
    |> (<) 0

let selectAllAndMap connectionString tableName mapper =
    connectionString
    |> Sql.connect
    |> Sql.query ("SELECT * FROM " + tableName)
    |> Sql.execute mapper
    |> Seq.toList
