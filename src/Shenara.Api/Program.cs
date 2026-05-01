using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Shenara.Api.Data;
using Shenara.Api.Filters;
using Shenara.Api.Services;
using System.Text.Json.Serialization;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);
var shenaraConnectionString =
    builder.Configuration.GetConnectionString("Shenara");

if (string.IsNullOrWhiteSpace(shenaraConnectionString))
{
    shenaraConnectionString = "Server=localhost;Database=Shenara;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;TrustServerCertificate=True";
}

builder.Services.AddOpenApi();
builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddDbContext<ShenaraDbContext>(options =>
    options.UseSqlServer(shenaraConnectionString)
        .ConfigureWarnings(warnings => warnings.Ignore(RelationalEventId.PendingModelChangesWarning)));
builder.Services.AddSingleton<AdminAuthService>();
builder.Services.AddSingleton<InquiryProtectionService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<PublicContentService>();
builder.Services.AddScoped<AdminTokenFilter>();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddPolicy("public-inquiries", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 3,
                Window = TimeSpan.FromDays(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.AddPolicy("admin-login", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromMinutes(15),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));
});
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

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.Use(async (context, next) =>
{
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; img-src 'self' https: data:; style-src 'self' 'unsafe-inline'; script-src 'self'; connect-src 'self' http://localhost:5189 https://localhost:7189; frame-ancestors 'none'; base-uri 'self'; form-action 'self'";
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
    context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";
    await next();
});

app.UseCors("ReactDev");
app.UseRateLimiter();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ShenaraDbContext>();
    var hasMigrationHistory = await HasTableAsync(dbContext, "__EFMigrationsHistory");
    var hasLegacyTables = await HasTableAsync(dbContext, "InventoryItems") || await HasTableAsync(dbContext, "BookingInquiries");
    var hasAppliedMigrations = hasMigrationHistory && await HasAppliedMigrationsAsync(dbContext);

    if (hasLegacyTables && !hasAppliedMigrations)
    {
        await BaselineExistingSchemaAsync(dbContext);
    }

    dbContext.Database.Migrate();
    await EnsureBookingInquirySchemaAsync(dbContext);
    await SyncServicePricingAsync(dbContext);

    await InventorySeedData.SeedInvoiceInventoryAsync(dbContext);
}

app.Run();

static async Task<bool> HasTableAsync(ShenaraDbContext dbContext, string tableName)
{
    var connection = dbContext.Database.GetDbConnection();
    var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

    if (shouldCloseConnection)
    {
        await connection.OpenAsync();
    }

    try
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT CASE WHEN OBJECT_ID(@tableName, 'U') IS NOT NULL THEN 1 ELSE 0 END";
        var parameter = command.CreateParameter();
        parameter.ParameterName = "@tableName";
        parameter.Value = tableName;
        command.Parameters.Add(parameter);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) == 1;
    }
    finally
    {
        if (shouldCloseConnection)
        {
            await connection.CloseAsync();
        }
    }
}

static async Task<bool> HasAppliedMigrationsAsync(ShenaraDbContext dbContext)
{
    var connection = dbContext.Database.GetDbConnection();
    var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

    if (shouldCloseConnection)
    {
        await connection.OpenAsync();
    }

    try
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "SELECT CASE WHEN EXISTS (SELECT 1 FROM [__EFMigrationsHistory]) THEN 1 ELSE 0 END";

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) == 1;
    }
    finally
    {
        if (shouldCloseConnection)
        {
            await connection.CloseAsync();
        }
    }
}

static async Task BaselineExistingSchemaAsync(ShenaraDbContext dbContext)
{
    var connection = dbContext.Database.GetDbConnection();
    var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

    if (shouldCloseConnection)
    {
        await connection.OpenAsync();
    }

    try
    {
        await using (var createHistoryCommand = connection.CreateCommand())
        {
            createHistoryCommand.CommandText = """
                IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
                BEGIN
                    CREATE TABLE [__EFMigrationsHistory] (
                        [MigrationId] nvarchar(150) NOT NULL,
                        [ProductVersion] nvarchar(32) NOT NULL,
                        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
                    );
                END;
                """;

            await createHistoryCommand.ExecuteNonQueryAsync();
        }

        var productVersion = typeof(DbContext).Assembly.GetName().Version?.ToString() ?? "9.0.0";

        foreach (var migration in dbContext.Database.GetMigrations())
        {
            await using var command = connection.CreateCommand();
            command.CommandText = """
                IF NOT EXISTS (SELECT 1 FROM [__EFMigrationsHistory] WHERE [MigrationId] = @migrationId)
                BEGIN
                    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
                    VALUES (@migrationId, @productVersion);
                END;
                """;

            var migrationIdParameter = command.CreateParameter();
            migrationIdParameter.ParameterName = "@migrationId";
            migrationIdParameter.Value = migration;
            command.Parameters.Add(migrationIdParameter);

            var productVersionParameter = command.CreateParameter();
            productVersionParameter.ParameterName = "@productVersion";
            productVersionParameter.Value = productVersion;
            command.Parameters.Add(productVersionParameter);

            await command.ExecuteNonQueryAsync();
        }
    }
    finally
    {
        if (shouldCloseConnection)
        {
            await connection.CloseAsync();
        }
    }
}

static async Task EnsureBookingInquirySchemaAsync(ShenaraDbContext dbContext)
{
    if (!await HasTableAsync(dbContext, "BookingInquiries") ||
        await HasColumnAsync(dbContext, "BookingInquiries", "RemoteAddress"))
    {
        return;
    }

    var connection = dbContext.Database.GetDbConnection();
    var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

    if (shouldCloseConnection)
    {
        await connection.OpenAsync();
    }

    try
    {
        await using var command = connection.CreateCommand();
        command.CommandText = "ALTER TABLE [BookingInquiries] ADD [RemoteAddress] nvarchar(64) NULL";
        await command.ExecuteNonQueryAsync();
    }
    finally
    {
        if (shouldCloseConnection)
        {
            await connection.CloseAsync();
        }
    }
}

static async Task<bool> HasColumnAsync(ShenaraDbContext dbContext, string tableName, string columnName)
{
    var connection = dbContext.Database.GetDbConnection();
    var shouldCloseConnection = connection.State != System.Data.ConnectionState.Open;

    if (shouldCloseConnection)
    {
        await connection.OpenAsync();
    }

    try
    {
        await using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT CASE WHEN COL_LENGTH(@tableName, @columnName) IS NOT NULL THEN 1 ELSE 0 END
            """;

        var tableParameter = command.CreateParameter();
        tableParameter.ParameterName = "@tableName";
        tableParameter.Value = tableName;
        command.Parameters.Add(tableParameter);

        var columnParameter = command.CreateParameter();
        columnParameter.ParameterName = "@columnName";
        columnParameter.Value = columnName;
        command.Parameters.Add(columnParameter);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) == 1;
    }
    finally
    {
        if (shouldCloseConnection)
        {
            await connection.CloseAsync();
        }
    }
}

static async Task SyncServicePricingAsync(ShenaraDbContext dbContext)
{
    var services = await dbContext.ServiceOfferings
        .Where(service => service.Name == "Balloon Decor" || service.Name == "Back Arch Styling")
        .ToListAsync();

    foreach (var service in services)
    {
        service.StartingPrice = service.Name switch
        {
            "Balloon Decor" => "From $45",
            "Back Arch Styling" => "From $220",
            _ => service.StartingPrice
        };
    }

    if (services.Count > 0)
    {
        await dbContext.SaveChangesAsync();
    }
}
