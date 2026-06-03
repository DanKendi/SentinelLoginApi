namespace SentinelApi.Domain.Entities;

public class UsuarioRegiao
{
    public int IdUsuario { get; set; }
    public int IdRegiao { get; set; }
    public DateTime DataInscricao { get; set; }
    public char Ativo { get; set; }

    // Navegação
    public Usuario Usuario { get; set; } = null!;
    public Regiao Regiao { get; set; } = null!;
}