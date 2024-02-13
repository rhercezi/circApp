namespace Core.DTOs
{
    public class UserDto : BaseDto
    {
        public Guid Id { get; set; }
        public DateTime? Created { get; set; }
        public DateTime? Updated { get; set; }
        public string? UserName { get; set; }
        public string? FirstName { get; set; }
        public string? FamilyName { get; set; }
        public string? Email { get; set; }
        public bool EmailVerified { get; set; }
    }
}