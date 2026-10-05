using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using MvcMovie.Data;

#nullable disable

namespace MvcMovie.Migrations;

[DbContext(typeof(ApplicationDbContext))]
public partial class ApplicationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.0");

        modelBuilder.Entity("MvcMovie.Models.Movie", entity =>
        {
            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                .HasAnnotation("Sqlite:Autoincrement", true);

            entity.Property<string>("Description")
                .IsRequired()
                .HasMaxLength(2000)
                .HasColumnType("TEXT");

            entity.Property<string>("Director")
                .IsRequired()
                .HasMaxLength(120)
                .HasColumnType("TEXT");

            entity.Property<int>("Duration").HasColumnType("INTEGER");
            entity.Property<string>("Genre").IsRequired().HasMaxLength(40).HasColumnType("TEXT");
            entity.Property<decimal>("Rating").HasColumnType("TEXT");
            entity.Property<string>("PosterImage").HasMaxLength(255).HasColumnType("TEXT");
            entity.Property<int>("ReleaseYear").HasColumnType("INTEGER");
            entity.Property<string>("Title").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
            entity.Property<DateTime>("CreatedAt").HasColumnType("TEXT");

            entity.HasKey("Id");
            entity.HasIndex("Title");
            entity.ToTable("Movies");
        });
    }
}
