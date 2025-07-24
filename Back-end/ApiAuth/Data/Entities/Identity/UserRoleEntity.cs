using Microsoft.AspNetCore.Identity;
using WebWorker.Data.Entities.Identity;

namespace ApiAuth.Data.Entities.Identity
{
    public class UserRoleEntity : IdentityUserRole<long>
    {
        public virtual UserEntity? User { get; set; }
        public virtual RoleEntity? Role { get; set; }
    }
}
