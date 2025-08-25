using AdminPortalV8.Libraries.ExtendedUserIdentity.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Models.ViewModels
{
    public class ManageRoleViewModel : IValidatableObject
    {
        [Required]
        public long RolePvid { get; set; }

        [Required]
        [MaxLength(80)]
        [Display(Name = "Role Name")]
        public string Name { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true, NullDisplayText = "dd/MM/yyyy", ConvertEmptyStringToNull = true)]
        public DateTime? StartActiveDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}", ApplyFormatInEditMode = true, NullDisplayText = "dd/MM/yyyy", ConvertEmptyStringToNull = true)]
        public DateTime? EndActiveDate { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> res = new List<ValidationResult>();
            if (!EndActiveDate.HasValue || !StartActiveDate.HasValue) return res;
            if (EndActiveDate.Value < StartActiveDate.Value)
            {
                ValidationResult mss = new ValidationResult("End date must be greater than Start Date");
                res.Add(mss);

            }
            return res;
        }

        public static implicit operator AppRole(ManageRoleViewModel model)
        {
            return new AppRole
            {
                Pvid = model.RolePvid,
                IsActive = model.IsActive,
                Name = model.Name,
                StartActiveDate = model.StartActiveDate.HasValue ? model.StartActiveDate.Value : DateTime.Now,
                EndActiveDate = model.EndActiveDate.HasValue ? model.EndActiveDate.Value : DateTime.MaxValue
            };
        }
    }
}