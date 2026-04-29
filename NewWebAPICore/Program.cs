using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NewWebAPICore.Filters;
using NewWebAPICore.Middleware;
using NewWebAPICore.Service;
using System.Security.Claims;
using System.Text;
using WebAPICore.Data;
using WebAPICore.Model;
using WebAPICore.ServiceToken;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "NewWebAPICore",
        Version = "v1"
    });

    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter JWT token like: Bearer {your_token_here}"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddScoped<BlobService>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    var connectionString = config["ConnectionString"];
    var container = config["ContainerName"];
    return new BlobService(connectionString, container);
});


builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuditLogFilter>();
builder.Services.AddScoped<CustomResultFilter>();
builder.Services.AddScoped<ExceptionFilter>();
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddDbContext<AppDbContext>(x => x.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
    sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure();
    }));

builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };

        options.Events = new JwtBearerEvents
        {

            OnChallenge = async context =>
            {
                context.HandleResponse();

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    statusCode = 401,
                    message = "You are not authenticated. Please login."
                });
            },

            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    statusCode = 403,
                    message = "You are not authorized to access this resource."
                });
            }
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NewWebAPICore v1");
    c.RoutePrefix = "swagger";

    c.DocumentTitle = "Product Web API - API Documentation";
});

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<GlobalExceptionMiddleware>();
app.MapControllers();

#region MINIMAL APIs
//app.MapGet("/api/health", () =>
//{
//    return Results.Ok(new
//    {
//        status = "Healthy",
//        time = DateTime.UtcNow
//    });
//});

//app.MapGet("/api/me", (ClaimsPrincipal user) =>
//{
//    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//    var role = user.FindFirst(ClaimTypes.Role)?.Value;

//    return Results.Ok(new
//    {
//        userId,
//        role
//    });
//})
//.RequireAuthorization();
//app.MapGet("/api/admin/ping", () =>
//{
//    return Results.Ok("Admin access granted");
//})
//.RequireAuthorization();

//app.MapGet("/api/product/count", async (AppDbContext db) =>
//{
//    var count = await db.Productions.CountAsync();
//    return Results.Ok(new { totalProducts = count });
//})
//.RequireAuthorization();
#endregion

app.Run();
