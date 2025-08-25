using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Extensions
{
    public static class JsonInfoResultExtension
    {
        public static JsonInfoResult ThrowModelStateError(this JsonInfoResult result, ModelStateDictionary modelState)
        {
            var errors = modelState.Values.SelectMany(m => m.Errors.Select(e => e.ErrorMessage));
            var errormsg = String.Join(",", errors);
            result.Exception = new Exception(errormsg);
            result.Success = false;
            return result;
        }
        public static JsonInfoResult ThrowAsyncError(this JsonInfoResult result, IdentityResult asyncresult)
        {
            var errormsg = String.Join(",", asyncresult.Errors);
            result.Exception = new Exception(errormsg);
            result.Success = false;
            return result;
        }
    }
}
