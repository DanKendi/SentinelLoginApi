namespace SentinelApi.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using SentinelApi.Domain.Entities;
using SentinelApi.Domain.Interfaces;
using SentinelApi.Infrastructure.Persistence.Context;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly SentinelDbContext _context;

    public UsuarioRepository(SentinelDbContext context)
    {
        _context = context;
    }

    public async Task<Usuario?> GetByEmailAsync(string email)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
    }

    public async Task<Usuario?> GetByIdAsync(int id)
    {
        return await _context.Usuarios
            .Include(u => u.UsuarioRegioes)
            .ThenInclude(ur => ur.Regiao)
            .FirstOrDefaultAsync(u => u.IdUsuario == id);
    }

    public async Task<Usuario?> GetByUidFirebaseAsync(string uid)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.UidFirebase == uid);
    }

    public async Task AddAsync(Usuario usuario)
    {
        await _context.Usuarios.AddAsync(usuario);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Usuario usuario)
    {
        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();
    }
}