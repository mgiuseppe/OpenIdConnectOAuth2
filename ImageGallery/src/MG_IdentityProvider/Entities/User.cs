using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MG_IdentityProvider.Entities
{
    [Table("Users")]
    public class User
    {
        [Key]
        [MaxLength(50)]
        public string SubjectId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Username { get; set; }

        [MaxLength(100)]
        public string Password { get; set; }

        [Required]
        public bool IsActive { get; set; }

        public ICollection<UserClaim> Claims { get; set; } = new List<UserClaim>();

        public ICollection<UserLogin> Logins { get; set; } = new List<UserLogin>();
    }
}
