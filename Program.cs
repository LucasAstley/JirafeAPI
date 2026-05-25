using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Microsoft.Extensions.Primitives;
using System.Text;
using JirafeAPI.Data;
using JirafeAPI.Hubs;
using JirafeAPI.Repositories;
using JirafeAPI.Services;
using JirafeAPI.Utilities;

Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var configuration = builder.Configuration;

var databaseProvider = configuration["DATABASE_PROVIDER"]
                       ?? throw new InvalidOperationException("DATABASE_PROVIDER environment variable is required.");
var sqliteConnectionString = configuration["SQLITE_CONNECTION_STRING"]
                             ?? configuration.GetConnectionString("DefaultConnection")
                             ?? throw new InvalidOperationException(
                                 "SQLITE_CONNECTION_STRING environment variable is required for Sqlite provider.");
var postgresConnectionString = configuration["POSTGRESQL_CONNECTION_STRING"];
var jwtSecret = configuration["JWT_SECRET"]
                ?? throw new InvalidOperationException("JWT_SECRET environment variable is required.");
var jwtIssuer = configuration["JWT_ISSUER"]
                ?? throw new InvalidOperationException("JWT_ISSUER environment variable is required.");
var jwtAudience = configuration["JWT_AUDIENCE"]
                  ?? throw new InvalidOperationException("JWT_AUDIENCE environment variable is required.");
var jwtExpirationMinutesRaw = configuration["JWT_EXPIRATION_MINUTES"]
                              ?? throw new InvalidOperationException(
                                  "JWT_EXPIRATION_MINUTES environment variable is required.");
if (!int.TryParse(jwtExpirationMinutesRaw, out var jwtExpirationMinutes))
{
    throw new InvalidOperationException("JWT_EXPIRATION_MINUTES must be a valid integer.");
}

var corsOriginsRaw = configuration["CORS_ALLOWED_ORIGINS"]
                     ?? throw new InvalidOperationException("CORS_ALLOWED_ORIGINS environment variable is required.");
var corsOrigins = corsOriginsRaw
    .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
if (corsOrigins.Length == 0)
{
    throw new InvalidOperationException("CORS_ALLOWED_ORIGINS must contain at least one origin.");
}

var aspnetcoreUrls = configuration["ASPNETCORE_URLS"];
if (!string.IsNullOrWhiteSpace(aspnetcoreUrls))
{
    builder.WebHost.UseUrls(aspnetcoreUrls);
}
else
{
    var apiHost = configuration["API_HOST"];
    var apiPort = configuration["API_PORT"];
    if (!string.IsNullOrWhiteSpace(apiHost) && !string.IsNullOrWhiteSpace(apiPort))
    {
        builder.WebHost.UseUrls($"http://{apiHost}:{apiPort}");
    }
}

builder.Services.AddControllers();
builder.Services.AddRouting(options => options.LowercaseUrls = true);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "JirafeAPI",
        Version = "v1",
        Description = "API documentation for Jirafe"
    });

    var jwtSecurityScheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Description = "Paste your JWT token here",
    };

    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, jwtSecurityScheme);
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference(
                JwtBearerDefaults.AuthenticationScheme,
                hostDocument: document,
                externalResource: null
            ),
            new List<string>()
        }
    });
});

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
builder.Services.AddSingleton<IBoardRealtimeNotifier, BoardRealtimeNotifier>();

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

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!StringValues.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs/board"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("JirafeCorsPolicy", policy =>
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

app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "JirafeAPI v1");
    options.RoutePrefix = "swagger";
});

if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseCors("JirafeCorsPolicy");

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
