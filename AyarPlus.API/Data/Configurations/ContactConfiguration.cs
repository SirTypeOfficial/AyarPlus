using AyarPlus.API.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AyarPlus.API.Data.Configurations;

public class ContactConfiguration : IEntityTypeConfiguration<Contact>
{
    public void Configure(EntityTypeBuilder<Contact> builder)
    {
        builder.ToTable("Contacts");

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(200);

        builder.Property(c => c.Email)
            .HasMaxLength(200);

        builder.Property(c => c.Phone)
            .HasMaxLength(50);

        builder.Property(c => c.Type)
            .HasMaxLength(50);

        builder.Property(c => c.TaxNumber)
            .HasMaxLength(50);

        builder.Property(c => c.Address)
            .HasMaxLength(500);

        builder.Property(c => c.City)
            .HasMaxLength(100);

        builder.Property(c => c.ZipCode)
            .HasMaxLength(20);

        builder.Property(c => c.State)
            .HasMaxLength(100);

        builder.Property(c => c.Country)
            .HasMaxLength(100);

        builder.Property(c => c.Website)
            .HasMaxLength(300);

        builder.Property(c => c.CurrencyCode)
            .HasMaxLength(10);

        builder.Property(c => c.Reference)
            .HasMaxLength(100);

        builder.Property(c => c.CreatedFrom)
            .HasMaxLength(100);

        builder.Property(c => c.FileNumber)
            .HasMaxLength(100);

        builder.Property(c => c.FrontImagePath)
            .HasMaxLength(500);

        builder.Property(c => c.BackImagePath)
            .HasMaxLength(500);

        builder.HasIndex(c => c.Email);
        builder.HasIndex(c => c.Phone);
        builder.HasIndex(c => c.DeletedAt);
    }
}

