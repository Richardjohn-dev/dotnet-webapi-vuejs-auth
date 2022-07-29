using RoleBasedIdentityAuthentication.API.Authentication;
using RoleBasedIdentityAuthentication.API.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors();

builder.Services.ConfigurePersistenceServices(builder.Configuration);
builder.Services.ConfigureIdentityServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ApplicationDbSeeder>();
    await context.ManageDataAsync();
}

app.UseSwagger();
app.UseSwaggerUI();


app.UseHttpsRedirection();

// api is on 8000
app.UseCors(options => options
             .WithOrigins(new[] { "http://localhost:8080" })
             .AllowAnyHeader()
             .AllowAnyMethod()
             .AllowCredentials()
         );


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
