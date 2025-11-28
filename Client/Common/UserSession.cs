using Models;

namespace Client.Common
{
    public class UserSession : IUserSession
    {
        public required User CurrentUser { get; set; }
    }
}