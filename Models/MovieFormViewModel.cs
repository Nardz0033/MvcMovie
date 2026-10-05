using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MvcMovie.Models;

public class MovieFormViewModel
{
    public int Id { get; set; }

    [Required, StringLength(120, MinimumLength = 1)]
    [Display(Name = "Movie title")]
    public string Title { get; set; } = string.Empty;

    [Required, StringLength(2000, MinimumLength = 10)]
    public string Description { get; set; } = string.Empty;

    [Required, StringLength(40)]
    public string Genre { get; set; } = string.Empty;

    [Range(1888, 2100)]
    [Display(Name = "Release year")]
    public int ReleaseYear { get; set; } = DateTime.Now.Year;

    [Range(typeof(decimal), "0", "10")]
    [Display(Name = "Rating (0–10)")]
    public decimal Rating { get; set; } = 7.0m;

    [Range(1, 600)]
    [Display(Name = "Duration (minutes)")]
    public int Duration { get; set; } = 120;

    [Required, StringLength(120)]
    public string Director { get; set; } = string.Empty;

    [Display(Name = "Poster image")]
    public IFormFile? PosterUpload { get; set; }

    public string? CurrentPosterImage { get; set; }
    public IReadOnlyList<string> Genres { get; } = Movie.GenreOptions;
}
