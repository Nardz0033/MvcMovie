using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MvcMovie.Data;

#nullable disable

namespace MvcMovie.Migrations;

[DbContext(typeof(ApplicationDbContext))]
[Migration("20261005110000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.0");

        modelBuilder.Entity("MvcMovie.Models.Movie", entity =>
        {
            entity.Property<int>("Id")
                .ValueGeneratedOnAdd()
                .HasColumnType("INTEGER")
                .HasAnnotation("Sqlite:Autoincrement", true);
            entity.Property<string>("Description").IsRequired().HasMaxLength(2000).HasColumnType("TEXT");
            entity.Property<string>("Director").IsRequired().HasMaxLength(120).HasColumnType("TEXT");
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

    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Movies",
            columns: table => new
            {
                Id = table.Column<int>(type: "INTEGER", nullable: false)
                    .Annotation("Sqlite:Autoincrement", true),
                Title = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                Description = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                Genre = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                ReleaseYear = table.Column<int>(type: "INTEGER", nullable: false),
                Rating = table.Column<decimal>(type: "TEXT", nullable: false),
                Duration = table.Column<int>(type: "INTEGER", nullable: false),
                Director = table.Column<string>(type: "TEXT", maxLength: 120, nullable: false),
                PosterImage = table.Column<string>(type: "TEXT", maxLength: 255, nullable: true),
                CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
            },
            constraints: table => table.PrimaryKey("PK_Movies", x => x.Id));

        migrationBuilder.CreateIndex(
            name: "IX_Movies_Title",
            table: "Movies",
            column: "Title");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "Movies");
    }
}
