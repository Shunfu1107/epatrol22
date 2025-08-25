using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities
{
    public class User
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserName { get; set; }

        public string Password { get; set; }

        public string SecurityStamp { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public bool Active { get; set; }

        public bool? FirstPasswordReset { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }

        public bool? ReceivedEmail { get; set; }
    }
}