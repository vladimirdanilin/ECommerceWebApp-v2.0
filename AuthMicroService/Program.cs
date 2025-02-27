using AuthMicroService;
using AuthMicroService.Data;
using AuthMicroService.Data.Services;
using AuthMicroService.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

var password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD");
if (!string.IsNullOrEmpty(password))
{
    connectionString = connectionString.Replace("Password=", $"Password={password}");
}

//Configure connection to PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

//Configure Services
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();

//Add ASP.NET Identity
builder.Services.AddIdentity<User, IdentityRole<int>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Add services to the container.
builder.Services.AddControllers();

//Configure JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

//Add Authorization and Policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Require" + Roles.SuperAdmin, policy => policy.RequireRole(Roles.SuperAdmin));
    options.AddPolicy("Require" + Roles.SalesManager, policy => policy.RequireRole(Roles.SalesManager));
    options.AddPolicy("Require" + Roles.WarehouseManager, policy => policy.RequireRole(Roles.WarehouseManager));
    options.AddPolicy("Require" + Roles.Customer, policy => policy.RequireRole(Roles.Customer));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

//Roles Configuration
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    await CreateRoles(roleManager, userManager);
}

async Task CreateRoles(RoleManager<IdentityRole<int>> roleManager, UserManager<User> userManager)
{
    var roles = new[] { Roles.SuperAdmin, Roles.SalesManager, Roles.WarehouseManager, Roles.Customer };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole<int>(role));
        }
    }

    string superAdminEmail = "volodyadanilin@gmail.com";
    var superAdmin = await userManager.FindByEmailAsync(superAdminEmail);

    if (superAdmin == null)
    {
        string superAdminPassword = "Vova25!";

        superAdmin = new User
        {
            FirstName = "Vladimir",
            LastName = "Danilin",
            UserName = superAdminEmail,
            Email = superAdminEmail,
            NormalizedEmail = userManager.NormalizeEmail(superAdminEmail)
        };

        var result = await userManager.CreateAsync(superAdmin, superAdminPassword);
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(superAdmin, Roles.SuperAdmin);
        }
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
