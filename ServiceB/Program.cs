using ServiceB.Interfacees;
using ServiceB.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthorization();

builder.Services.AddCors();

builder.Services.AddRouting(); 

builder.Services.AddSingleton<IStorageStrategy, JsonStorageStrategy>();
builder.Services.AddScoped<IGraphOperationHandler, GraphOperationHandler>();
builder.Services.AddScoped<IWebSocketMessageHandler, WebSocketMessageHandler>();

var app = builder.Build();

app.UseRouting();

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

var webSocketOptions = new WebSocketOptions
{
    KeepAliveInterval = TimeSpan.FromMinutes(2)
};
app.UseWebSockets(webSocketOptions);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Map("/ws", async context =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var messageHandler = context.RequestServices.GetRequiredService<IWebSocketMessageHandler>();
        
        
        var buffer = new byte[1024 * 4];
        while (true)
        {
            var receiveResult = await webSocket.ReceiveAsync(
                new ArraySegment<byte>(buffer),
                CancellationToken.None);

            if (receiveResult.CloseStatus.HasValue)
            {
                await webSocket.CloseAsync(
                    receiveResult.CloseStatus.Value,
                    receiveResult.CloseStatusDescription,
                    CancellationToken.None);
                break;
            }

            var message = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
            await messageHandler.HandleMessageAsync(webSocket, message);
        }
    }
    else
    {
        context.Response.StatusCode = StatusCodes.Status400BadRequest;
    }
});
app.UseAuthorization();

// Test endpoint
app.MapGet("/", () => "WebSocket server is running");
app.Logger.LogInformation("ServiceB WebSocket server starting on ws://localhost:5002/ws");
app.Run();