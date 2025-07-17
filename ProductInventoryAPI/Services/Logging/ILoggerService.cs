namespace ProductInventoryAPI.Services.Logging
{
    public interface ILoggerService
    {
        Task LogAsync(string message);
    }
}
