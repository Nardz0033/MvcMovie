using Microsoft.EntityFrameworkCore;
using MvcMovie.Models;

namespace MvcMovie.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(ApplicationDbContext db)
    {
        if (await db.Movies.AnyAsync())
        {
            return;
        }

        var movies = new[]
        {
            new Movie
            {
                Title = "Inception",
                Description = "A skilled thief enters the dreams of others to steal secrets, then attempts the impossible: planting an idea.",
                Genre = "Sci-Fi",
                ReleaseYear = 2010,
                Rating = 8.8m,
                Duration = 148,
                Director = "Christopher Nolan",
                CreatedAt = DateTime.UtcNow
            },
            new Movie
            {
                Title = "Interstellar",
                Description = "A team of explorers travels beyond this galaxy to discover whether mankind has a future among the stars.",
                Genre = "Sci-Fi",
                ReleaseYear = 2014,
                Rating = 8.7m,
                Duration = 169,
                Director = "Christopher Nolan",
                CreatedAt = DateTime.UtcNow
            },
            new Movie
            {
                Title = "The Dark Knight",
                Description = "Batman faces a criminal mastermind whose reign of chaos tests the limits of Gotham and its hero.",
                Genre = "Action",
                ReleaseYear = 2008,
                Rating = 9.0m,
                Duration = 152,
                Director = "Christopher Nolan",
                CreatedAt = DateTime.UtcNow
            },
            new Movie
            {
                Title = "Spider-Man: Into the Spider-Verse",
                Description = "Miles Morales discovers that more than one person can wear the mask in this bold animated adventure.",
                Genre = "Animation",
                ReleaseYear = 2018,
                Rating = 8.4m,
                Duration = 117,
                Director = "Bob Persichetti",
                CreatedAt = DateTime.UtcNow
            },
            new Movie
            {
                Title = "Avengers: Endgame",
                Description = "The Avengers assemble once more for a final stand that will decide the fate of the universe.",
                Genre = "Action",
                ReleaseYear = 2019,
                Rating = 8.4m,
                Duration = 181,
                Director = "Anthony Russo",
                CreatedAt = DateTime.UtcNow
            },
            new Movie
            {
                Title = "La La Land",
                Description = "An aspiring actor and a jazz musician fall in love while chasing their dreams in Los Angeles.",
                Genre = "Romance",
                ReleaseYear = 2016,
                Rating = 8.0m,
                Duration = 128,
                Director = "Damien Chazelle",
                CreatedAt = DateTime.UtcNow
            }
        };

        db.Movies.AddRange(movies);
        await db.SaveChangesAsync();
    }
}
