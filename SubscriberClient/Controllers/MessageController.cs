using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriberAdmin.Models;
using SubscriberClient.Models;

namespace SubscriberClient.Controllers;

public class MessageController : Controller
{
    private readonly SubscriberContext _db;
    private readonly IHttpClientFactory _clientFactory;

    public MessageController(SubscriberContext db,
        IHttpClientFactory clientFactory)
    {
        _db = db;
        _clientFactory = clientFactory;
    }

    public IActionResult Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Send([FromForm] LineNotifyMessage req)
    {
        var members = await _db.Members.Select(x => x.NotifyAccessToken).ToListAsync();
        var clients = members.Select(x =>
        {
            var client = _clientFactory.CreateClient();

            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {x}");
            return client;
        });
        await Task.WhenAll(clients.Select(x => x.PostAsync("https://notify-api.line.me/api/notify",
            new FormUrlEncodedContent(
                new[]
                {
                    new KeyValuePair<string, string>("message", req.Message)
                }))));
        return RedirectToAction("Index");
    }
}