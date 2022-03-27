using Microsoft.AspNetCore.Mvc;
using SubscriberAdmin.Models;
using SubscriberClient.Models;

namespace SubscriberClient.WebApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class LineController : ControllerBase
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly SubscriberContext _db;

        public LineController(IHttpClientFactory httpClientFactory,
            SubscriberContext db)
        {
            _httpClientFactory = httpClientFactory;
            _db = db;
        }

        [HttpPost("notify-callback")]
        public IActionResult NotifyCallback([FromForm] LineNotifyCallback notify)
        {
            Console.WriteLine(notify.Code);
            return Ok();
        }

        [HttpGet("login-callback")]
        public async Task<IActionResult> LoginCallback([FromQuery] LineLoginCallback callback)
        {
            var setting = new LineLoginSetting();

            if (callback.State != setting.State)
            {
                return new BadRequestResult();
            }

            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.PostAsync("https://api.line.me/oauth2/v2.1/token",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", callback.Code),
                    new KeyValuePair<string, string>("redirect_uri", setting.RedirectUri),
                    new KeyValuePair<string, string>("client_id", setting.ClientId),
                    new KeyValuePair<string, string>("client_secret", setting.Secret),
                }));
            var tokenResponse = await responseMessage.Content.ReadFromJsonAsync<LineLoginTokenResponse>();

            var responseMessage2 = await client.PostAsync("https://api.line.me/oauth2/v2.1/verify",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("id_token", tokenResponse.IdToken),
                    new KeyValuePair<string, string>("client_id", setting.ClientId),
                }));

            var verifyResponse = await responseMessage2.Content.ReadFromJsonAsync<LineLoginVerifyResponse>();
            await _db.Members.AddAsync(new Member
            {
                Name = verifyResponse.Name
            });
            await _db.SaveChangesAsync();


            return Ok();
        }
    }
}