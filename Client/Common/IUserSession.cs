using Models;

namespace Client.Common
{
    public interface IUserSession
    {
        User CurrentUser { get; set; }
    }
}