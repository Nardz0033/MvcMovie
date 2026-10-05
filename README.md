# NardzMovie · Movie Catalog

A cinematic ASP.NET Core MVC movie collection powered by Entity Framework Core and SQLite.

## Run locally

Install the .NET 10 SDK, then run the app from this directory:

```powershell
dotnet run
```

The app uses `Data Source=movies.db`. At startup it applies checked-in EF Core migrations and seeds a starter collection when the database is empty. The SQLite database is created in the application content root.

## Movie posters

Add a poster from a movie's create or edit form. JPG, PNG, and WEBP uploads up to 5 MB are accepted and stored under `wwwroot/images/movies` with generated filenames. Uploaded images are not stored in the database. Movies without a poster use the local artwork at `wwwroot/images/movie-placeholder.svg`.

## Database migrations

The initial migration is checked in. With the EF Core CLI installed, create and apply later migrations using:

```powershell
dotnet ef migrations add DescribeYourChange
dotnet ef database update
```