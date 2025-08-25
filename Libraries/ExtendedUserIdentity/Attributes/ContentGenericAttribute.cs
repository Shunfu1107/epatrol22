using AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces;
using AdminPortalV8.Libraries.ExtendedUserIdentity.Models;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Globalization;

namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class ContentGenericAttribute : ActionFilterAttribute, IContent
    {
        /// <summary>
        /// Unique Permission Key
        /// </summary>
        public string Key { set; get; }

        /// <summary>
        /// Information about the behavior of the action
        /// </summary>
        public string Desc { get; set; }

        /// <summary>
        /// Title of the Action
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// <para>The Id of another action where's this action belong to</para>
        /// </summary>
        public string AssociatedKey { get; set; }

        /// <summary>
        /// Gets or Sets the created date of the content
        /// </summary>
        public DateTime DateCreatedInternal { get; set; }

        /// <summary>
        /// <para>Mark the action at witch they created, Format yyyy/MM/dd</para>
        /// <para>This help the developer when decide to change implementation</para>
        /// <para>Reduce accidently implementation change</para>
        /// </summary>
        public string DateCreated
        {
            get
            {
                return DateCreatedInternal.ToString("yyyy/MM/dd", CultureInfo.CurrentCulture);
            }
            set
            {
                DateCreatedInternal = DateTime.Parse(value, CultureInfo.CurrentCulture, DateTimeStyles.None);
            }
        }

        /// <summary>
        /// Validate authorization by only the Permission without validate the Location
        /// </summary>
        public bool StaticAuthorization { get; set; }

        protected ContentGenericAttribute() { }

        public IContent ConvertToContentPermission()
        {
            return new ContentPermission
            {
                AssociatedKey = AssociatedKey,
                Desc = Desc,
                Title = Title,
                Key = Key,
                DateCreated = DateCreated,
                StaticAuthorization = StaticAuthorization
            };
        }
    }
}
