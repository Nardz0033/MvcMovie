namespace MvcMovie.Models;

public class MovieCatalogViewModel
{
    public IReadOnlyList<Movie> Movies { get; init; } = [];
    public IReadOnlyList<string> Genres { get; init; } = Movie.GenreOptions;
    public string? Search { get; init; }
    public string? Genre { get; init; }
    public Movie? FeaturedMovie { get; init; }
}
