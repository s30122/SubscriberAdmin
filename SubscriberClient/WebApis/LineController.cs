using Microsoft.AspNetCore.Mvc;

namespace SubscriberClient.WebApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineController : ControllerBase
    {
        [HttpPost("my-callback")]
        public IActionResult NotifyCallback([FromForm] LineNotifyCallback notify)
        {
            Console.WriteLine(notify.Code);
            return Ok();
        }
    }

    public class LineNotifyCallback
    {
        public string Code { get; set; }
        public string State { get; set; }
    }
}