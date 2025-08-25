using System.ComponentModel.DataAnnotations;

namespace AdminPortalV8.Helpers
{
    public class AppValidator
    {

        public static void Validate(object model)
        {
            ValidationContext context = new ValidationContext(model, null, null);
            List<ValidationResult> results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(model, context, results, true))
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                foreach (ValidationResult vr in results)
                {
                    dic.Add(vr.MemberNames.First(), vr.ErrorMessage);
                }

                throw new AppException(dic);
            }
        }
    }
}
