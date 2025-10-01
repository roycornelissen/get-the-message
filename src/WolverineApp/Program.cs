using JasperFx;
using Wolverine;
using WolverineApp.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging(logging =>
{
    logging
        .AddConsole()
        .SetMinimumLevel(LogLevel.Trace);
});

builder.Host.UseWolverine();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/send", async (IMessageBus bus) =>
    {
        await bus.PublishAsync(new OrderPlaced(Guid.NewGuid(), Guid.NewGuid(), 100));
    })
.WithName("Send");

return await app.RunJasperFxCommands(args);
