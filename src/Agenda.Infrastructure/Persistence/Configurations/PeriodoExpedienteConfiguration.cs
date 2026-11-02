using Agenda.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infrastructure.Persistence.Configurations;


public class PeriodoExpedienteConfiguration : IEntityTypeConfiguration<PeriodoExpediente>
{
    public void Configure(EntityTypeBuilder<PeriodoExpediente> builder)
    {
        builder.HasKey( p => p.Id );
    }
}