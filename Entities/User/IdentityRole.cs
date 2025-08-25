using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities
{
    public class IdentityRole
    {   
        [Key]
        public long Pvid { get; set; }
        public string Name { get; set; }
        public DateTime StartActiveDate { get; set; }
        public DateTime EndActiveDate { get; set; }
        public bool IsActive { get; set; }

        public int? CID { get; set; }

        public string? referID { get; set; }
    }
}