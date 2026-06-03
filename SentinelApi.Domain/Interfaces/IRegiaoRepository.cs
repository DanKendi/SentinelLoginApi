namespace SentinelApi.Domain.Interfaces;

using SentinelApi.Domain.Entities;

public interface IRegiaoRepository
{
    Task<IEnumerable<Regiao>> GetAllAsync();
    Task<Regiao?> GetByIdAsync(int id);
}