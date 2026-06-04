namespace SentinelApi.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SentinelApi.Domain.Entities;
using SentinelApi.Domain.Interfaces;
using SentinelApi.Infrastructure.Persistence.Context;

public class RegiaoRepository : IRegiaoRepository
{
    private readonly SentinelDbContext _context;

    public RegiaoRepository(SentinelDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Regiao>> GetAllAsync()
    {
        return await _context.Regioes.ToListAsync();
    }

    public async Task<Regiao?> GetByIdAsync(int id)
    {
        return await _context.Regioes
            .Include(r => r.UsuarioRegioes)
            .FirstOrDefaultAsync(r => r.IdRegiao == id);
    }
}