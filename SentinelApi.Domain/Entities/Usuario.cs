namespace SentinelApi.Domain.Entities;

public class Usuario
{
    public int IdUsuario { get; set; }
    public string Nome { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string SenhaHash { get; set; } = string.Empty;
    public string FcmToken { get; set; } = string.Empty;
    public string? UidFirebase { get; set; }
    public decimal? Latitude { get; set; }
    public decimal? Longitude { get; set; }
    public int RaioKm { get; set; }
    public DateTime DataCadastro { get; set; }
    public char Ativo { get; set; }

    // Navegação N:N
    public ICollection<UsuarioRegiao> UsuarioRegioes { get; set; } = new List<UsuarioRegiao>();
}