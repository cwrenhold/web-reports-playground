module DatabaseSeeder.TableCreation

open Npgsql.FSharp

let createSubjectsTable(connectionString) =
    connectionString
    |> Sql.connect
    |> Sql.query 
        "CREATE TABLE IF NOT EXISTS subjects
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            name VARCHAR(255) NOT NULL
        )"
    |> Sql.executeNonQuery

let createFiltersTable(connectionString) =
    connectionString
    |> Sql.connect
    |> Sql.query 
        "CREATE TABLE IF NOT EXISTS filters
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            name VARCHAR(255) NOT NULL
        )"
    |> Sql.executeNonQuery

let createFilterValuesTable(connectionString) =
    connectionString
    |> Sql.connect
    |> Sql.query 
        "CREATE TABLE IF NOT EXISTS filter_values
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            filter_id INTEGER NOT NULL,
            text VARCHAR(255) NOT NULL,
            FOREIGN KEY (filter_id) REFERENCES filters(id)
        )"
    |> Sql.executeNonQuery

let createStudentsTable(connectionString) =
    connectionString
    |> Sql.connect
    |> Sql.query 
        "CREATE TABLE IF NOT EXISTS students
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            first_name VARCHAR(255) NOT NULL,
            last_name VARCHAR(255) NOT NULL
        )"
    |> Sql.executeNonQuery

let createStudentSubjectsTable(connectionString) =
    connectionString
    |> Sql.connect
    |> Sql.query 
        "CREATE TABLE IF NOT EXISTS student_subjects
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            student_id INTEGER NOT NULL,
            subject_id INTEGER NOT NULL,
            FOREIGN KEY (student_id) REFERENCES students(id),
            FOREIGN KEY (subject_id) REFERENCES subjects(id)
        )"
    |> Sql.executeNonQuery

let createStudentGradesTable(connectionString) =
    connectionString
    |> Sql.connect
    |> Sql.query 
        "CREATE TABLE IF NOT EXISTS student_grades
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            student_id INTEGER NOT NULL,
            subject_id INTEGER NOT NULL,
            points NUMERIC(5, 5),
            FOREIGN KEY (student_id) REFERENCES students(id),
            FOREIGN KEY (subject_id) REFERENCES subjects(id)
        )"
    |> Sql.executeNonQuery

let createStudentFiltersTable(connectionString) =
    connectionString
    |> Sql.connect
    |> Sql.query 
        "CREATE TABLE IF NOT EXISTS student_filters
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            student_id INTEGER NOT NULL,
            filter_id INTEGER NOT NULL,
            filter_value_id INTEGER NOT NULL,
            FOREIGN KEY (student_id) REFERENCES students(id),
            FOREIGN KEY (filter_id) REFERENCES filters(id),
            FOREIGN KEY (filter_value_id) REFERENCES filter_values(id)
        )"
    |> Sql.executeNonQuery
