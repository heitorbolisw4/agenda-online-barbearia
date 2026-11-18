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
    }
}
