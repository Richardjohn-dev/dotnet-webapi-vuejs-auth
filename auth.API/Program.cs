using Auth.API.Data;
using Auth.API.Helpers;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// api is on 8000
app.UseCors(options => options
             .WithOrigins(new[] { "http://localhost:3000", "http://localhost:8080", "http://localhost:4200" })
             .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowCredentials()
         );
// allowCredentials is needed to send cookies to the front end

app.UseAuthorization();

app.MapControllers();

app.Run();
