using FriendOrganizer.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FriendOrganizer.DataAccess.EntityConfiguration
{
    public class ProgrammingLanguageConfiguration : IEntityTypeConfiguration<ProgrammingLanguage>
    {
        public void Configure(EntityTypeBuilder<ProgrammingLanguage> builder)
        {
            builder.ToTable(builder.Metadata.ClrType.Name);
            builder.Property(pl => pl.Name).IsRequired();
            builder.Property(pl => pl.Name).HasMaxLength(50);

            builder.HasData(
                new ProgrammingLanguage { Id = 1, Name = "C#" },
                new ProgrammingLanguage { Id = 2, Name = "Python" },
                new ProgrammingLanguage { Id = 3, Name = "Swift" },
                new ProgrammingLanguage { Id = 4, Name = "Java" },
                new ProgrammingLanguage { Id = 5, Name = "F#" });
        }
    }
}
