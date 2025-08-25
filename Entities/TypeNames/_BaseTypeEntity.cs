using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities
{
    public class _BaseTypeEntity : _BaseEntity
    {
        [Required]
        public string Name { get; set; }
    }
}
