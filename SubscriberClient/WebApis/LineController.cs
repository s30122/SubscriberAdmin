using System.Collections;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using Microsoft.Extensions.Options;
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
        private readonly LineLoginSetting _loginSetting;
        private readonly LineNotifySetting _lineNotifySetting;

        public LineController(IHttpClientFactory httpClientFactory,
            SubscriberContext db,
            IOptions<LineLoginSetting> options,
            IOptions<LineNotifySetting> notifyOptions)
        {
            _httpClientFactory = httpClientFactory;
            _db = db;
            _loginSetting = options.Value;
            _lineNotifySetting = notifyOptions.Value;
        }

        [HttpPost("notify-callback")]
        public async Task<IActionResult> NotifyCallback([FromForm] LineNotifyCallback notify)
        {
            var member = await _db.Members.SingleOrDefaultAsync(x => x.RandomState == notify.State);
            if (member is null)
            {
                return new BadRequestResult();
            }

            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.PostAsync("https://notify-bot.line.me/oauth/token",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", notify.Code),
                    new KeyValuePair<string, string>("redirect_uri", _lineNotifySetting.RedirectUri),
                    new KeyValuePair<string, string>("client_id", _lineNotifySetting.ClientId),
                    new KeyValuePair<string, string>("client_secret", _lineNotifySetting.Secret),
                }));
            var token = await responseMessage.Content.ReadFromJsonAsync<LineNotifyTokenResponse>();

            member.NotifyAccessToken = token.AccessToken;
            await _db.SaveChangesAsync();


            return Redirect($"/home/MemberSubscribe?memberId={member.Id}");
        }

        [HttpPost]
        public async Task<IActionResult> Unsubscribe([FromForm] string notifyAccessToken, [FromForm] int id)
        {
            var member =
                await _db.Members.SingleOrDefaultAsync(x => x.Id == id && x.NotifyAccessToken == notifyAccessToken);
            if (member is null)
            {
                return new BadRequestResult();
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {notifyAccessToken}");
            var responseMessage = await client.PostAsync("https://notify-api.line.me/api/revoke",
                new StringContent("", Encoding.Default, "application/x-www-form-urlencoded"));
            var revokeResponse = await responseMessage.Content.ReadFromJsonAsync<LineRevokeResponse>();
            if (revokeResponse.Status == HttpStatusCode.OK && revokeResponse.Message == "ok")
            {
                member.NotifyAccessToken = string.Empty;
                await _db.SaveChangesAsync();
                return Redirect($"/home/index?memberId={member.Id}");
            }

            return Ok("取消訂閱失敗");
        }

        [HttpGet("login-callback")]
        public async Task<IActionResult> LoginCallback([FromQuery] LineLoginCallback callback)
        {
            if (callback.State != _loginSetting.State)
            {
                return new BadRequestResult();
            }

            var client = _httpClientFactory.CreateClient();
            var responseMessage = await client.PostAsync("https://api.line.me/oauth2/v2.1/token",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", callback.Code),
                    new KeyValuePair<string, string>("redirect_uri", _loginSetting.RedirectUri),
                    new KeyValuePair<string, string>("client_id", _loginSetting.ClientId),
                    new KeyValuePair<string, string>("client_secret", _loginSetting.Secret),
                }));
            var tokenResponse = await responseMessage.Content.ReadFromJsonAsync<LineLoginTokenResponse>();

            var responseMessage2 = await client.PostAsync("https://api.line.me/oauth2/v2.1/verify",
                new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("id_token", tokenResponse.IdToken),
                    new KeyValuePair<string, string>("client_id", _loginSetting.ClientId),
                }));

            var verifyResponse = await responseMessage2.Content.ReadFromJsonAsync<LineLoginVerifyResponse>();

            var member = new Member
            {
                Name = verifyResponse.Name,
                RandomState = Guid.NewGuid().ToString("N").ToUpper()
            };
            await _db.Members.AddAsync(member);
            await _db.SaveChangesAsync();

            return Redirect($"/home/MemberSubscribe?memberId={member.Id}");
        }
    }
}