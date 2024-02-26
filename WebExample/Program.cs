using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Xml.Linq;
using WebExample;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// RINGLEADER EXAMPLE

builder.Services.AddSingleton<TerminalCookieHandler>();

builder.Services
    .AddHttpClient<ExampleTypedClient>()
    .UseContextualCookies() // Enable cookie context support for HttpRequestMessage
    .AddHttpMessageHandler<TerminalCookieHandler>(); // Terminal demo handler

// Override behavior for handler generation when criteria are met
builder.Services.AddContextualHttpClientFactory((client, context) =>
{
    if (client == typeof(ExampleTypedClient).Name)
    {
        var handler = new SocketsHttpHandler();
        if (context == "cert1")
        {
            handler.SslOptions = new System.Net.Security.SslClientAuthenticationOptions()
            {
                ClientCertificates = new X509Certificate2Collection()
            };
        }
        return handler;
    }

    return null;
});

// ------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
