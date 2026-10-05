using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MvcMovie.Data;
using MvcMovie.Models;

namespace MvcMovie.Controllers;

public class MoviesController(
    ApplicationDbContext db,
    IWebHostEnvironment environment,
    ILogger<MoviesController> logger) : Controller
{
    private const long MaxPosterBytes = 5 * 1024 * 1024;
    private const long MaxRequestBytes = MaxPosterBytes + 256 * 1024;
    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".jpg", ".jpeg", ".png", ".webp"
    };

    public async Task<IActionResult> Index(string? search, string? genre, CancellationToken cancellationToken)
    {
        search = string.IsNullOrWhiteSpace(search) ? null : search.Trim();
        if (search?.Length > 100)
        {
            search = search[..100];
        }

        genre = genre is not null && Movie.GenreOptions.Contains(genre, StringComparer.OrdinalIgnoreCase) ? genre : null;

        IQueryable<Movie> query = db.Movies.AsNoTracking();
        if (search is not null)
        {
            var pattern = $"%{search}%";
            query = query.Where(movie =>
                EF.Functions.Like(movie.Title, pattern) ||
                EF.Functions.Like(movie.Genre, pattern) ||
                EF.Functions.Like(movie.Director, pattern));
        }

        if (genre is not null)
        {
            query = query.Where(movie => movie.Genre == genre);
        }

        var movies = await query
            .OrderByDescending(movie => movie.Rating)
            .ThenByDescending(movie => movie.ReleaseYear)
            .ToListAsync(cancellationToken);

        var featured = movies.FirstOrDefault() ?? await db.Movies
            .AsNoTracking()
            .OrderByDescending(movie => movie.Rating)
            .FirstOrDefaultAsync(cancellationToken);

        return View(new MovieCatalogViewModel
        {
            Movies = movies,
            Search = search,
            Genre = genre,
            FeaturedMovie = featured
        });
    }

    public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFoundView();
        }

        var movie = await db.Movies.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return movie is null ? NotFoundView() : View(movie);
    }

    public IActionResult Create() => View(new MovieFormViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
    public async Task<IActionResult> Create(MovieFormViewModel form, CancellationToken cancellationToken)
    {
        if (!await ValidatePosterAsync(form.PosterUpload, cancellationToken))
        {
            return View(form);
        }

        ValidateGenre(form.Genre);
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var movie = new Movie();
        ApplyForm(movie, form);
        string? savedPoster = null;
        try
        {
            if (form.PosterUpload is not null)
            {
                savedPoster = await SavePosterAsync(form.PosterUpload, cancellationToken);
                movie.PosterImage = savedPoster;
            }

            db.Movies.Add(movie);
            await db.SaveChangesAsync(cancellationToken);
            TempData["Success"] = $"“{movie.Title}” is now in your collection.";
            return RedirectToAction(nameof(Details), new { id = movie.Id });
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Unable to save the new movie.");
            RemovePoster(savedPoster);
            ModelState.AddModelError(string.Empty, "We couldn't save this movie. Please try again.");
            return View(form);
        }
        catch (IOException exception)
        {
            logger.LogError(exception, "Unable to store the uploaded poster.");
            RemovePoster(savedPoster);
            ModelState.AddModelError(nameof(form.PosterUpload), "The poster couldn't be stored. Please try again.");
            return View(form);
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogError(exception, "Permission denied while storing the uploaded poster.");
            RemovePoster(savedPoster);
            ModelState.AddModelError(nameof(form.PosterUpload), "The poster couldn't be stored. Please try again.");
            return View(form);
        }
    }

    public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFoundView();
        }

        var movie = await db.Movies.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return movie is null ? NotFoundView() : View(ToForm(movie));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(MaxRequestBytes)]
    [RequestFormLimits(MultipartBodyLengthLimit = MaxRequestBytes)]
    public async Task<IActionResult> Edit(int id, MovieFormViewModel form, CancellationToken cancellationToken)
    {
        if (id != form.Id)
        {
            return BadRequest();
        }

        var movie = await db.Movies.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (movie is null)
        {
            return NotFoundView();
        }

        form.CurrentPosterImage = movie.PosterImage;
        if (!await ValidatePosterAsync(form.PosterUpload, cancellationToken))
        {
            return View(form);
        }

        ValidateGenre(form.Genre);
        if (!ModelState.IsValid)
        {
            return View(form);
        }

        var previousPoster = movie.PosterImage;
        string? savedPoster = null;
        try
        {
            ApplyForm(movie, form);
            if (form.PosterUpload is not null)
            {
                savedPoster = await SavePosterAsync(form.PosterUpload, cancellationToken);
                movie.PosterImage = savedPoster;
            }

            await db.SaveChangesAsync(cancellationToken);
            if (savedPoster is not null)
            {
                RemovePoster(previousPoster);
            }

            TempData["Success"] = "Your movie details have been updated.";
            return RedirectToAction(nameof(Details), new { id = movie.Id });
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Unable to update movie {MovieId}.", id);
            RemovePoster(savedPoster);
            ModelState.AddModelError(string.Empty, "We couldn't update this movie. Please try again.");
            return View(form);
        }
        catch (IOException exception)
        {
            logger.LogError(exception, "Unable to store the updated poster for movie {MovieId}.", id);
            RemovePoster(savedPoster);
            ModelState.AddModelError(nameof(form.PosterUpload), "The poster couldn't be stored. Please try again.");
            return View(form);
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogError(exception, "Permission denied while storing the updated poster for movie {MovieId}.", id);
            RemovePoster(savedPoster);
            ModelState.AddModelError(nameof(form.PosterUpload), "The poster couldn't be stored. Please try again.");
            return View(form);
        }
    }

    public async Task<IActionResult> Delete(int? id, CancellationToken cancellationToken)
    {
        if (id is null)
        {
            return NotFoundView();
        }

        var movie = await db.Movies.AsNoTracking().FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        return movie is null ? NotFoundView() : View(movie);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        var movie = await db.Movies.FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (movie is null)
        {
            return NotFoundView();
        }

        try
        {
            db.Movies.Remove(movie);
            await db.SaveChangesAsync(cancellationToken);
            RemovePoster(movie.PosterImage);
            TempData["Success"] = $"“{movie.Title}” has been removed.";
            return RedirectToAction(nameof(Index));
        }
        catch (DbUpdateException exception)
        {
            logger.LogError(exception, "Unable to delete movie {MovieId}.", id);
            TempData["Error"] = "We couldn't delete this movie. Please try again.";
            return RedirectToAction(nameof(Delete), new { id });
        }
    }

    private async Task<bool> ValidatePosterAsync(IFormFile? file, CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return true;
        }

        if (file.Length > MaxPosterBytes)
        {
            ModelState.AddModelError(nameof(MovieFormViewModel.PosterUpload), "Choose an image smaller than 5 MB.");
            return false;
        }

        var extension = Path.GetExtension(file.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            ModelState.AddModelError(nameof(MovieFormViewModel.PosterUpload), "Use a JPG, PNG, or WEBP image.");
            return false;
        }

        var header = new byte[12];
        await using var stream = file.OpenReadStream();
        var bytesRead = await stream.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);
        var validSignature = extension.ToLowerInvariant() switch
        {
            ".jpg" or ".jpeg" => bytesRead >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
            ".png" => bytesRead >= 8 && header.AsSpan(0, 8).SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
            ".webp" => bytesRead >= 12 &&
                       header.AsSpan(0, 4).SequenceEqual("RIFF"u8) &&
                       header.AsSpan(8, 4).SequenceEqual("WEBP"u8),
            _ => false
        };

        if (!validSignature)
        {
            ModelState.AddModelError(nameof(MovieFormViewModel.PosterUpload), "That file doesn't appear to be a valid image.");
        }

        return validSignature;
    }

    private async Task<string> SavePosterAsync(IFormFile file, CancellationToken cancellationToken)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var filename = $"{Guid.NewGuid():N}{extension}";
        var directory = Path.Combine(environment.WebRootPath, "images", "movies");
        Directory.CreateDirectory(directory);

        var path = Path.Combine(directory, filename);
        try
        {
            await using var destination = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
            await file.CopyToAsync(destination, cancellationToken);
            return filename;
        }
        catch
        {
            RemovePoster(filename);
            throw;
        }
    }

    private void RemovePoster(string? filename)
    {
        if (string.IsNullOrWhiteSpace(filename) || Path.GetFileName(filename) != filename ||
            !AllowedExtensions.Contains(Path.GetExtension(filename)))
        {
            return;
        }

        var path = Path.Combine(environment.WebRootPath, "images", "movies", filename);
        try
        {
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }
        }
        catch (IOException exception)
        {
            logger.LogWarning(exception, "Unable to remove poster image {PosterFilename}.", filename);
        }
        catch (UnauthorizedAccessException exception)
        {
            logger.LogWarning(exception, "Permission denied while removing poster image {PosterFilename}.", filename);
        }
    }

    private static void ApplyForm(Movie movie, MovieFormViewModel form)
    {
        movie.Title = form.Title.Trim();
        movie.Description = form.Description.Trim();
        movie.Genre = Movie.GenreOptions.First(genre => string.Equals(genre, form.Genre, StringComparison.OrdinalIgnoreCase));
        movie.ReleaseYear = form.ReleaseYear;
        movie.Rating = form.Rating;
        movie.Duration = form.Duration;
        movie.Director = form.Director.Trim();
    }

    private void ValidateGenre(string genre)
    {
        if (!Movie.GenreOptions.Contains(genre, StringComparer.OrdinalIgnoreCase))
        {
            ModelState.AddModelError(nameof(MovieFormViewModel.Genre), "Choose a genre from the list.");
        }
    }

    private IActionResult NotFoundView()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("NotFound");
    }

    private static MovieFormViewModel ToForm(Movie movie) => new()
    {
        Id = movie.Id,
        Title = movie.Title,
        Description = movie.Description,
        Genre = movie.Genre,
        ReleaseYear = movie.ReleaseYear,
        Rating = movie.Rating,
        Duration = movie.Duration,
        Director = movie.Director,
        CurrentPosterImage = movie.PosterImage
    };
}
