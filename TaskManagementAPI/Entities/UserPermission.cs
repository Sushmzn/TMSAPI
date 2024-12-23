using System.ComponentModel.DataAnnotations.Schema;
using System.Security;

namespace TaskManagementAPI.Entities
{
    public class UserPermission
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        // Define the foreign key relationship for UserId
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        public int PermissionId { get; set; }

        // Define the foreign key relationship for PermissionId
        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
    }
}
