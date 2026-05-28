using Microsoft.EntityFrameworkCore;
using CidadeMelhorApi.Data;
using CidadeMelhorApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=CidadeMelhor.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("PermitirTudo",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.Migrate();

    if (!context.Admins.Any())
    {
        context.Admins.Add(new Admin
        {
            Nome = "Administrador",
            Email = "admin@gmail.com",
            Senha = "admin123"
        });

        context.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("PermitirTudo");
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();

app.Run();
