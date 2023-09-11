#!/bin/bash

# Set the password for the postgres user to the value of the POSTGRES_PASSWORD environment variable.
export PGPASSWORD=$POSTGRES_PASSWORD

# Using psql, attempt to connect to the postgres database, try this 10 times waiting 5 seconds between attempts for a total of 50 seconds.
until psql -h $POSTGRES_SERVER -U $POSTGRES_USER -c '\q'; do
  >&2 echo "Postgres is unavailable - sleeping"
  sleep 5
done

# Create the database if it doesn't already exist.
if ! psql -h $POSTGRES_SERVER -U $POSTGRES_USER -tAc "SELECT 1 FROM pg_database WHERE datname='$PROJECT_DB'" | grep -q 1; then
  psql -h $POSTGRES_SERVER -U $POSTGRES_USER -c "CREATE DATABASE $PROJECT_DB"
fi

# Create the dotnet dev certificates
dotnet dev-certs https
