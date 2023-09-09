module DatabaseSeeder.RandomisedDataSeeding

open FSharp.Data.Sql
open Npgsql.FSharp

open DatabaseSeeder.Names
open DatabaseSeeder.DataTypes

let randomSeed = 1
let randomiser = System.Random(randomSeed)
let totalStudentCount = 1000

let populateStudents(connectionString) =
    let alreadyPopulated = 
        connectionString
        |> Sql.connect
        |> Sql.query "SELECT COUNT(*) AS count FROM students"
        |> Sql.execute (fun x -> x.int "count")
        |> Seq.head
        |> (<) 0

    if alreadyPopulated then
        printfn "Students table already populated"
    else
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
