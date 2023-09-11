module DatabaseSeeder.TableCreation

open DatabaseSeeder.DatabaseUtils

let createSubjectsTable(connectionString) =
    executeQuery connectionString
        "CREATE TABLE IF NOT EXISTS subjects
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            name VARCHAR(255) NOT NULL
        )"

let createFiltersTable(connectionString) =
    executeQuery connectionString
        "CREATE TABLE IF NOT EXISTS filters
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            name VARCHAR(255) NOT NULL
        )"

let createFilterValuesTable(connectionString) =
    executeQuery connectionString
        "CREATE TABLE IF NOT EXISTS filter_values
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            filter_id INTEGER NOT NULL,
            text VARCHAR(255) NOT NULL,
            FOREIGN KEY (filter_id) REFERENCES filters(id)
        )"

let createGradesTable(connectionString) =
    executeQuery connectionString
        "CREATE TABLE IF NOT EXISTS grades
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            points NUMERIC(10, 5) NULL,
            text VARCHAR(255) NOT NULL
        )"

let createStudentsTable(connectionString) =
    executeQuery connectionString
        "CREATE TABLE IF NOT EXISTS students
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            first_name VARCHAR(255) NOT NULL,
            last_name VARCHAR(255) NOT NULL
        )"
let createStudentSubjectsTable(connectionString) =
    executeQuery connectionString
        "CREATE TABLE IF NOT EXISTS student_subjects
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            student_id INTEGER NOT NULL,
            subject_id INTEGER NOT NULL,
            FOREIGN KEY (student_id) REFERENCES students(id),
            FOREIGN KEY (subject_id) REFERENCES subjects(id)
        )"

let createStudentGradesTable(connectionString) =
    executeQuery connectionString
        "CREATE TABLE IF NOT EXISTS student_grades
        (
            id INTEGER PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
            student_id INTEGER NOT NULL,
            subject_id INTEGER NOT NULL,
            grade_id INTEGER NOT NULL,
            FOREIGN KEY (student_id) REFERENCES students(id),
            FOREIGN KEY (subject_id) REFERENCES subjects(id),
            FOREIGN KEY (grade_id) REFERENCES grades(id)
        )"

let createStudentFiltersTable(connectionString) =
    executeQuery connectionString
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
