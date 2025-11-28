using GateMonitor.Controllers.Hubs;
using GateMonitor.Data;
using GateMonitor.Models.Settings;
using GateMonitor.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.WebSockets;
using System.Text;
using GateMonitor.Controllers.WebSockets;
using WebSocketManager = GateMonitor.Controllers.WebSockets.WebSocketManager;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:5132");

builder.Services.AddAuthentication("MyCookieAuth")
    .AddCookie("MyCookieAuth", options =>
    {
        options.LoginPath = "/Login";
        options.ExpireTimeSpan = TimeSpan.FromHours(12);
    });

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Main")));

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings"));

builder.Services.AddScoped<UserService>();

builder.Services.AddSingleton<WebSocketManager>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("http://localhost:5132");
    });
});

var app = builder.Build();

using var scope = app.Services.CreateScope();

var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

db.Database.EnsureCreated();

await AppDbSeeder.Seed(db);

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllers();

app.MapHub<RfidScanHub>("/rfidHub");

app.UseWebSockets();

// WebSocket endpoint
app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        var socket = await context.WebSockets.AcceptWebSocketAsync();

        var wsManager = context.RequestServices.GetRequiredService<WebSocketManager>();
        string clientId = Guid.NewGuid().ToString();
        wsManager.AddSocket(clientId, socket);

        // Keep connection open and handle messages
        await HandleClient(socket);

        // Remove socket on disconnect
        wsManager.RemoveSocket(clientId);
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});

app.Run();

static async Task HandleClient(WebSocket socket)
{
    var buffer = new byte[1024];

    while (socket.State == WebSocketState.Open)
    {
        var result = await socket.ReceiveAsync(buffer, CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Close)
        {
            await socket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
        }
        else
        {
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine("Received: " + message);

            // Example: echo back
            var outBuffer = Encoding.UTF8.GetBytes("Server received: " + message);
            await socket.SendAsync(outBuffer, WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}