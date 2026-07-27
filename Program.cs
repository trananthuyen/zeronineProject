using zeronineProject.Core.Entities;
using zeronineProject.Infrastructure.ExternalAPICalls.BinanceAPIs;
using zeronineProject.Infrastructure.ExternalAPICalls.TelegramAPIs;
using zeronineProject.UI.HostedServices;
using zeronineProject.UI.Hubs;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddHostedService<
    BinancePriceBackgroundService
>();

builder.Services.Configure<TelegramOptions>(
    builder.Configuration.GetSection("Telegram"));

builder.Services.AddHttpClient<SendMessage>();

builder.Services.AddScoped<GetTradingPair>();

var app = builder.Build();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapHub<CryptoHub>("/cryptoHub");

app.Run();
