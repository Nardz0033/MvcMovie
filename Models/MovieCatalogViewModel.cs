namespace MvcMovie.Models;

public class MovieCatalogViewModel
{
    public IReadOnlyList<Movie> Movies { get; init; } = [];
    public IReadOnlyList<string> Genres { get; init; } = Movie.GenreOptions;
    public string? Search { get; init; }
    public string? Genre { get; init; }
    public Movie? FeaturedMovie { get; init; }
    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int SearchResultCount { get; init; }
    public IReadOnlyDictionary<string, int> GenreCounts { get; init; } = new Dictionary<string, int>();
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
    public int FirstItem => TotalCount == 0 ? 0 : (PageNumber - 1) * PageSize + 1;
    public int LastItem => Math.Min(PageNumber * PageSize, TotalCount);
}
