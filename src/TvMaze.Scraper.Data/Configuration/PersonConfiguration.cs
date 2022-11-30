using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TvMaze.Scraper.Data.Model;

namespace TvMaze.Scraper.Data.Configuration;

public class PersonConfiguration : IEntityTypeConfiguration<Person>
{
    public void Configure(EntityTypeBuilder<Person> builder)
    {
        builder.HasKey(e => e.PersonId);
        
        builder
            .Property(e => e.PersonId)
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
            .Property(e => e.Birthday)
            .IsRequired();
    }
}