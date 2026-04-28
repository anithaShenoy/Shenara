using Microsoft.EntityFrameworkCore;
using Shenara.Api.Data;
using Shenara.Api.Filters;
using Shenara.Api.Services;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDbContext<ShenaraDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Shenara")));
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<PublicContentService>();
builder.Services.AddScoped<AdminTokenFilter>();
builder.Services.AddCors(options =>
{
    options.AddPolicy("ReactDev", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("ReactDev");
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShenaraDbContext>();
    if (app.Environment.IsDevelopment())
    {
        dbContext.Database.EnsureCreated();
    }
}

app.Run();
