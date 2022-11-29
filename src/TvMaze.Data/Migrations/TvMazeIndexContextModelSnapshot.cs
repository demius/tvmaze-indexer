﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TvMaze.Data;

#nullable disable

namespace TvMaze.Data.Migrations
{
    [DbContext(typeof(TvMazeIndexContext))]
    partial class TvMazeIndexContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "7.0.0");

            modelBuilder.Entity("TvMaze.Data.Model.Show", b =>
                {
                    b.Property<int>("ShowId")
                        .HasColumnType("INTEGER")
                        .HasColumnName("show_id");

                    b.Property<uint>("LastUpdated")
                        .HasColumnType("INTEGER")
                        .HasColumnName("last_updated");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("TEXT")
                        .HasColumnName("name");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasColumnName("url");

                    b.HasKey("ShowId")
                        .HasName("pk_shows");

                    b.ToTable("shows", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
