namespace Client.Services
{
    public interface IMessageBoxService
    {
        Task<ButtonResult> ShowAsync(string title, string message);
    }
}