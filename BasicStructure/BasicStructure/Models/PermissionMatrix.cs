namespace BasicStructure.Models
{
    public class PermissionMatrix
    {
        public int Id { get; set; }
        public int IdentityRoleId { get; set; }
        public IdentityRole<int> IdentityRole { get; set; }
        public int APIId { get; set; }
        public API API { get; set; }
    }
}
