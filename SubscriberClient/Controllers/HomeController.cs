using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SubscriberAdmin.Models;
using SubscriberClient.Models;

namespace SubscriberClient.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly SubscriberContext _db;

    public HomeController(ILogger<HomeController> logger,
        SubscriberContext db)
    {
        _logger = logger;
        _db = db;
    }

    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public async Task<IActionResult> MemberSubscribe(int memberId)
    {
        var member = await _db.Members.FirstOrDefaultAsync(x => x.Id == memberId);
        if (member is null)
        {
            RedirectToAction("Index", memberId);
        }

        return View(member);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}