# Setup

To run this project, you'll want to use VSCode and dev containers. The dev container for this project will install all the dependencies you need to run the project.

Once you have the dev container running, the first step is the run the database seeder, you can do this by either running the `.NET Core Launch (DatabaseSeeder)` launch configuration in VSCode, or navigate into the `DatabaseSeeder` project and running `dotnet run` from a terminal inside of the dev container.

## Approach 1: Server side with ASP.NET Core

In this approach, an ASP.NET Core application acts as a backend to generate the report data. The data is taken directly from the database using Entity Framework Core, and then a view is rendered using Razor Pages.

In this version, clients need very little processing - from their side of things, they receive a fully rendered HTML page, and all they need to do is execute a small amount of JavaScript to render the chart in the web browser.

This backend will also be used to provide raw data API for front end approaches. In this setup, a series of simple querie are executed against the database, and that normalised data is then sent to the client with very little processing.

This server can be run using the `.NET Core Launch (BackendCSharp)` launch configuration in VSCode, or by running `dotnet run` from a terminal inside of the dev container. As this is also required for the front end versions, it is recommended to run this first.

**Note: If the database schema needs to be modified, the database can be re-scaffolded using the `scaffold-database.sh` script in the `BackendCSharp` project**

## Approach 2: JavaScript front end with API back end

This approach will depend on the raw data API provided by the ASP.NET Core backend. The front end here will be written in TypeScript, which will then be transpiled to JavaScript. The front end will then use the raw data API to get the data it needs, and then it will do the filtering and chart rendering in the front end.

This will mean that the client will be taking very raw data and collating it on their end rather than the server side. This will mean that the server side will be doing less work, but the client side will be doing more work.

To run this version, you'll need to run the ASP.NET Core backend first, and then you can run the `Launch TypeScript Frontend` launch configuration in VSCode, or run `npm run dev` from a terminal inside of the dev container.

## Approach 3: Wasm front end with API back end

In this version, Go is used to write a WASM front end. This will be similar to the JavaScript front end, but the front end will be written in Go instead of TypeScript. This will mean that the front end will be compiled to WASM, and then the client will be able to run the WASM in the browser.

WASM is an interesting approach to this (for myself, at least), as it, in theory, should allow for a more performant front end, as WASM is compiled to machine code, rather than being interpreted like JavaScript. This should mean that the client side will be doing more work, but it should be able to do it faster.

To run this version, you'll need to run the ASP.NET Core backend first, and then you can run the `build-frontend-go` task in VSCode inside of the dev container. After this has been built, you can navigate to the nginx server which is hosting the WASM front end by loading the `http://localhost:8004` URL in your browser.

## Approach ???: ???

I'm not sure if I'll do any more approaches, but if I do, I'll add them here. Possibly looking at using raw SQL on the server side, or maybe using a different language for the server side API, e.g. Rails.
