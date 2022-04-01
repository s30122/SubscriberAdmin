using Microsoft.EntityFrameworkCore;
using SubscriberAdmin.Models;
using SubscriberClient.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllers();
builder.Services.AddHttpClient();
builder.Services.AddDbContext<SubscriberContext>(options => options.UseInMemoryDatabase("subs"));
builder.Services.Configure<LineLoginSetting>( builder.Configuration.GetSection("LineLoginSetting"));
builder.Services.Configure<LineNotifySetting>( builder.Configuration.GetSection("LineNotifySetting"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();


