using Microsoft.EntityFrameworkCore;
using CidadeMelhorApi.Data;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options => 
    options.UseSqlite("Data Source=CidadeMelhor.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseCors("AllowAll");

var frontendPath = Path.Combine(builder.Environment.ContentRootPath, "Frontend");

if (Directory.Exists(frontendPath))
{
    var frontendFiles = new PhysicalFileProvider(frontendPath);

    app.UseDefaultFiles(new DefaultFilesOptions
    {
        FileProvider = frontendFiles
    });

    app.UseStaticFiles(new StaticFileOptions
    {
        FileProvider = frontendFiles
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
