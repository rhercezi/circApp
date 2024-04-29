using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace User.Query.Domain.Entities
{
    [Table("RefreshTokens")]
    public class RefreshTokenEntity
    {
        [Key]
        [Column("id")]
        public string Id { get; set; }
        [Column("user_id")]
        public string UserId { get; set; }
        [Column("iat")]
        public long Iat { get; set; }
        [Column("nbf")]
        public long Nbf { get; set; }
        [Column("exp")]
        public long Exp { get; set; }
    }
}