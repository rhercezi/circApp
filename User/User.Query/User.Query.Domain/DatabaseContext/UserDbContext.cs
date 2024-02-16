using Microsoft.EntityFrameworkCore;
using User.Query.Domain.Entities;

namespace User.Query.Domain.DatabaseContext
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options) : base(options)
        {
        }

        public DbSet<UserEntity> Users { get; set; }
    }
}