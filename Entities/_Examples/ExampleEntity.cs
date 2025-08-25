using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities.Examples
{
    //The table and column to be created in database, must specify the class in AppDbContext (DbSet) or else it won't be generated.
    public class ExampleEntity : _BaseEntity
    {
        public string Param1 { get; set; }
        public string Param2 { get; set; }
    }
}
