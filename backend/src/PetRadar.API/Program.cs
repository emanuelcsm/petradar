using PetRadar.API.Infrastructure;
using PetRadar.API.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Infrastructure — shared
builder.Services.AddApiOptions(builder.Configuration);
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddDomainEvents();
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddRealtime();
builder.Services.AddFrontendCors(builder.Configuration);

// Modules
builder.Services.AddApplicationModules(builder.Configuration);

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseGlobalExceptionMiddleware();
app.UseHttpsRedirection();
app.UseLocalMediaStaticFiles();
app.UseFrontendCors();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapRealtimeEndpoints();

app.Run();
