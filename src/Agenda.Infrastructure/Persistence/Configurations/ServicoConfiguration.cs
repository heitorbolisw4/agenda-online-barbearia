using Agenda.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Agenda.Infrastructure.Persistence.Configurations;



public class ServicoConfiguration : IEntityTypeConfiguration<Servico>
{
    public void Configure(EntityTypeBuilder<Servico> builder)
    {
        builder.HasKey( s => s.Id );
        builder.Property( s => s.Nome ).IsRequired().HasMaxLength(60);

        builder.Property( s => s.DuracaoMin ).IsRequired();
    }
}