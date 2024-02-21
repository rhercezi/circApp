namespace Circles.Common.Entities
{
    public class Circle
    {
        public Guid CircleId { get; set; }
        public required string Name { get; set; }
        public required string Color { get; set; }
        public List<AppUser>? Users { get; set; }
    }
}