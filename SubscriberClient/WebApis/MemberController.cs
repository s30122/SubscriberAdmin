using Microsoft.AspNetCore.Mvc;
using SubscriberAdmin.Models;

namespace SubscriberClient.WebApis
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberController : ControllerBase
    {
        private readonly SubscriberContext _db;

        public MemberController(SubscriberContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult GetMembers()
        {
            return new JsonResult(_db.Members.ToList());
        }
    }
}