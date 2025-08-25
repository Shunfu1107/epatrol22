namespace AdminPortalV8.Services
{
    //
    // Summary:
    //     Minimal interface for a user with id and username
    //
    // Type parameters:
    //   TKey:
    public interface IUser<out TKey>
    {
        //
        // Summary:
        //     Unique key for the user
        TKey Id { get; }
        //
        // Summary:
        //     Unique username
        string UserName { get; set; }
    }
}
