using ApiAuth.Data.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace WebWorker.Data.Entities.Identity;

public class RoleEntity : IdentityRole<long>
{
    public RoleEntity() : base() { }

    public RoleEntity(string roleName) : base(roleName) { }
    public virtual ICollection<UserRoleEntity>? UserRoles { get; set; }
}