using Canopy.API.Data;
using Canopy.API.Endpoints;
using Canopy.API.Filters;
using Canopy.API.Models.Events;
using Canopy.API.Services;
using Canopy.API.Validators.Events;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
var allowSpecificOrigins = "_allowSpecificOrigins";

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: allowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
            policy.WithOrigins("https://canopy-web-production.up.railway.app/")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

builder.Services.AddScoped<ClerkAuthService>();
builder.Services.AddScoped<ClerkAuthenticationFilter>();
builder.Services.AddScoped<IEventsRepository, EventsRepository>();
builder.Services.AddScoped<IEventsService, EventsService>();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddAuthentication(); 
builder.Services.AddAuthorization();
builder.Services.AddValidatorsFromAssemblyContaining<CreateEventRequestValidator>();
builder.Services.AddAutoMapper(typeof(EventsProfile));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if(string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string not configured.");
}

builder.Services.AddDbContext<CanopyDbContext>(options => 
    options.UseNpgsql(connectionString));

var app = builder.Build();
app.UseCors(allowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

var authenticatedApi = app.MapGroup("/api")
    .AddEndpointFilter<ClerkAuthenticationFilter>();

var eventsGroup = authenticatedApi.MapGroup("/events");

eventsGroup.MapEventsEndpoints();

app.Run();
