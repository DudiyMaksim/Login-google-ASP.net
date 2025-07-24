using Microsoft.AspNetCore.Identity;
using System.Data;

namespace ApiAuth.Data.Entities.Identity
{
    public class UserEntity : IdentityUser<long>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Image { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public virtual ICollection<UserRoleEntity>? UserRoles { get; set; }
    }
}
