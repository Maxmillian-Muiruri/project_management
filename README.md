# project_management

# project_management

A beginner-friendly guide to this project. This README doesn't just tell you *what* the code does — it explains *why* it's built this way, so you can learn from it even if you're new to ASP.NET Core.

---

## 1. What Is This Project?

`project_management` is a **Web API** — a program that doesn't have a visual interface (no buttons or screens), but instead lets other programs (like a mobile app, a website, or a tool like Postman) send it requests over the internet and get data back.

This particular API manages **Products**. It lets other programs:
- Get a list of products
- Get one specific product
- Create a new product
- Update an existing product
- Delete a product

This is called **CRUD** — **C**reate, **R**ead, **U**pdate, **D**elete. Almost every app that stores data (a to-do list, an online store, a blog) needs these four basic operations, so CRUD APIs are one of the most common things you'll build as a developer.

It's built using **ASP.NET Core**, a framework made by Microsoft for building web applications and APIs in the **C#** programming language, and targets **.NET 10**, which is the version of the underlying platform/runtime that powers it.

### Why is it organized into layers?

Instead of putting all the code in one giant file, this project splits responsibilities into separate layers:

- **Controllers** — handle incoming web requests (the "front door")
- **Services** — contain the actual business logic (the "brain")
- **Data / EF Core** — talk to the database (the "memory")
- **DTOs** — define what data looks like when it enters or leaves the API (the "shape of the conversation")

Think of it like a restaurant: the **Controller** is the waiter taking your order, the **Service** is the chef deciding how to cook it, and the **Data layer** is the pantry where ingredients (data) are stored. Keeping these separate means you can change how food is cooked (business logic) without changing how orders are taken (the API), and vice versa.

It also uses **Swagger / OpenAPI**, which is a tool that automatically generates a webpage where you can see and test every endpoint (URL) this API offers, without needing a separate app like Postman.

---

## 2. Getting Started (Developer Setup)

### What you need before you start

- **.NET 10 SDK** — this is the toolkit (compiler, runtime, command-line tools) needed to build and run .NET applications. Without it, commands like `dotnet run` won't work.
- **SQL Server** — this is the database system this project is set up to use by default. A database is where your product data actually gets saved permanently (as opposed to memory, which disappears when the app stops).

### Step-by-step setup

**Step 1: Tell the app how to find your database**

Open the `appsettings.json` file (a configuration file the app reads on startup) and add a **connection string** — a piece of text that tells the app the database's address, name, and login details:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=project_management;Trusted_Connection=True;"
  }
}
```

Here, `Server=.` means "use the SQL Server running on this same computer," `Database=project_management` is the name of the database to use (or create), and `Trusted_Connection=True` means "log in using my Windows account" instead of a username/password.

**Step 2: Create the database tables**

This project uses a feature called **migrations** — versioned instructions that tell the database how to build (or update) its tables to match the C# code.

```bash
dotnet tool install --global dotnet-ef   # only needed once, installs the migration tool
dotnet ef database update                # actually creates/updates the database
```

You only need to install `dotnet-ef` once on your machine. `dotnet ef database update` can be run every time the project's data structure changes.

**Step 3: Run the app**

```bash
dotnet run
```

This compiles the code and starts the API listening on a local address, usually something like `https://localhost:5001`. Since the project runs in "Development" mode by default, it also turns on Swagger, so you can open that address in your browser and see a live, testable list of every endpoint.

> **Heads-up:** If you skip Step 1 and there's no connection string, the app will crash the moment it tries to talk to the database — not before. So if `dotnet run` works but everything errors out, check `appsettings.json` first.

---

## 3. High-Level Architecture (The Big Picture)

Here's how a single request flows through the app, from start to finish:

```
Client (browser, app, Postman)
        ↓
   Controller   ← receives the HTTP request, decides what to do
        ↓
    Service     ← contains business logic, decides how to fulfill it
        ↓
  AppDbContext  ← talks to the actual database
        ↓
    Database    ← where the data is permanently stored
```

