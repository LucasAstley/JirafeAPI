using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using JirafeAPI.Data;
using JirafeAPI.Hubs;
using JirafeAPI.Repositories;
using JirafeAPI.Services;
using JirafeAPI.Utilities;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
var databaseProvider = Environment.GetEnvironmentVariable("DATABASE_PROVIDER") ?? "Sqlite";
var sqliteConnectionString = Environment.GetEnvironmentVariable("SQLITE_CONNECTION_STRING") ?? "Data Source=jirafe.db";
var postgresConnectionString = Environment.GetEnvironmentVariable("POSTGRESQL_CONNECTION_STRING");
var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ??
                "your_very_secure_secret_key_change_this_in_production_at_least_32_characters_long";
var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "JirafeAPI";
var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "JirafeClient";
var jwtExpirationMinutes = int.Parse(Environment.GetEnvironmentVariable("JWT_EXPIRATION_MINUTES") ?? "15");
var corsOrigins = Environment.GetEnvironmentVariable("CORS_ORIGINS")?.Split(",") ?? new[] { "http://localhost:3000" };

builder.Services.AddControllers();

builder.Services.AddOpenApi();

if (databaseProvider.Equals("Sqlite", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<TaskBoardDbContext>(options =>
        options.UseSqlite(sqliteConnectionString));
}
else if (databaseProvider.Equals("PostgreSQL", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddDbContext<TaskBoardDbContext>(options =>
        options.UseNpgsql(postgresConnectionString));
}
else
{
    throw new InvalidOperationException("Unsupported database provider.");
}

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();
builder.Services.AddScoped<IWorkspaceMemberRepository, WorkspaceMemberRepository>();
builder.Services.AddScoped<IBoardRepository, BoardRepository>();
builder.Services.AddScoped<IBoardMemberRepository, BoardMemberRepository>();
builder.Services.AddScoped<IListRepository, ListRepository>();
builder.Services.AddScoped<ICardRepository, CardRepository>();
builder.Services.AddScoped<ICardMemberRepository, CardMemberRepository>();
builder.Services.AddScoped<ILabelRepository, LabelRepository>();
builder.Services.AddScoped<ICardLabelRepository, CardLabelRepository>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddSingleton(new JwtHelper(jwtSecret, jwtIssuer, jwtAudience, jwtExpirationMinutes));

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IWorkspaceService, WorkspaceService>();
builder.Services.AddScoped<IBoardService, BoardService>();
builder.Services.AddScoped<IListService, ListService>();
builder.Services.AddScoped<ICardService, CardService>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILabelService, LabelService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
        policy.WithOrigins(corsOrigins)
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();
    });
});

builder.Services.AddSignalR();


var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskBoardDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigins");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<BoardHub>("/hubs/board");

app.MapGet("/health", () =>
        Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }))
    .WithName("GetHealth");

app.MapGet("/health/live", () =>
        Results.Ok(new { status = "live", timestamp = DateTime.UtcNow }))
    .WithName("GetHealthLive");

app.Run();