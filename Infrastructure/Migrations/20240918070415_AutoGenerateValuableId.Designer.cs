﻿// <auto-generated />
using System;
using Infrastructure.Database.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Infrastructure.Migrations
{
    [DbContext(typeof(KvittaDbContext))]
    [Migration("20240918070415_AutoGenerateValuableId")]
    partial class AutoGenerateValuableId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.7")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("Infrastructure.Database.Models.Valuable", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("character varying(25)");

                    b.Property<DateTimeOffset>("PurchaseDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<double>("Value")
                        .HasColumnType("double precision");

                    b.Property<Guid?>("WarrantyId")
                        .HasColumnType("uuid");

                    b.HasKey("Id");

                    b.HasIndex("WarrantyId");

                    b.ToTable("Valuables");
                });

            modelBuilder.Entity("Infrastructure.Database.Models.Warranty", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uuid");

                    b.Property<string>("Description")
                        .HasMaxLength(255)
                        .HasColumnType("character varying(255)");

                    b.Property<DateOnly>("ExpirationDate")
                        .HasColumnType("date");

                    b.HasKey("Id");

                    b.ToTable("Warranties");
                });

            modelBuilder.Entity("Infrastructure.Database.Models.Valuable", b =>
                {
                    b.HasOne("Infrastructure.Database.Models.Warranty", "Warranty")
                        .WithMany()
                        .HasForeignKey("WarrantyId");

                    b.Navigation("Warranty");
                });
#pragma warning restore 612, 618
        }
    }
}
