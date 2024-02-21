namespace Circles.Common.Entities
{
    public class AppUser
    {
        public Guid UserId { get; set; }
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string UserName { get; set; }
    }
}