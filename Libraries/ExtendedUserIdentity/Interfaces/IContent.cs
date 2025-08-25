namespace AdminPortalV8.Libraries.ExtendedUserIdentity.Interfaces
{
    public interface IContent
    {
        /// <summary>
        /// Unique Permission Key
        /// </summary>
        string Key { set; get; }

        /// <summary>
        /// Information about the behavior of the action
        /// </summary>
        string Desc { get; set; }

        /// <summary>
        /// Title of the Action
        /// </summary>
        string Title { get; set; }

        /// <summary>
        /// <para>The Id of another action where's this action belong to</para>
        /// </summary>
        string AssociatedKey { get; set; }

        /// <summary>
        /// <para>Mark the action at witch they created, Format yyyy/MM/dd</para>
        /// <para>This help the developer when decide to change implementation</para>
        /// <para>Reduce accidently implementation change</para>
        /// </summary>
        string DateCreated { get; set; }

        bool StaticAuthorization { get; set; }
    }
}