using Agenda.Domain.Enum;
using Agenda.Domain.Exceptions;

namespace Agenda.Domain;

public class Agendamento
{
    private Agendamento(Guid profissionalId, Guid servicoId, string servicoNome, int duracaoMin, string clienteNome, string clienteTelefone, DateTimeOffset inicio, DateTimeOffset fim)
    {

        ProfissionalId = profissionalId;
        ServicoId = servicoId;
        ServicoNome = servicoNome;
        DuracaoMin = duracaoMin;
        ClienteNome = clienteNome;
        ClienteTelefone = clienteTelefone;
        Inicio = inicio;
        Fim = fim;
    }

    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Codigo { get; private  set; } = string.Empty;
    public  Guid ProfissionalId { get; private set; }
    public Guid ServicoId { get; private set; }
    public string ServicoNome { get; private set; }
    public int DuracaoMin { get; private set; }
    public DateTimeOffset Inicio { get; private set; }
    public DateTimeOffset Fim { get; private set; }
    public string ClienteNome { get; private set; }
    public string ClienteTelefone { get; private set; }
    public StatusAgendamento Status { get; private set; } = StatusAgendamento.Confirmado;
    public Origem Origem { get; private set; }
    public DateTimeOffset? ChegadaEm { get; private set; }
    
    public string TokenHash { get; private set; }  = string.Empty;
    public Guid ChaveIdempotencia { get; private set; }

    

    public static Agendamento Criar(Guid profissionalId, Guid servicoId, string servicoNome, int duracaoMin, string clienteNome, string clienteTelefone, DateTimeOffset inicio)
    {
        ValidProfissionalId(profissionalId);
        ValidServico(servicoId, servicoNome);
        ValidDuracao(duracaoMin);
        ValidCliente(clienteNome, clienteTelefone);
        var fim = inicio.AddMinutes(duracaoMin);
        return new Agendamento(profissionalId, servicoId, servicoNome ,duracaoMin, clienteNome, clienteTelefone, inicio, fim);
    }

    public void Cancelar()
    {
        if( Status != StatusAgendamento.Confirmado)
        {
            throw new DomainException("O Agendamento nao pode ser CANCELADO!");
        }
        Status = StatusAgendamento.Cancelado;
    }

    private static void ValidProfissionalId(Guid profissionalId)
    {
        if( profissionalId == Guid.Empty )
        {
            throw new DomainException("Deve ser informado um profissional valido");
        }
    }
    private static void ValidServico(Guid servicoId, string servicoNome)
    {
        if( servicoId == Guid.Empty )
        {
            throw new DomainException("Deve ser informado um servico valido");
        }
        if (string.IsNullOrWhiteSpace(servicoNome))
        {
            throw new DomainException("Deve ser informado um servico valido");
            
        }
        
    }
    private static void ValidDuracao(int duracaoMin)
    {
        if( duracaoMin <= 0)
        {
            throw new DomainException("A duracao nao deve ser igual ou inferior a ZERO!");
        }
    }

    private static void ValidCliente(string nome, string telefone)
    {
        if (string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O Nome do Cliente deve ser informado!");
        }
        if (string.IsNullOrWhiteSpace(telefone))
        {
            throw new DomainException("O Telefone do Cliente deve ser informado!");
            
        }
    }

    
}