using EcommerceApp.Application;
using EcommerceApp.Application.Common;
using EcommerceApp.Application.Filters;
using EcommerceApp.Infrastructure;
using EcommerceApp.Infrastructure.Hubs;
using Hangfire;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;
using System.Threading.RateLimiting;

// User Roles: [Admin, Seller, Customer]

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddInfrastructure(builder.Configuration.GetConnectionString(builder.Environment.IsDevelopment() ? "DbConnection" : "DbConnectionThroughDocker")!, builder.Configuration);
builder.Services.AddHangfire(config => { config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangFireDbConnection")); });
builder.Services.AddHangfireServer();
builder.Services.AddApplication();
builder.Services.AddControllers(options => { options.Filters.Add<ValidationFilter>(); });
builder.Services.AddSignalR();
builder.Services.AddRateLimiter(rateLimiterOptions => 
{
    rateLimiterOptions.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    rateLimiterOptions.AddPolicy("RoleBasedRateLimiterPolicy", context =>
    {
        var role = context.User?.FindFirst(ClaimTypes.Role)?.Value ?? "Anonymous";
        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
             context.Connection.RemoteIpAddress?.ToString() ?? "anonymous"; 

        /*
          - If two users get same partition key they share the same requests bucket which we dont want.
          - A partition defines who shares a bucket
            - Bad partition "seller"
            - All sellers share limits ❌
            
            - Good partition "seller:42"
            - Each seller has their own bucket ✅
        */
        var partitionKey = $"{role}:{userId}";

        Console.WriteLine("RATELIMITER REACHED: ");
        Console.WriteLine("Role: " + role);
        Console.WriteLine("UserId: " + userId);

        return role switch
        {
            // 10 concurrent admins can access the system without any rate limits on apis.
            AppRoles.Admin => RateLimitPartition.GetConcurrencyLimiter(partitionKey, _ => new ConcurrencyLimiterOptions
            {
                PermitLimit = 10,
                QueueLimit = 0
            }),
            // A seller user can hit 300 requests per minute.
            AppRoles.Seller => RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 300,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }),
            // A customer user can hit 200 requests per minute.
            AppRoles.Customer => RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 200,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }),
            // A anonymous user can hit 100 requests per minute. 
            _ => RateLimitPartition.GetFixedWindowLimiter(partitionKey, _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1),
                QueueLimit = 0
            }),
        };
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>();

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwt!.Issuer,
            ValidAudience = jwt.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwt.Key)),

            ClockSkew = TimeSpan.Zero
        };

        // For Authorized Hubs.
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/chat"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Ecommerce API", Version = "v1" });

    // Add Bearer token support
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token in the text input below.\r\n\r\nExample: \"Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6...\""
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
            new string[] {}
        }
    });
});

builder.Services.AddCors(opt =>
{
    opt.AddPolicy(name: "CorsPolicy", builder =>
    {
        builder.WithOrigins("http://localhost:3000") // react app origin
               .AllowAnyHeader()
               .AllowAnyMethod()
               .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Configure the HTTP request pipeline.

app.UseStaticFiles(); 

app.UseCors("CorsPolicy");
app.UseHttpsRedirection();

app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

app.MapControllers().RequireRateLimiting("RoleBasedRateLimiterPolicy");
app.MapHub<ChatHub>("/hubs/chat");

app.Run();


