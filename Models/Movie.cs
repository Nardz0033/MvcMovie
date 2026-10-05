using System.ComponentModel.DataAnnotations;

namespace MvcMovie.Models;

public class Movie
{
    public static readonly string[] GenreOptions =
    [
        "Action", "Comedy", "Drama", "Horror", "Romance", "Sci-Fi", "Animation", "Thriller"
    ];

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
    public int ReleaseYear { get; set; }

    [Range(typeof(decimal), "0", "10")]
    public decimal Rating { get; set; }

    [Range(1, 600)]
    [Display(Name = "Duration (minutes)")]
    public int Duration { get; set; }

    [Required, StringLength(120)]
    public string Director { get; set; } = string.Empty;

    [StringLength(255)]
    public string? PosterImage { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
