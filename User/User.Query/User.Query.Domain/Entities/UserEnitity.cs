using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Query.Domain.Entities
{
    [Table("Users")]
    public class UserEntity
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }
        [Column("created")]
        public DateTime? Created { get; set; }
        [Column("updated")]
        public DateTime? Updated { get; set; }
        [Column("user_name")]
        [Required]
        public string UserName { get; set; }
        [Required]
        [Column("pass_hash")]
        public string Password { get; set; }
        [Required]
        [Column("first_name")]
        public string FirstName { get; set; }
        [Required]
        [Column("family_name")]
        public string FamilyName { get; set; }
        [Required]
        [Column("email")]
        public string Email { get; set; }
        [Required]
        [Column("email_verified")]
        public bool EmailVerified { get; set; }

    }
}