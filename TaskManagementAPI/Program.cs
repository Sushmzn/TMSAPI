using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagementAPI.Constants;
using TaskManagementAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    // Configure Swagger to support JWT authorization
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        Description = "Enter 'Bearer' followed by a space and the JWT token"
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
            new string[] { }
        }
    });
});

// Configure DBContext for PostgreSQL
builder.Services.AddDbContext<TaskManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = "default-issuer",  // Use a default issuer
            ValidAudience = "default-audience",  // Use a default audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("312b4eb8b2403522310e5abc8275513e97c53e391d5054585a00622dcbb1a973"))
        };
    });


// Configure authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PermissionConstants.TaskView, policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permission" &&
                                        c.Value.Split(',').Contains(PermissionConstants.TaskView))
        ));

    options.AddPolicy(PermissionConstants.TaskCreate, policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permission" &&
                                        c.Value.Split(',').Contains(PermissionConstants.TaskCreate))
        ));

    options.AddPolicy(PermissionConstants.TaskDelete, policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permission" &&
                                        c.Value.Split(',').Contains(PermissionConstants.TaskDelete))
        ));

    options.AddPolicy(PermissionConstants.TaskEdit, policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c => c.Type == "permission" &&
                                        c.Value.Split(',').Contains(PermissionConstants.TaskEdit))
        ));
});

// Register JwtTokenService for dependency injection
builder.Services.AddTransient<JwtTokenService>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<TaskManagementDbContext>();
    dbContext.Database.Migrate(); // Applies pending migrations
}
// Configure middleware pipeline
app.UseAuthentication();  // Adds JWT Authentication
app.UseAuthorization();   // Adds Authorization Middleware

// Global exception handling middleware
app.UseMiddleware<TaskManagementAPI.Middlewares.GlobalExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction() || app.Environment.IsStaging())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TaskManagementAPI V1");
        c.RoutePrefix = "swagger";  // Serve Swagger UI at the /swagger path
    });
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();