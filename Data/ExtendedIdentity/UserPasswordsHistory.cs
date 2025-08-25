using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPortalV8.Data.ExtendedIdentity
{
    [Table(name:"UserPasswordHistory")]
    public class UserPasswordsHistory
    {
        
        public int UserID { get; set; }
        public string? PasswordHash1 { get; set; }
        public DateTime? PasswordDate1 { get; set; }
        public string? PasswordHash2 { get; set; }
        public DateTime? PasswordDate2 { get; set; }
        public string? PasswordHash3 { get; set; }
        public DateTime? PasswordDate3 { get; set; }
        public string? PasswordHash4 { get; set; }
        public DateTime? PasswordDate4 { get; set; }
        public string? PasswordHash5 { get; set; }
        public DateTime? PasswordDate5 { get; set; }
        public string? PasswordHash6 { get; set; }
        public DateTime? PasswordDate6 { get; set; }
        public string? PasswordHash7 { get; set; }
        public DateTime? PasswordDate7 { get; set; }
        public int? CID { get; set; }
        public string? referID { get; set; }
    }
}
