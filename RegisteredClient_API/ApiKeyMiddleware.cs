using Microsoft.Extensions.Configuration;
using MySqlConnector;
using System.Configuration;

namespace RegisteredClient_API
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;

        public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
        {
            _next = next;
            _configuration = configuration;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (!context.Request.Headers.TryGetValue(_configuration.GetValue<string>("Const_DeviceID"), out var extractedDeviceId))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Device id was not provided");
                return;
            }

            if (!context.Request.Headers.TryGetValue(_configuration.GetValue<string>("Const_DeviceKey"), out var extractedDeviceKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Device key was not provided");
                return;
            }

            using var SQL_Conn = new MySqlConnection(_configuration.GetConnectionString("WebApiDatabase"));
            await SQL_Conn.OpenAsync();
            using var SQL_CMD = new MySqlCommand("SELECT * FROM devices WHERE device_id=@device_id AND device_key=@device_key;", SQL_Conn);
            SQL_CMD.Parameters.AddWithValue("@device_id", extractedDeviceId.ToString());
            SQL_CMD.Parameters.AddWithValue("@device_key", extractedDeviceKey.ToString());
            using var SQL_drA = await SQL_CMD.ExecuteReaderAsync();
            if(!SQL_drA.HasRows)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client");
                return;
            }

            await _next(context);
        }
    }
}