- **Presentation layer (Controllers):** the entry point for HTTP requests — like `GET api/products`.
- **Application layer (Services):** where the actual logic lives — e.g., "find this product," "make sure the price isn't negative," "convert this to the shape the client expects."
- **Persistence layer (Models + Data):**
  - **Models** are C# classes that represent a table in the database (e.g., a `Product` class represents the Products table).
  - **`AppDbContext`** is the object that manages the connection to the database and lets you query/save data using C# instead of writing raw SQL.
- **Contracts (DTOs):** special classes used only for sending/receiving data through the API, kept separate from the database models.
- **Cross-cutting concerns (`Program.cs`):** the startup file where everything — dependency injection, the database connection, Swagger — gets wired together.

### Why bother separating all of this?

- **Easier to test:** you can test the business logic (Services) without needing a real database.
- **Easier to change:** if you switch databases later, you mostly only touch the Data layer.
- **Safer APIs:** using DTOs means clients only ever see the fields you intend to expose — not your raw internal database structure.
- **Flexible:** Dependency Injection (explained below) lets you swap out pieces (like using a fake service for testing) without rewriting the whole app.

---

## 4. Project Layout and File-by-File Breakdown

### `Program.cs` — the starting point of the whole application

Every .NET app needs an entry point — the first code that runs. In modern .NET, this is written using the **minimal hosting model**: instead of a lot of boilerplate setup code, you write simple top-to-bottom statements.

In this file:
- `AddControllers()` tells the app "I'm going to use Controllers to handle web requests."
- `AddDbContext<AppDbContext>(options => options.UseSqlServer(...))` registers the database connection, so any part of the app can ask for `AppDbContext` and get one that's properly connected.
- `AddOpenApi()`, `AddSwaggerGen()`, `MapOpenApi()`, and `SwaggerUI` all work together to build that interactive Swagger documentation page.
- `builder.Services.AddScoped<IProductService, ProductService>()` is **Dependency Injection** in action (explained in detail in Section 5) — it tells the app "whenever something asks for an `IProductService`, give it a `ProductService`."

### `Properties/launchSettings.json` — local run settings

This file isn't shipped to production — it only affects how the app behaves when you run it on your own machine, e.g., which port it uses (`https://localhost:5001`) and whether it runs in "Development" or "Production" mode.

### `Controllers/productsController.cs` — the API's "front door"

This defines the actual URLs people can call:

| HTTP Method | URL | What it does |
|---|---|---|
| `GET` | `/api/products` | Get a list of all products |
| `GET` | `/api/products/{id}` | Get one specific product by its ID |
| `POST` | `/api/products` | Create a new product |
| `PUT` | `/api/products/{id}` | Update an existing product |
| `DELETE` | `/api/products/{id}` | Delete a product |

`GET`, `POST`, `PUT`, and `DELETE` are **HTTP methods** — a standard way of telling a server what *kind* of action you want to perform on a resource, built into how the web works.

The controller doesn't do the actual work itself — it receives the request, hands it off to the `IProductService`, and then returns a response with an appropriate **status code**, such as:
- `200 OK` — success, here's your data
- `201 Created` — success, a new resource was made
- `204 No Content` — success, but nothing to send back (common after a delete)
- `404 Not Found` — the thing you asked for doesn't exist
- `500` — something went wrong on the server

### `Services/IProductService.cs` and `Services/ProductService.cs` — the "brain"

`IProductService` is an **interface** — a contract that says "any class implementing me must have these methods," without saying *how* those methods work.

`ProductService` is the actual implementation — the class that does the real work: fetching products from the database, saving new ones, updating and deleting them, and converting between the database format and the API format.

Why have both an interface and a class? Because it means other code (like the Controller) only needs to know *what* an `IProductService` can do, not *how*. This makes it easy to swap in a fake version for testing later.

### `Models/Product.cs` — what a product looks like in the database

This is a plain C# class whose properties map directly to columns in the database's `Products` table (e.g., `Id`, `Name`, `Price`). The `Id` is expected to auto-increment — meaning the database assigns it automatically each time a new row is added, so you never need to set it yourself.

### `Dtos/ProductRequest.cs` and `Dtos/ProductResponse.cs` — the API's public "shape"

**DTO** stands for **Data Transfer Object**. These are simple classes used purely for sending and receiving data through the API — they are *not* the same as the database Model, even though they might look similar.

