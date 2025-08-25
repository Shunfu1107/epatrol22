using System.ComponentModel.DataAnnotations;

namespace AdminPortalV7.Entities
{
    public class IdentityRole
    {
        [Key]
        public long Pvid { get; set; }
        public string Name { get; set; }
        public DateTime StartActiveDate { get; set; }
        public DateTime EndActiveDate { get; set; }
        public bool IsActive { get; set; }
    }
}
