using Agenda.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infrastructure.Persistence.Configurations;

public class ProfissionalConfiguration : IEntityTypeConfiguration<Profissional>
{
    public void Configure(EntityTypeBuilder<Profissional> builder)
    {
        builder.HasKey( p => p.Id );
        builder.Property( p => p.Nome ).IsRequired().HasMaxLength(60);
        // max lenght 40 pra login, max lenght 100 pra senha, login is unique
        builder.Property(p => p.Login).HasMaxLength(40).IsRequired();
        builder.HasIndex(p => p.Login).IsUnique();

        builder.Property(p => p.SenhaHash).HasMaxLength(100).IsRequired();
        builder.Property(p => p.Papel).HasConversion<string>().HasMaxLength(20);
    
    }
}
