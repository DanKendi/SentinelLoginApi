namespace SentinelApi.Domain.Interfaces;

using SentinelApi.Domain.Entities;

public interface IUsuarioRepository
{
    Task<Usuario?> GetByEmailAsync(string email);
    Task<Usuario?> GetByIdAsync(int id);
    Task<Usuario?> GetByUidFirebaseAsync(string uid);
    Task AddAsync(Usuario usuario);
    Task UpdateAsync(Usuario usuario);
}