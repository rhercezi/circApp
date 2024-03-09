namespace Appointments.Domain.Entities
{
    public class Address
    {
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? Housenumber { get; set; }
        public string? PostCode { get; set; }
        public Decimal Longitude { get; set; }
        public Decimal Latitude { get; set; }
    }
}