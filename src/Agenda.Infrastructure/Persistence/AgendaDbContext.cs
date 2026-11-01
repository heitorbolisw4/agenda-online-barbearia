using Agenda.Domain;
using Microsoft.EntityFrameworkCore;

namespace Agenda.Infrastructure.Persistence;



public class AgendaDbContext( DbContextOptions<AgendaDbContext> options ) : DbContext( options )
{
    public DbSet<Agendamento> Agendamentos => Set<Agendamento>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AgendaDbContext).Assembly);

    }

}