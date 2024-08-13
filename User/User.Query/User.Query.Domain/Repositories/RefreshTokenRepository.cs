using Microsoft.EntityFrameworkCore;
using User.Query.Domain.DatabaseContext;
using User.Query.Domain.Entities;

namespace User.Query.Domain.Repositories
{
    public class RefreshTokenRepository
    {
        private readonly IDbContextFactory<UserDbContext> _context;

        public RefreshTokenRepository(IDbContextFactory<UserDbContext> context)
        {
            _context = context;
        }

        public async Task AddRefreshToken(RefreshTokenEntity refreshToken)
        {
            using var context = _context.CreateDbContext();
            await context.RefreshTokens.AddAsync(refreshToken);
            context.SaveChanges();
        }

        public async Task<RefreshTokenEntity> GetRefreshTokenById(string id)
        {
            using var context = _context.CreateDbContext();
            return await context.RefreshTokens.SingleAsync(rt => rt.Id == id);
        }

        public async Task<RefreshTokenEntity> GetRefreshTokenByUserId(string userId)
        {
            using var context = _context.CreateDbContext();
            return await context.RefreshTokens.SingleAsync(rt => rt.UserId == userId);
        }

        public async Task DeleteRefreshToken(RefreshTokenEntity refreshToken)
        {
            using var context = _context.CreateDbContext();
            context.Remove(refreshToken);
            await context.SaveChangesAsync();
        }

        public async Task DeleteRefreshTokenByUserId(string userId)
        {
            using var context = _context.CreateDbContext();
            var refreshTokens = await context.RefreshTokens.Where(rt => rt.UserId == userId).ToListAsync();
            context.RemoveRange(refreshTokens);
            await context.SaveChangesAsync();
        }
    }
}