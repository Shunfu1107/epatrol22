using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities
{
    //The table and column to be created in database, must specify the class in AppDbContext (DbSet) or else it won't be generated.
    public class InterestType : _BaseTypeEntity
    {
        public int? CID { get; set; }
        public string? referID { get; set; }
    }

}
