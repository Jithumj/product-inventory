using Dapper;
using ProductInventoryAPI.Data;

namespace ProductInventoryAPI.Services.Logging
{
    public class LoggerService:ILoggerService
    {
        private readonly DapperContext _context;

        public LoggerService(DapperContext context)
        {
            _context = context;
        }

        public async Task LogAsync(string message)
        {
            using var conn = _context.CreateConnection();
            var sql = "INSERT INTO Logs (Id, CreatedAt, LogMessage) VALUES (@Id, @CreatedAt, @LogMessage)";
            await conn.ExecuteAsync(sql, new
            {
                Id = Guid.NewGuid(),
                CreatedAt = DateTimeOffset.UtcNow,
                LogMessage = message
            });
        }
    }
}
