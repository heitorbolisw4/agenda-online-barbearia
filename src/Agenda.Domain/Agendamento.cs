using System.Text.RegularExpressions;
using Agenda.Domain.Enum;
using Agenda.Domain.Exceptions;

namespace Agenda.Domain;

public partial class Agendamento
{
    private Agendamento(Guid profissionalId, Guid servicoId, string servicoNome, int duracaoMin, string clienteNome, string clienteTelefone, DateTimeOffset inicio, DateTimeOffset fim, Origem origem)
    {

        ProfissionalId = profissionalId;
        ServicoId = servicoId;
        ServicoNome = servicoNome;
        DuracaoMin = duracaoMin;
        ClienteNome = clienteNome;
        ClienteTelefone = clienteTelefone;
        Inicio = inicio;
        Fim = fim;
        Origem = origem;
    }

    public Guid Id { get; private set; } = Guid.CreateVersion7();
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

    

    public static Agendamento Criar(Guid profissionalId, Guid servicoId, string servicoNome, int duracaoMin, string clienteNome, string clienteTelefone, DateTimeOffset inicio, Origem origem )
    {
        ValidProfissionalId(profissionalId);
        ValidServico(servicoId, servicoNome);
        ValidDuracao(duracaoMin);
        ValidNome(clienteNome);
        var telefoneNormalizado = NormalizarTelefone(clienteTelefone);
        var fim = inicio.AddMinutes(duracaoMin);
        return new Agendamento(profissionalId, servicoId, servicoNome ,duracaoMin, clienteNome, telefoneNormalizado, inicio, fim, origem);
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

    private static void ValidNome(string nome)
    {
        if(string.IsNullOrWhiteSpace(nome))
        {
            throw new DomainException("O Nome do Cliente deve ser informado!");
        }
    }

    private static string NormalizarTelefone(string telefone)
    {
        
        if (string.IsNullOrWhiteSpace(telefone))
        {
            throw new DomainException("O Telefone do Cliente deve ser informado!");
            
        }
        // Tira só o enfeite (espaço, parênteses, hífen, ponto, +). Letra continua lá.
        var digits = Formatacao().Replace(telefone, "");
        if(!digits.All(char.IsAsciiDigit))
        {
            throw new DomainException("O Telefone deve conter apenas numeros");
        }
        if(digits.Length is 12 or 13 && digits.StartsWith("55"))
        {
            digits = digits[2..];
        }
        if(digits.Length is not (10 or 11))
        {
            throw new DomainException("O Telefone deve ter DDD + numero(10 ou 11 digitos)");
        }
        return digits;
    }

    [GeneratedRegex(@"[\s().+-]")]
    private static partial Regex Formatacao();
}