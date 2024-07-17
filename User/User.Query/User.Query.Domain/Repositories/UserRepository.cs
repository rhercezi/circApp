using Microsoft.EntityFrameworkCore;
using User.Common.Events;
using User.Query.Domain.DatabaseContext;
using User.Query.Domain.Entities;

namespace User.Query.Domain.Repositories
{
    public class UserRepository
    {
        private readonly IDbContextFactory<UserDbContext> _context;

        public UserRepository(IDbContextFactory<UserDbContext> context)
        {
            _context = context;
        }

        public async Task<UserEntity> GetUserByIdAsync(Guid id)
        {
            using var context = _context.CreateDbContext();
            return await context.Users.SingleAsync(u => u.Id == id);
        }

        public async Task<UserEntity> GetUserByEmailAsync(string email)
        {
            using var context = _context.CreateDbContext();
            return await context.Users.SingleAsync(u => u.Email == email);
        }

        public async Task<UserEntity> GetUserByUsernameAsync(string username)
        {
            using var context = _context.CreateDbContext();
            return await context.Users.SingleAsync(u => u.UserName == username);
        }

        public async Task CreateUser(UserCreatedEvent xEvent)
        {
            using var context = _context.CreateDbContext();
            var user = CreateUserFromEvent(xEvent);
            await context.Users.AddAsync(user);
            context.SaveChanges();
        }

        public async Task UpdateUser(UserEditedEvent xEvent)
        {
            using var context = _context.CreateDbContext();
            var userOriginal = await context.Users.SingleAsync(u => u.Id == xEvent.Id);
            UpdateValues(ref userOriginal, xEvent);
            context.SaveChanges();
        }

        public async Task DeleteUser(UserDeletedEvent xEvent)
        {
            using var context = _context.CreateDbContext();
            var user = await context.Users.SingleAsync(u => u.Id == xEvent.Id);
            context.Remove(user);
            context.SaveChanges();

        }

        public async Task VerifyEmail(EmailVerifiedEvent xEvent)
        {
            using var context = _context.CreateDbContext();
            var user = await context.Users.SingleAsync(u => u.Id == xEvent.Id);
            user.EmailVerified = true;
            context.SaveChanges();
        }

        public async Task UpdateUsersPassword(PasswordUpdatedEvent xEvent)
        {
            using var context = _context.CreateDbContext();
            var userOriginal = await context.Users.SingleAsync(u => u.Id == xEvent.Id);
            userOriginal.Password = xEvent.Password;
            context.SaveChanges();
        }

        private UserEntity CreateUserFromEvent(UserCreatedEvent xEvent)
        {
                    
            return new UserEntity {
                Id = xEvent.Id,
                Created = xEvent.Created,
                UserName = xEvent.UserName,
                Password = xEvent.Password,
                FirstName = xEvent.FirstName,
                FamilyName = xEvent.FamilyName,
                Email = xEvent.Email,
                EmailVerified = false

            };
        }

        private void UpdateValues(ref UserEntity userOriginal, UserEditedEvent xEvent)
        {
            userOriginal.Updated = xEvent.Updated;
            if (!string.IsNullOrEmpty(xEvent.UserName))
            {
                userOriginal.UserName = xEvent.UserName;
            }
            if (!string.IsNullOrEmpty(xEvent.FirstName))
            {
                userOriginal.FirstName = xEvent.FirstName;
            }
            if (!string.IsNullOrEmpty(xEvent.FamilyName))
            {
                userOriginal.FamilyName = xEvent.FamilyName;
            }
            if (!string.IsNullOrEmpty(xEvent.Email))
            {
                userOriginal.Email = xEvent.Email;
            }
        }
    }
}