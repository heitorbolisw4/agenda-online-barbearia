using Agenda.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infrastructure.Persistence.Configurations;


public class AgendamentoConfiguration : IEntityTypeConfiguration<Agendamento>
{
    public void Configure(EntityTypeBuilder<Agendamento> builder)
    {
        builder.Property( a => a.Status ).HasConversion<string>();
        builder.Property( a => a.Origem ).HasConversion<string>();

        // O Npgsql só grava "timestamp with time zone" em UTC (offset 0).
        // O cliente manda o horário em -03:00, então convertemos antes de gravar.
        builder.Property( a => a.Inicio ).HasConversion( v => v.ToUniversalTime(), v => v );
        builder.Property( a => a.Fim ).HasConversion( v => v.ToUniversalTime(), v => v );
        builder.Property( a => a.ChegadaEm ).HasConversion( v => v.HasValue ? v.Value.ToUniversalTime() : v, v => v );
    }
}