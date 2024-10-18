using Microsoft.EntityFrameworkCore;
using OrderMicroService;
using OrderMicroService.Data.Services;

var builder = WebApplication.CreateBuilder(args);

//Configure Connection to PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Configure Services
builder.Services.AddScoped<IOrderService, OrderService>();

//Configure HttpClient
builder.Services.AddHttpClient("ShoppingCartMicroService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7151/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("ProfileMicroService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7009/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

builder.Services.AddHttpClient("ProductMicroService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7104/api/");
    client.Timeout = TimeSpan.FromSeconds(30);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
