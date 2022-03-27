using Microsoft.EntityFrameworkCore;

namespace SubscriberAdmin.Models
{
    public class SubscriberContext : DbContext
    {
        public SubscriberContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Subscriber> Subscribers { get; set; }
        public DbSet<Member> Members { get; set; }
    }
}