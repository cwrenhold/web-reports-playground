#!/bin/bash

dotnet ef dbcontext scaffold "Host=$POSTGRES_SERVER; Database=$PROJECT_DB;Username=$POSTGRES_USER;Password=$POSTGRES_PASSWORD" Npgsql.EntityFrameworkCore.PostgreSQL -o Data
