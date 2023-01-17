using Microsoft.AspNetCore.Mvc;
using MySqlConnector;
using System.Net;

namespace RegisteredClient_API.Controllers
{
    
    [ApiController]
    public class MessagesCheckController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MessagesCheckController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet()]
        [Route("api/messagecheck")]
        public async Task<ActionResult> getDB_MessageCheck()
        {
            Request.Headers.TryGetValue(_configuration.GetValue<string>("Const_DeviceId"), out var extractedDeviceId);

            using var SQL_Conn = new MySqlConnection(_configuration.GetConnectionString("WebApiDatabase"));
            await SQL_Conn.OpenAsync();
            using var SQL_CMD = new MySqlCommand("SELECT count(*) as cnt FROM messages WHERE device_id=@device_id;", SQL_Conn);
            SQL_CMD.Parameters.AddWithValue("@device_id", extractedDeviceId.ToString());
            using var SQL_drA = await SQL_CMD.ExecuteReaderAsync();
            while (await SQL_drA.ReadAsync())
            {
                int cnt = Convert.ToInt16(SQL_drA["cnt"]);
                if (cnt > 0)
                {
                    return Ok(new DBGeneralResult { result = true, StatusCode = HttpStatusCode.OK, message_count = cnt });
                }
            }

            //return new HttpResponseMessage(HttpStatusCode.OK, "No result");
            return NoContent();
        }
    }

    public class DBGeneralResult
    {
        public bool result { get; set; }
        public int message_count { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
