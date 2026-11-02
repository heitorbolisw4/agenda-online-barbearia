using Agenda.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Persistence;



public class AgendaDbContext( DbContextOptions<AgendaDbContext> options ) : DbContext( options )
{
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();
    public DbSet<Servico> Servicos => Set<Servico>();
    public DbSet<PeriodoExpediente> Expedientes => Set<PeriodoExpediente>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgendaDbContext).Assembly);

    }

}