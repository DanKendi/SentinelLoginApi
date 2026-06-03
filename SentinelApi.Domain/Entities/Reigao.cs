namespace SentinelApi.Domain.Entities;

public class Regiao
{
    public int IdRegiao { get; set; }
    public string NmRegiao { get; set; } = string.Empty;
    public string NmEstado { get; set; } = string.Empty;
    public string NmPais { get; set; } = string.Empty;
    public decimal ReLatitude { get; set; }
    public decimal ReLongitude { get; set; }

    // Navegação N:N
    public ICollection<UsuarioRegiao> UsuarioRegioes { get; set; } = new List<UsuarioRegiao>();
}