namespace Client.Services
{
    public interface ILoadingService
    {
        Task RunWithLoadingAsync(Func<Task> action);
    }
}
