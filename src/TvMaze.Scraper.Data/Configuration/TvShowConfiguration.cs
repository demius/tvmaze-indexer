using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TvMaze.Scraper.Data.Model;

namespace TvMaze.Scraper.Data.Configuration;

public class TvShowConfiguration : IEntityTypeConfiguration<TvShow>
{
    public void Configure(EntityTypeBuilder<TvShow> builder)
    {
        builder.HasKey(e => e.TvShowId);
        
        builder
            .Property(e => e.TvShowId)
            .ValueGeneratedNever();
        
        builder
            .Property(e => e.Name)
            .IsRequired()
            .HasMaxLength(255);
        
        builder
            .Property(e => e.Url)
            .IsRequired()
            .HasMaxLength(255);
        
        builder
            .HasMany(e => e.CastMembers)
            .WithMany(e => e.TvShows)
            .UsingEntity(j => j.ToTable("cast_members"));
    }
}