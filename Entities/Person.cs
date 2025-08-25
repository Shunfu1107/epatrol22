using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace AdminPortalV8.Entities
{
    //The table and column to be created in database, must specify the class in AppDbContext (DbSet) or else it won't be generated.
    public class Person : _BaseEntity
    {
        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; }
        public string? Name { get; set; }
        public string? NRIC { get; set; }
        public DateTime DOB { get; set; }
        public string? Gender { get; set; }
        public string? MaritalStatus { get; set; }
      
        public string? Nationality { get; set; }
        public string? Race { get; set; }
        public string? Address { get; set; }
        public string? Residential { get; set; } //for client and MET volunteer this is refer Unit No.
        public string? Postal { get; set; }
        public string? Mobile { get; set; }
        public string? Occupation { get; set; }
        public string? LanguagePreference { get; set; }
        public string? PhotoPath { get; set; }        
        public string? ShortName { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }
    }
}