- `ProductRequest` defines what a client must send when creating or updating a product.
- `ProductResponse` defines what the API sends back.

Keeping these separate from `Product` (the database model) means you can change your database structure without automatically changing what the outside world sees, and vice versa — a very common and important practice in API design.

### `Data/AppDbContext.cs` — the bridge to the database

This class extends EF Core's `DbContext`, and defines a `DbSet<Product> Products`. A **`DbSet<T>`** represents a table in the database as a C# collection — so instead of writing SQL like `SELECT * FROM Products`, you can write `context.Products.ToList()` in C#.

### `Migrations/` — a history of database changes

Each migration file is a snapshot of a change made to the database structure over time (e.g., "add a Price column"). This lets you (or a teammate) recreate the exact same database structure on a different machine just by running `dotnet ef database update`.

### `project_management.csproj` — the project's settings file

This XML file tells the .NET build tools which version of .NET to target (`net10.0`) and which external packages (libraries) the project depends on. Think of it like a shopping list of pre-built code the project relies on instead of writing everything from scratch.

---

## 5. Core C# & .NET Concepts Explained

If you're newer to C#/.NET, these are the ideas worth understanding well, since they show up constantly in real-world projects.

### Dependency Injection (DI)

Instead of a class creating the things it depends on itself (e.g., `ProductService` creating its own database connection), it *asks* for them, and a central system (the DI container) hands them over. This is set up with lines like:

```csharp
builder.Services.AddScoped<IProductService, ProductService>();
```

