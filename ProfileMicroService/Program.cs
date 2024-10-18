using Microsoft.EntityFrameworkCore;
using ProfileMicroService;
using ProfileMicroService.Services;

var builder = WebApplication.CreateBuilder(args);

//Configure Connection To PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//Services Configuration
builder.Services.AddScoped<IAddressService, AddressService>();

//Configure HttpClient

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
