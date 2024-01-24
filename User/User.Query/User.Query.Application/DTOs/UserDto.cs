using Core.DTOs;
using User.Query.Domain.Entities;

namespace User.Query.Application.DTOs
{
    public class UserDto : BaseDto
    {
        public UserDto(){}
        public UserDto(UserEntity UserEntity)
        {
            Id = UserEntity.Id;
            Created = UserEntity.Created;
            Updated = UserEntity.Updated;
            UserName = UserEntity.UserName;
            Password = UserEntity.Password;
            FirstName = UserEntity.FirstName;
            FamilyName = UserEntity.FamilyName;
            Email = UserEntity.Email;
            EmailVerified = UserEntity.EmailVerified;
        }

        public Guid Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string? Email { get; set; }
        public bool EmailVerified { get; set; }
    }
}