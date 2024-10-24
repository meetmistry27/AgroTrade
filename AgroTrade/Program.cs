using Microsoft.EntityFrameworkCore;
using AgroTrade.Models;
using AgroTrade.Services;
using System.Net.WebSockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
    new MySqlServerVersion(new Version(8, 0, 30))));

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CropService>();
builder.Services.AddScoped<TransactionService>();

var app = builder.Build();
app.UseWebSockets();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

var auctionClients = new List<WebSocket>();

//app.Map("/auction/ws", async context =>
//{
//    if (context.WebSockets.IsWebSocketRequest)
//    {
//        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
//        auctionClients.Add(webSocket);
//        var buffer = new byte[1024];

//        try
//        {
//            while (true)
//            {
//                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

//                if (result.MessageType == WebSocketMessageType.Close)
//                {
//                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
//                    auctionClients.Remove(webSocket);
//                    break;
//                }

//                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

//                foreach (var client in auctionClients)
//                {
//                    if (client.State == WebSocketState.Open)
//                    {
//                        await client.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
//                    }
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Auction WebSocket error: {ex.Message}");
//            auctionClients.Remove(webSocket);
//        }
//    }
//    else
//    {
//        context.Response.StatusCode = 400;
//    }
//});



var clients = new List<WebSocket>();

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        WebSocket webSocket = await context.WebSockets.AcceptWebSocketAsync();
        clients.Add(webSocket);
        var buffer = new byte[1024];

        try
        {
            while (true)
            {
                var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

                if (result.MessageType == WebSocketMessageType.Close)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                    clients.Remove(webSocket); 
                    break;
                }

                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                //Console.WriteLine("Received: " + message);

                foreach (var client in clients)
                {
                    if (client.State == WebSocketState.Open)
                    {
                        await client.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), result.MessageType, result.EndOfMessage, CancellationToken.None);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"WebSocket error: {ex.Message}");
            clients.Remove(webSocket);
        }
    }
    else
    {
        context.Response.StatusCode = 400;
    }
});


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "websocketDemo",
    pattern: "WebSocketDemo",
    defaults: new { controller = "Home", action = "WebSocketDemo" });

app.Run();
