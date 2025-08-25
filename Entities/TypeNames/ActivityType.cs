using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities
{
    public class ActivityType : _BaseTypeEntity
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }
    }
}
