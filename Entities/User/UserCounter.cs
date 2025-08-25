using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities
{
    public class UserCounter : _BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public int StaffCount { get; set; }
        public int VolunteerCount { get; set; }
        public int OrganizationCount { get; set; }
        public int ClientCount { get; set; }
        public int MemberCount { get; set; }

    }
}
