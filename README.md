# Setup

To run this project, you'll want to use VSCode and dev containers. The dev container for this project will install all the dependencies you need to run the project.

Once you have the dev container running, the first step is the run the database seeder, you can do this by either running the `.NET Core Launch (DatabaseSeeder)` launch configuration in VSCode, or navigate into the `DatabaseSeeder` project and running `dotnet run` from a terminal inside of the dev container.

# Moving forwards

Now that there's a database seeder that's up and running, I'm going to look at different approaches to getting data from the database and rendering a chart for a user with some very simple filtering. The purpose of this is to give myself a good reference point for looking at how to do this in different ways, but also to be able to compare how different approaches might work.

## Approach 1: Server side with ASP.NET Core

This approach will be to use ASP.NET Core to create a simple API that will return the data that we need to render the chart. This API will have an endpoint that accepts filtering criteria, and any filtering will be performed by the API.

Queries with this will *probably* be written with Entity Framework, but we'll see...

**Note: If the database schema needs to be modified, the database can be re-scaffolded using the `scaffold-database.sh` script in the `BackendCSharp` project**

## Approach 2: JavaScript front end with API back end

This approach will leverage a different backend API which will serve the raw, underlying data rather than the final models. This will mean that the JavaScript will be responsible for filtering the data and rendering the chart.

## Approach 3: Wasm front end with API back end

This approach will be similar to approach 2, but will use a Wasm front end rather than a JavaScript front end. This will mean that the filtering and chart rendering will be done in the Wasm front end. I will probably be using Go for the language the Wasm is written in, but I'm not 100% sure yet.

## Approach ???: ???

I'm not sure if I'll do any more approaches, but if I do, I'll add them here. Possibly looking at using raw SQL on the server side, or maybe using a different language for the server side API, e.g. Rails.