This means "whenever some part of the app needs an `IProductService`, give it a `ProductService`, and create a fresh one for each web request." That "fresh one per request" behavior is called **Scoped lifetime** — there are also `Singleton` (one instance shared forever) and `Transient` (a new instance every single time it's requested) lifetimes, each suited to different situations.

**Why it matters:** DI makes code easier to test (you can substitute a fake dependency) and easier to maintain (you change *how* something works in one place, not everywhere it's used).

### Entity Framework Core (EF Core)

EF Core is an **ORM** — an Object-Relational Mapper. It lets you interact with a database using C# objects and methods instead of writing raw SQL queries by hand.

- `DbContext` represents an active session talking to the database.
- `DbSet<T>` represents one table.
- Common methods:
  - `Add(entity)` — marks a new object to be inserted
  - `Find(id)` — looks up a record by its primary key
  - `Remove(entity)` — marks a record for deletion
  - `SaveChanges()` — actually sends all pending changes to the database (nothing is saved until you call this)

### Nullable reference types and `required`

By default, C# lets any object reference be `null` (i.e., "pointing at nothing"), which is a common source of bugs (`NullReferenceException`). Turning on **nullable reference types** (`<Nullable>enable</Nullable>`) makes the compiler warn you whenever something *might* be `null` but you're treating it like it can't be.

The `required` keyword goes a step further: it forces whoever creates an object to explicitly provide a value for that property, or the code won't compile. This catches mistakes early, before the app even runs.

### Collection types: `IEnumerable<T>`, `List<T>`, `IQueryable<T>`

These are all ways of representing "a group of things" in C#, but they behave differently:

- **`List<T>`** — a real, in-memory collection you can loop through, add to, index into (`list[0]`), etc. Once created, it holds actual data.
- **`IEnumerable<T>`** — a more general promise that says "you can loop through me," without guaranteeing the data already exists in memory. Sometimes the data is computed *as you loop*, which is called **deferred execution**.
- **`IQueryable<T>`** — specifically used by EF Core to build up a database query in C# before it's actually run. Extra `.Where()` or `.Select()` calls added to an `IQueryable` change the actual SQL sent to the database, rather than filtering data that's already been loaded.

**Why this matters:** if you return an `IEnumerable<T>` or `IQueryable<T>` straight from a database query without converting it to a `List`, the data might not actually be fetched from the database until much later — potentially *after* the connection has already closed, causing confusing errors. The safe habit is to call `.ToList()` before returning data out of the Service layer.

### LINQ (Language Integrated Query)

LINQ lets you write query-like code directly in C#, e.g.:

```csharp
var cheapProducts = context.Products.Where(p => p.Price < 10).ToList();
```

This reads almost like plain English: "get all products where the price is less than 10." Behind the scenes, EF Core translates this into actual SQL.

### `var`

`var` just tells the compiler "figure out the type yourself from context." It doesn't make C# dynamically typed — the variable still has a fixed, specific type — it's just a shorthand so you don't have to write it out explicitly.

### OpenAPI, Swagger, and Scalar

- **OpenAPI** is a standard format for describing what an API can do (its endpoints, inputs, and outputs) in a machine-readable way.
- **Swagger** generates a friendly, interactive webpage from that description, where you can try out endpoints directly in your browser.
- **Scalar.AspNetCore** is an additional tool used alongside OpenAPI to enhance or customize how that API documentation is generated or displayed.

---

## 6. External Dependencies (Packages Used)

A **package** (or NuGet package, in the .NET world) is a chunk of pre-built code that someone else wrote, which you can add to your project instead of writing everything yourself.

| Package | Version | What it's for |
|---|---|---|
| `Microsoft.AspNetCore.OpenApi` | — | Adds built-in support for generating OpenAPI documentation |
| `Microsoft.EntityFrameworkCore` | 10.0.11 | The core EF Core engine — change tracking, queries, migrations |
| `Microsoft.EntityFrameworkCore.SqlServer` | 10.0.11 | Lets EF Core specifically talk to SQL Server databases |
| `Microsoft.EntityFrameworkCore.Tools` | 10.0.11 | Command-line tools for creating and applying migrations |
| `Scalar.AspNetCore` | 2.17.2 | Enhances the generated OpenAPI/API reference experience |
| `Swashbuckle.AspNetCore` | 10.2.3 | Generates the interactive Swagger UI |

---

## 7. How Routing and Naming Work

**Routing** is how the app decides which piece of code should handle an incoming URL. This project uses attribute-based routing:

```csharp
[Route("api/[controller]")]
```

The `[controller]` part automatically gets replaced with the controller's class name, minus the word "Controller." So a class named `ProductsController` automatically handles URLs starting with `api/products` — without you needing to type that path out manually.

---

## 8. Persistence and Migration Workflow (Step by Step)

Whenever you change a Model class (e.g., you add a new property to `Product`), the database doesn't automatically know about it. You need to create and apply a migration:

**Step 1 — Create a migration** (a record of what changed):
```bash
dotnet ef migrations add InitialCreate
```
`InitialCreate` here is just a descriptive name you choose for the migration — later ones might be named things like `AddPriceColumn`.

**Step 2 — Apply the migration to the actual database:**
```bash
dotnet ef database update
```

Until you run this second command, your database structure won't reflect your code changes.

---

## 9. Best-Practice Recommendations

These are habits worth adopting as you build on this project:

- **Keep DTOs and database Models separate.** Don't let your API accidentally expose internal database fields.
- **Use `AsNoTracking()`** for read-only queries — this tells EF Core "I'm not going to modify this data, so don't bother tracking changes to it," which improves performance.
- **Avoid returning `IQueryable<T>`** from a Service — always convert to a `List` first, so the database query fully finishes inside the Service layer.
- **Consider a mapping library like AutoMapper** once you have many DTOs — manually writing conversion code between Models and DTOs gets repetitive fast.
- **Add centralized error-handling middleware** so all unexpected errors get turned into consistent, user-friendly responses instead of raw stack traces.
- **Write tests** — unit tests for your Services (business logic) and integration tests for your Controllers (the full request/response cycle).

---

## 10. Running & Debugging Tips

- **Visual Studio users:** launch profiles in `Properties/launchSettings.json` let you debug using either IIS Express or Kestrel (ASP.NET Core's built-in lightweight web server) — you can pick which one from the dropdown next to the "Run" button.
- **Swagger UI** is available automatically when running in Development mode — it's the fastest way to manually try out an endpoint without writing any test code.
- **If you hit an EF Core exception:** the first three things to check are (1) is your connection string correct, (2) have you actually applied your latest migration, and (3) are you accidentally setting an `Id` value yourself when the database expects to generate it automatically.